using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetDck.Core.Controls{
    static class ControlExtensions{
        public static void InvokeSafe(this Control control, Action func){
            if (form.InvokeRequired){
                form.Invoke(func);
            }
            else{
                func();
            }
        }

        public static void MoveToCenter(this Form targetForm, Form parentForm){
            targetForm.Location = new Point(parentForm.Location.X+parentForm.Width/2-targetForm.Width/2,parentForm.Location.Y+parentForm.Height/2-targetForm.Height/2);
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
