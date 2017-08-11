using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other.Media{
    class VideoPlayer{
        private readonly string PlayerExe = Path.Combine(Program.ProgramPath, "TweetDuck.Video.exe");

        private readonly Form owner;
        private Process currentProcess;
        private string lastUrl;

        public VideoPlayer(Form owner){
            this.owner = owner;
        }

        public void Launch(string url){
            Close();

            lastUrl = url;

            try{
                if ((currentProcess = Process.Start(new ProcessStartInfo{
                    FileName = PlayerExe,
                    Arguments = $"{owner.Handle} \"{url}\"",
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
            }
        }

        private void process_Exited(object sender, EventArgs e){
            switch(currentProcess.ExitCode){
                case 2: // CODE_LAUNCH_FAIL
                    if (FormMessage.Error("Video Playback Error", "Error launching video player, this may be caused by missing Windows Media Player. Do you want to open the video in a browser?", FormMessage.Yes, FormMessage.No)){
                        BrowserUtils.OpenExternalBrowser(lastUrl);
                    }

                    break;

                case 3: // CODE_MEDIA_ERROR
                    FormMessage.Error("Video Playback Error", "The video could not be loaded or rendered correctly.", FormMessage.OK);
                    break;
            }

            currentProcess.Dispose();
            currentProcess = null;
        }
    }
}
