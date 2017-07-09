using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Configuration;

namespace TweetDuck.Core.Other.Settings{
    class BaseTabSettings : UserControl{
        protected static UserConfig Config => Program.UserConfig;

        public IEnumerable<Control> InteractiveControls{
            get{
                foreach(Panel panel in Controls.OfType<Panel>()){
                    foreach(Control control in panel.Controls){
                        yield return control;
                    }
                }
            }
        }

        public BaseTabSettings(){
            Padding = new Padding(6);
        }

        public virtual void OnReady(){}
        public virtual void OnClosing(){}

        protected static void PromptRestart(){
            if (FormMessage.Information(Program.BrandName+" Options", "The application must restart for the option to take place. Do you want to restart now?", FormMessage.Yes, FormMessage.No)){
                Program.Restart();
            }
        }
    }
}
