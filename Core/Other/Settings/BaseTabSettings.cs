using System.Diagnostics;
using System.Windows.Forms;
using TweetDck.Configuration;

namespace TweetDck.Core.Other.Settings{
    partial class BaseTabSettings : UserControl{
        protected static UserConfig Config{
            get{
                return Program.UserConfig;
            }
        }

        public bool Ready { get; set; }

        public BaseTabSettings(){
            Padding = new Padding(6);
        }

        protected static void PromptRestart(){
            if (MessageBox.Show("The application must restart for the setting to take place. Do you want to restart now?", Program.BrandName+" Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes){
                Process.Start(Application.ExecutablePath, "-restart");
                Application.Exit();
            }
        }
    }
}
