using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace TweetDuck.Video{
    static class Program{
        internal const string Version = "1.2.1.0";

        // referenced in VideoPlayer
        // set by task manager -- public const int CODE_PROCESS_KILLED = 1;
        public const int CODE_INVALID_ARGS = 2;
        public const int CODE_LAUNCH_FAIL = 3;
        public const int CODE_MEDIA_ERROR = 4;
        public const int CODE_OWNER_GONE = 5;
        public const int CODE_USER_REQUESTED = 6;

        [STAThread]
        private static int Main(string[] args){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            IntPtr ownerHandle;
            int defaultVolume;
            string videoUrl;
            string pipeToken;

            try{
                ownerHandle = new IntPtr(int.Parse(args[0], NumberStyles.Integer));
                defaultVolume = int.Parse(args[1], NumberStyles.Integer);
                videoUrl = new Uri(args[2], UriKind.Absolute).AbsoluteUri;
                pipeToken = args[3];
            }catch{
                return CODE_INVALID_ARGS;
            }

            try{
                Application.Run(new FormPlayer(ownerHandle, defaultVolume, videoUrl, pipeToken));
            }catch(Exception e){
                Console.Out.WriteLine(e.Message);
                return CODE_LAUNCH_FAIL;
            }

            return 0;
        }
    }
}
