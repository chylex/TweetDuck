using System.Windows.Forms;
using TweetDuck.Core.Utils;

namespace TweetDuck.Plugins.Controls{
    sealed class PluginListFlowLayout : FlowLayoutPanel{
        public PluginListFlowLayout(){
            FlowDirection = FlowDirection.TopDown;
            WrapContents = false;
        }

        protected override void WndProc(ref Message m){
            NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_HORZ, false); // basically fuck the horizontal scrollbar very much
            base.WndProc(ref m);
        }
    }
}
