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
        public bool Running => currentInstance != null && currentInstance.Running;

        public event EventHandler ProcessExited;

        private readonly FormBrowser owner;
        private string lastUrl;
        private string lastUsername;

        private Instance currentInstance;
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
                DuplexPipe.Server pipe = DuplexPipe.CreateServer();
                pipe.DataIn += pipe_DataIn;

                Process process;

                if ((process = Process.Start(new ProcessStartInfo{
                    FileName = Path.Combine(Program.ProgramPath, "TweetDuck.Video.exe"),
                    Arguments = $"{owner.Handle} {Program.UserConfig.VideoPlayerVolume} \"{url}\" \"{pipe.GenerateToken()}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                })) != null){
                    currentInstance = new Instance(process, pipe);

                    process.EnableRaisingEvents = true;
                    process.Exited += process_Exited;
                    
                    process.BeginOutputReadLine();
                    process.OutputDataReceived += process_OutputDataReceived;

                    pipe.DisposeToken();
                }
                else{
                    pipe.DataIn -= pipe_DataIn;
                    pipe.Dispose();
                }
            }catch(Exception e){
                Program.Reporter.HandleException("Video Playback Error", "Error launching video player.", true, e);
            }
        }

        public void SendKeyEvent(Keys key){
            currentInstance?.Pipe.Write("key", ((int)key).ToString());
        }

        private void pipe_DataIn(object sender, DuplexPipe.PipeReadEventArgs e){
            owner.InvokeSafe(() => {
                switch(e.Key){
                    case "vol":
                        if (int.TryParse(e.Data, out int volume) && volume != Program.UserConfig.VideoPlayerVolume){
                            Program.UserConfig.VideoPlayerVolume = volume;
                            Program.UserConfig.Save();
                        }

                        break;

                    case "download":
                        if (!string.IsNullOrEmpty(lastUrl)){
                            owner.AnalyticsFile.DownloadedVideos.Trigger();
                            TwitterUtils.DownloadVideo(lastUrl, lastUsername);
                        }

                        break;

                    case "rip":
                        currentInstance?.Dispose();
                        currentInstance = null;

                        isClosing = false;
                        TriggerProcessExitEventUnsafe();
                        break;
                }
            });
        }

        public void Close(){
            if (currentInstance != null){
                if (isClosing){
                    Destroy();
                    isClosing = false;
                }
                else{
                    isClosing = true;
                    currentInstance.Process.Exited -= process_Exited;
                    currentInstance.Pipe.Write("die");
                }
            }
        }

        public void Dispose(){
            ProcessExited = null;

            isClosing = true;
            Destroy();
        }

        private void Destroy(){
            if (currentInstance != null){
                currentInstance.KillAndDispose();
                currentInstance = null;

                TriggerProcessExitEventUnsafe();
            }
        }

        private void owner_FormClosing(object sender, FormClosingEventArgs e){
            if (currentInstance != null){
                currentInstance.Process.Exited -= process_Exited;
            }
        }

        private void process_OutputDataReceived(object sender, DataReceivedEventArgs e){
            if (!string.IsNullOrEmpty(e.Data)){
                Program.Reporter.Log("[VideoPlayer] "+e.Data);
            }
        }

        private void process_Exited(object sender, EventArgs e){
            if (currentInstance == null){
                return;
            }

            int exitCode = currentInstance.Process.ExitCode;

            currentInstance.Dispose();
            currentInstance = null;

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

        private sealed class Instance : IDisposable{
            public bool Running{
                get{
                    Process.Refresh();
                    return !Process.HasExited;
                }
            }

            public Process Process { get; }
            public DuplexPipe.Server Pipe { get; }

            public Instance(Process process, DuplexPipe.Server pipe){
                this.Process = process;
                this.Pipe = pipe;
            }

            public void KillAndDispose(){
                try{
                    Process.Kill();
                }catch{
                    // kill me instead then
                }

                Dispose();
            }

            public void Dispose(){
                Process.Dispose();
                Pipe.Dispose();
            }
        }
    }
}
