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
            Padding = new Padding(6,6,6,6);
        }
    }
}
