using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TweetDuck.Core.Utils;
using TweetLib.Communication;

namespace TweetDuck.Core.Controls{
    static class ControlExtensions{
        public static readonly Point InvisibleLocation = new Point(-32000, -32000);

        public static void InvokeSafe(this Control control, Action func){
            if (control.InvokeRequired){
                control.Invoke(func);
            }
            else{
                func();
            }
        }

        public static void InvokeAsyncSafe(this Control control, Action func){
            control.BeginInvoke(func);
        }

        public static float GetDPIScale(this Control control){
            using(Graphics graphics = control.CreateGraphics()){
                return graphics.DpiY/96F;
            }
        }

        public static bool IsFullyOutsideView(this Form form){
            return !Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(form.Bounds));
        }

        public static void MoveToCenter(this Form targetForm, Form parentForm){
            targetForm.Location = new Point(parentForm.Location.X+parentForm.Width/2-targetForm.Width/2, parentForm.Location.Y+parentForm.Height/2-targetForm.Height/2);
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

        public static void SetValueSafe(this NumericUpDown numUpDown, int value){
            if (value >= numUpDown.Minimum && value <= numUpDown.Maximum){
                numUpDown.Value = value;
            }
        }

        public static void SetValueSafe(this TrackBar trackBar, int value){
            if (value >= trackBar.Minimum && value <= trackBar.Maximum){
                trackBar.Value = value;
            }
        }

        public static bool AlignValueToTick(this TrackBar trackBar){
            if (trackBar.Value % trackBar.SmallChange != 0){
                trackBar.Value = trackBar.SmallChange*(int)Math.Floor(((double)trackBar.Value/trackBar.SmallChange)+0.5);
                return false;
            }
            else return true;
        }

        public static void SetElevated(this Button button){
            button.Text = " "+button.Text;
            button.FlatStyle = FlatStyle.System;
            Comms.SendMessage(button.Handle, NativeMethods.BCM_SETSHIELD, 0, 1);
        }

        public static void EnableMultilineShortcuts(this TextBox textBox){
            textBox.KeyDown += (sender, args) => {
                if (args.Control && args.KeyCode == Keys.A){
                    ((TextBox)sender).SelectAll();
                    args.SuppressKeyPress = true;
                    args.Handled = true;
                }
            };
        }
    }
}
