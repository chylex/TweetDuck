using System;
using System.Runtime.InteropServices;

namespace TweetDuck.Browser{
    static class NativeMethods{
        public static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
        
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string messageName);
    }
}
