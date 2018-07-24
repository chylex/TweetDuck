using System.Collections.Generic;
using System.Windows.Forms;
using TweetDuck.Configuration;

namespace TweetDuck.Core.Other.Settings{
    class BaseTabSettings : UserControl{
        protected static UserConfig Config => Program.UserConfig;

        public IEnumerable<Control> InteractiveControls{
            get{
                IEnumerable<Control> FindInteractiveControls(Control parent){
                    foreach(Control control in parent.Controls){
                        if (control is Panel subPanel){
                            foreach(Control subControl in FindInteractiveControls(subPanel)){
                                yield return subControl;
                            }
                        }
                        else{
                            yield return control;
                        }
                    }
                }

                return FindInteractiveControls(this);
            }
        }

        protected BaseTabSettings(){
            Padding = new Padding(6);
        }

        public virtual void OnReady(){}
        public virtual void OnClosing(){}
    }
}
