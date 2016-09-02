using System.Windows.Forms;
using TweetDck.Core.Utils;

namespace TweetDck.Plugins.Controls{
    sealed partial class PluginListFlowLayout : FlowLayoutPanel{
        protected override void WndProc(ref Message m){
            NativeMethods.ShowScrollBar(Handle, NativeMethods.SB_HORZ, false); // basically fuck the horizontal scrollbar very much
            base.WndProc(ref m);
        }
    }
}
