using System;
using System.Windows.Forms;

namespace TweetDck.Core.Controls{
    static class ControlExtensions{
        public static void InvokeSafe(this Form form, Action func){
            if (form.InvokeRequired){
                form.Invoke(func);
            }
            else{
                func();
            }
        }

        public static void SetValueInstant(this ProgressBar bar, int value){
            if (value == bar.Maximum){
                bar.Value = value;
                bar.Value = value-1;
                bar.Value = value;
            }
            else{
                bar.Value = value+1;
                bar.Value = value;
            }
        }
    }
}
