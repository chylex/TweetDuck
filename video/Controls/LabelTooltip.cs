using System;
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
            control.MouseMove += (sender, args) => {
                SuspendLayout();

                Form form = control.FindForm();
                System.Diagnostics.Debug.Assert(form != null);
                
                Text = tooltipFunc(args);

                Point loc = form.PointToClient(control.Parent.PointToScreen(new Point(control.Location.X+(followCursor ? args.X : control.Width/2), 0)));
                loc.X = Math.Max(0, Math.Min(form.Width-Width, loc.X-Width/2));
                loc.Y -= Height-Margin.Top+Margin.Bottom;
                Location = loc;

                ResumeLayout();
                Visible = true;
            };

            control.MouseLeave += control_MouseLeave;
        }

        private void control_MouseLeave(object sender, EventArgs e){
            Visible = false;
        }
    }
}
