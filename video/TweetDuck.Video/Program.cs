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

        private static uint? message;
        public static uint VideoPlayerMessage => message ?? (message = NativeMethods.RegisterWindowMessage("TweetDuckVideoPlayer")).Value;

        [STAThread]
        private static int Main(string[] args){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IntPtr ownerHandle;
            int defaultVolume;
            string videoUrl;

            try{
                ownerHandle = new IntPtr(int.Parse(args[0], NumberStyles.Integer, CultureInfo.InvariantCulture));
                defaultVolume = int.Parse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
                videoUrl = new Uri(args[2], UriKind.Absolute).AbsoluteUri;
            }catch{
                return CODE_INVALID_ARGS;
            }

            try{
                Application.Run(new FormPlayer(ownerHandle, defaultVolume, videoUrl));
            }catch(Exception e){
                Console.Out.WriteLine(e.Message);
                return CODE_LAUNCH_FAIL;
            }

            return 0;
        }
    }
}
