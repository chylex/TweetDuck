using System.ComponentModel;
using System.Windows.Forms;
using WMPLib;

namespace TweetDuck.Video{
    [DesignTimeVisible(true)]
    [Clsid("{6bf52a52-394a-11d3-b153-00c04f79faa6}")]
    class ControlWMP : AxHost{
        public WindowsMediaPlayer Ocx { get; private set; }

        public ControlWMP() : base("6bf52a52-394a-11d3-b153-00c04f79faa6"){}

        protected override void AttachInterfaces(){
            Ocx = (WindowsMediaPlayer)GetOcx();
        }
    }
}
