using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using TweetDck.Core.Other;

namespace TweetDck{
    static class Reporter{
        public static void HandleException(string caption, string message, bool canIgnore, Exception e){
            Program.Log(e.ToString());

            FormMessage form = new FormMessage(caption, message+"\r\nError: "+e.Message, canIgnore ? MessageBoxIcon.Warning : MessageBoxIcon.Error);
            
            form.AddButton("Exit");
            Button btnIgnore = form.AddButton("Ignore");

            Button btnOpenLog = new Button{
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Location = new Point(12, 12),
                Margin = new Padding(0, 0, 48, 0),
                Size = new Size(88, 26),
                Text = "Open Log File",
                UseVisualStyleBackColor = true
            };

            btnOpenLog.Click += (sender, args) => Process.Start(Program.LogFilePath);

            form.AddActionControl(btnOpenLog);

            if (!canIgnore){
                btnIgnore.Enabled = false;
            }

            if (form.ShowDialog() == DialogResult.OK){
                if (form.ClickedButton == btnIgnore){
                    return;
                }
            }

            Environment.Exit(1);
        }
    }
}
