using System;
using System.Globalization;
using System.Windows.Forms;

namespace TweetDuck.Video{
    static class Program{
        // referenced in VideoPlayer
        public const int CODE_INVALID_ARGS = 1;
        public const int CODE_LAUNCH_FAIL = 2;
        public const int CODE_MEDIA_ERROR = 3;
        public const int CODE_OWNER_GONE = 4;

        [STAThread]
        private static int Main(string[] args){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IntPtr ownerHandle;
            string videoUrl;

            try{
                ownerHandle = new IntPtr(int.Parse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture));
                videoUrl = new Uri(args[1], UriKind.Absolute).AbsoluteUri;
            }catch{
                return CODE_INVALID_ARGS;
            }

            try{
                Application.Run(new FormPlayer(ownerHandle, videoUrl));
            }catch(Exception e){
                // TODO
                Console.Out.WriteLine(e.Message);
                return CODE_LAUNCH_FAIL;
            }

            return 0;
        }
    }
}
