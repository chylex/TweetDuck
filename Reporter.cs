using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TweetDck.Core.Other;

namespace TweetDck{
    class Reporter{
        private readonly string logFile;

        public Reporter(string logFile){
            this.logFile = logFile;
        }

        public bool Log(string data){
            StringBuilder build = new StringBuilder();

            if (!File.Exists(logFile)){
                build.Append("Please, report all issues to: https://github.com/chylex/TweetDuck/issues\r\n\r\n");
            }

            build.Append("[").Append(DateTime.Now.ToString("G", CultureInfo.CurrentCulture)).Append("]\r\n");
            build.Append(data).Append("\r\n\r\n");

            try{
                File.AppendAllText(logFile, build.ToString(), Encoding.UTF8);
                return true;
            }catch{
                return false;
            }
        }

        public void HandleException(string caption, string message, bool canIgnore, Exception e){
            Log(e.ToString());

            FormMessage form = new FormMessage(caption, message+"\r\nError: "+e.Message, canIgnore ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            
            form.AddButton("Exit");
            Button btnIgnore = form.AddButton("Ignore");

            Button btnOpenLog = new Button{
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Location = new Point(12, 12),
                Margin = new Padding(0, 0, 48, 0),
                Size = new Size(88, 26),
                Text = "Show Error Log",
                UseVisualStyleBackColor = true
            };

            btnOpenLog.Click += (sender, args) => {
                using(Process.Start(logFile)){}
            };

            form.AddActionControl(btnOpenLog);

            if (!canIgnore){
                btnIgnore.Enabled = false;
            }

            if (form.ShowDialog() == DialogResult.OK){
                if (form.ClickedButton == btnIgnore){
                    return;
                }
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
            form.AddButton("Exit");
            form.ShowDialog();

            try{
                Process.GetCurrentProcess().Kill();
            }catch{
                Environment.FailFast(message, new Exception(message));
            }
        }
    }
}
