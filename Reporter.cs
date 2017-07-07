using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TweetDuck.Core.Other;

namespace TweetDuck{
    sealed class Reporter{
        private readonly string logFile;

        public Reporter(string logFile){
            this.logFile = logFile;
        }

        public void SetupUnhandledExceptionHandler(string caption){
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
                if (args.ExceptionObject is Exception ex) {
                    HandleException(caption, "An unhandled exception has occurred.", false, ex);
                }
            };
        }

        public bool Log(string data){
            StringBuilder build = new StringBuilder();

            if (!File.Exists(logFile)){
                build.Append("Please, report all issues to: https://github.com/chylex/TweetDuck/issues\r\n\r\n");
            }

            build.Append("[").Append(DateTime.Now.ToString("G", Program.Culture)).Append("]\r\n");
            build.Append(data).Append("\r\n\r\n");

            try{
                File.AppendAllText(logFile, build.ToString(), Encoding.UTF8);
                return true;
            }catch{
                return false;
            }
        }

        public void HandleException(string caption, string message, bool canIgnore, Exception e){
            bool loggedSuccessfully = Log(e.ToString());

            FormMessage form = new FormMessage(caption, message+Environment.NewLine+"Error: "+e.Message, canIgnore ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            
            Button btnExit = form.AddButton("Exit");
            Button btnIgnore = form.AddButton("Ignore", DialogResult.Ignore);

            btnIgnore.Enabled = canIgnore;
            form.ActiveControl = canIgnore ? btnIgnore : btnExit;
            form.CancelButton = btnIgnore;

            Button btnOpenLog = new Button{
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Enabled = loggedSuccessfully,
                Font = SystemFonts.MessageBoxFont,
                Location = new Point(9, 12),
                Margin = new Padding(0, 0, 48, 0),
                Size = new Size(106, 26),
                Text = "Show Error Log",
                UseVisualStyleBackColor = true
            };

            btnOpenLog.Click += (sender, args) => {
                using(Process.Start(logFile)){}
            };

            form.AddActionControl(btnOpenLog);

            if (form.ShowDialog() == DialogResult.Ignore){
                return;
            }

            try{
                Process.GetCurrentProcess().Kill();
            }catch{
                Environment.FailFast(message, e);
            }
        }

        public static void HandleEarlyFailure(string caption, string message){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormMessage form = new FormMessage(caption, message, MessageBoxIcon.Error);
            form.ActiveControl = form.AddButton("Exit");
            form.ShowDialog();

            try{
                Process.GetCurrentProcess().Kill();
            }catch{
                Environment.FailFast(message, new Exception(message));
            }
        }
    }
}
