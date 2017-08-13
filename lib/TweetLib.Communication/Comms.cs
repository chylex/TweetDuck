using System;
using TweetLib.Communication.Utils;

namespace TweetLib.Communication{
    public static class Comms{
        public static void SendMessage(IntPtr hWnd, uint msg, uint wParam, int lParam){
            NativeMethods.SendMessage(hWnd, msg, new UIntPtr(wParam), new IntPtr(lParam));
        }

        public static void SendMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam){
            NativeMethods.SendMessage(hWnd, msg, wParam, lParam);
        }

        public static void PostMessage(IntPtr hWnd, uint msg, uint wParam, int lParam){
            NativeMethods.PostMessage(hWnd, msg, new UIntPtr(wParam), new IntPtr(lParam));
        }

        public static void PostMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam){
            NativeMethods.PostMessage(hWnd, msg, wParam, lParam);
        }

        public static void BroadcastMessage(uint msg, uint wParam, int lParam){
            NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, msg, new UIntPtr(wParam), new IntPtr(lParam));
        }

        public static void BroadcastMessage(uint msg, UIntPtr wParam, IntPtr lParam){
            NativeMethods.PostMessage(NativeMethods.HWND_BROADCAST, msg, wParam, lParam);
        }

        public static uint RegisterMessage(string name){
            return NativeMethods.RegisterWindowMessage(name);
        }
    }
}
