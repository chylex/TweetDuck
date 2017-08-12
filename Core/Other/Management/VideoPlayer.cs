using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Management{
    sealed class VideoPlayer : IDisposable{
        private readonly string PlayerExe = Path.Combine(Program.ProgramPath, "TweetDuck.Video.exe");

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

        private readonly Form owner;
        private Process currentProcess;
        private string lastUrl;

        public VideoPlayer(Form owner){
            this.owner = owner;
            this.owner.FormClosing += owner_FormClosing;
        }

        public void Launch(string url){
            Close();

            lastUrl = url;

            try{
                if ((currentProcess = Process.Start(new ProcessStartInfo{
                    FileName = PlayerExe,
                    Arguments = $"{owner.Handle} {Program.UserConfig.VideoPlayerVolume} \"{url}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                })) != null){
                    currentProcess.EnableRaisingEvents = true;
                    currentProcess.Exited += process_Exited;

                    #if DEBUG
                    currentProcess.BeginOutputReadLine();
                    currentProcess.OutputDataReceived += (sender, args) => Debug.WriteLine("VideoPlayer: "+args.Data);
                    #endif
                }
            }catch(Exception e){
                Program.Reporter.HandleException("Video Playback Error", "Error launching video player.", true, e);
            }
        }

        public void Close(){
            if (currentProcess != null){
                currentProcess.Exited -= process_Exited;

                try{
                    currentProcess.Kill();
                }catch{
                    // kill me instead then
                }

                currentProcess.Dispose();
                currentProcess = null;

                owner.InvokeAsyncSafe(TriggerProcessExitEventUnsafe);
            }
        }

        public void Dispose(){
            ProcessExited = null;
            Close();
        }

        private void owner_FormClosing(object sender, FormClosingEventArgs e){
            if (currentProcess != null){
                currentProcess.Exited -= process_Exited;
            }
        }

        private void process_Exited(object sender, EventArgs e){
            switch(currentProcess.ExitCode){
                case 3: // CODE_LAUNCH_FAIL
                    if (FormMessage.Error("Video Playback Error", "Error launching video player, this may be caused by missing Windows Media Player. Do you want to open the video in a browser?", FormMessage.Yes, FormMessage.No)){
                        BrowserUtils.OpenExternalBrowser(lastUrl);
                    }

                    break;

                case 4: // CODE_MEDIA_ERROR
                    if (FormMessage.Error("Video Playback Error", "The video could not be loaded, most likely due to unknown format. Do you want to open the video in a browser?", FormMessage.Yes, FormMessage.No)){
                        BrowserUtils.OpenExternalBrowser(lastUrl);
                    }

                    break;
            }

            currentProcess.Dispose();
            currentProcess = null;
            
            owner.InvokeAsyncSafe(TriggerProcessExitEventUnsafe);
        }

        private void TriggerProcessExitEventUnsafe(){
            ProcessExited?.Invoke(this, new EventArgs());
        }
    }
}
