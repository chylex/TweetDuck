using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TweetDuck.Video.Controls{
    sealed class LabelTooltip : Label{
        public LabelTooltip(){
            Visible = false;
        }

        public void AttachTooltip(Control control, bool followCursor, string tooltip){
            AttachTooltip(control, followCursor, args => tooltip);
        }

        public void AttachTooltip(Control control, bool followCursor, Func<MouseEventArgs, string> tooltipFunc){
            control.MouseEnter += control_MouseEnter;
            control.MouseLeave += control_MouseLeave;

            control.MouseMove += (sender, args) => {
                Form form = control.FindForm();
                Debug.Assert(form != null);
                
                Text = tooltipFunc(args);
                Location = form.PointToClient(control.Parent.PointToScreen(new Point(control.Location.X-Width/2+(followCursor ? args.X : control.Width/2), -Height+Margin.Top-Margin.Bottom)));;
            };
        }

        private void control_MouseEnter(object sender, EventArgs e){
            Visible = true;
        }

        private void control_MouseLeave(object sender, EventArgs e){
            Visible = false;
        }
    }
}
