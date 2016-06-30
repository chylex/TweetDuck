using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TweetDck.Core.Utils{
    [Serializable]
    class WindowState{
        private Rectangle rect;
        private bool isMaximized;

        public void Save(Form form){
            rect = form.WindowState == FormWindowState.Normal ? form.DesktopBounds : form.RestoreBounds;
            isMaximized = form.WindowState == FormWindowState.Maximized;
        }

        public void Restore(Form form){
            if (rect != Rectangle.Empty){
                form.DesktopBounds = rect;
                form.WindowState = isMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            if (rect == Rectangle.Empty || !Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(form.Bounds))){
                form.DesktopBounds = Screen.PrimaryScreen.WorkingArea;
                form.WindowState = FormWindowState.Maximized;
                Save(form);
            }
        }
    }
}
