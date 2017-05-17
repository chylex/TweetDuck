using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Controls;

namespace TweetDuck.Core.Utils{
    [Serializable]
    class WindowState{
        private Rectangle rect;
        private bool isMaximized;

        public void Save(Form form){
            rect = form.WindowState == FormWindowState.Normal ? form.DesktopBounds : form.RestoreBounds;
            isMaximized = form.WindowState == FormWindowState.Maximized;
        }

        public void Restore(Form form, bool firstTimeFullscreen){
            if (rect != Rectangle.Empty){
                form.DesktopBounds = rect;
                form.WindowState = isMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            if ((rect == Rectangle.Empty && firstTimeFullscreen) || form.IsFullyOutsideView()){
                form.DesktopBounds = Screen.PrimaryScreen.WorkingArea;
                form.WindowState = FormWindowState.Maximized;
                Save(form);
            }
        }
    }
}
