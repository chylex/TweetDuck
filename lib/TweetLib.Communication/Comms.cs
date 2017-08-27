using System;
using TweetLib.Communication.Utils;

namespace TweetLib.Communication{
    public static class Comms{
        public static void BroadcastMessage(uint msg, uint wParam, int lParam){
            NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, msg, new UIntPtr(wParam), new IntPtr(lParam));
        }

        public static uint RegisterMessage(string name){
            return NativeMethods.RegisterWindowMessage(name);
        }
    }
}
