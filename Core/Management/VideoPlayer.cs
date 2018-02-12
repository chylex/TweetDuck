using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;
using TweetLib.Communication;

namespace TweetDuck.Core.Management{
    sealed class VideoPlayer : IDisposable{
        public bool Running{
            get{
                if (currentProcess == null){
                    return false;
                }

                currentProcess.Refresh();
                return !currentProcess.HasExited;
            }
        }

        public event EventHandler ProcessExited;

        private readonly FormBrowser owner;
        private string lastUrl;
        private string lastUsername;

        private Process currentProcess;
        private DuplexPipe.Server currentPipe;
        private bool isClosing;

        public VideoPlayer(FormBrowser owner){
            this.owner = owner;
            this.owner.FormClosing += owner_FormClosing;
        }

        public void Launch(string url, string username){
            if (Running){
                Destroy();
                isClosing = false;
            }

            lastUrl = url;
            lastUsername = username;
            
            try{
                currentPipe = DuplexPipe.CreateServer();
                currentPipe.DataIn += currentPipe_DataIn;

                if ((currentProcess = Process.Start(new ProcessStartInfo{
                    FileName = Path.Combine(Program.ProgramPath, "TweetDuck.Video.exe"),
                    Arguments = $"{owner.Handle} {Program.UserConfig.VideoPlayerVolume} \"{url}\" \"{currentPipe.GenerateToken()}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                })) != null){
                    currentProcess.EnableRaisingEvents = true;
                    currentProcess.Exited += process_Exited;
                    
                    currentProcess.BeginOutputReadLine();
                    currentProcess.OutputDataReceived += process_OutputDataReceived;
                }

                currentPipe.DisposeToken();
            }catch(Exception e){
                Program.Reporter.HandleException("Video Playback Error", "Error launching video player.", true, e);
            }
        }

        public void SendKeyEvent(Keys key){
            currentPipe?.Write("key", ((int)key).ToString());
        }

        private void currentPipe_DataIn(object sender, DuplexPipe.PipeReadEventArgs e){
            owner.InvokeSafe(() => {
                switch(e.Key){
                    case "vol":
                        if (int.TryParse(e.Data, out int volume) && volume != Program.UserConfig.VideoPlayerVolume){
                            Program.UserConfig.VideoPlayerVolume = volume;
                            Program.UserConfig.Save();
                        }

                        break;

                    case "download":
                        owner.AnalyticsFile.DownloadedVideos.Trigger();
                        TwitterUtils.DownloadVideo(lastUrl, lastUsername);
                        break;

                    case "rip":
                        currentPipe.Dispose();
                        currentPipe = null;
                    
                        currentProcess.Dispose();
                        currentProcess = null;

                        isClosing = false;
                        TriggerProcessExitEventUnsafe();
                        break;
                }
            });
        }

        public void Close(){
            if (currentProcess != null){
                if (isClosing){
                    Destroy();
                    isClosing = false;
                }
                else{
                    isClosing = true;
                    currentProcess.Exited -= process_Exited;
                    currentPipe.Write("die");
                }
            }
        }

        public void Dispose(){
            ProcessExited = null;

            isClosing = true;
            Destroy();
        }

        private void Destroy(){
            if (currentProcess != null){
                try{
                    currentProcess.Kill();
                }catch{
                    // kill me instead then
                }

                currentProcess.Dispose();
                currentProcess = null;
                
                currentPipe.Dispose();
                currentPipe = null;

                TriggerProcessExitEventUnsafe();
            }
        }

        private void owner_FormClosing(object sender, FormClosingEventArgs e){
            if (currentProcess != null){
                currentProcess.Exited -= process_Exited;
            }
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e){
            if (!string.IsNullOrEmpty(e.Data)){
                Program.Reporter.Log("[VideoPlayer] "+e.Data);
            }
        }

        private void process_Exited(object sender, EventArgs e){
            int exitCode = currentProcess.ExitCode;

            currentProcess.Dispose();
            currentProcess = null;

            currentPipe.Dispose();
            currentPipe = null;

            switch(exitCode){
                case 3: // CODE_LAUNCH_FAIL
                    if (FormMessage.Error("Video Playback Error", "Error launching video player, this may be caused by missing Windows Media Player. Do you want to open the video in your browser?", FormMessage.Yes, FormMessage.No)){
                        BrowserUtils.OpenExternalBrowser(lastUrl);
                    }

                    break;

                case 4: // CODE_MEDIA_ERROR
                    if (FormMessage.Error("Video Playback Error", "The video could not be loaded, most likely due to unknown format. Do you want to open the video in your browser?", FormMessage.Yes, FormMessage.No)){
                        BrowserUtils.OpenExternalBrowser(lastUrl);
                    }

                    break;
            }
            
            owner.InvokeAsyncSafe(TriggerProcessExitEventUnsafe);
        }

        private void TriggerProcessExitEventUnsafe(){
            ProcessExited?.Invoke(this, EventArgs.Empty);
        }
    }
}
