using System;
using System.Runtime.InteropServices;

namespace TweetDuck.Video{
    static class NativeMethods{
        private const int GWL_HWNDPARENT = -8;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT{
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static void SetWindowOwner(IntPtr child, IntPtr owner){
            SetWindowLong(child, GWL_HWNDPARENT, owner);
            /*
             * "You must not call SetWindowLong with the GWL_HWNDPARENT index to change the parent of a child window"
             * 
             * ...which I'm not sure they're saying because this is completely unsupported and causes demons to come out of sewers
             * ...or because GWL_HWNDPARENT actually changes the OWNER and is therefore NOT changing the parent of a child window
             * 
             * ...so technically, this is following the documentation to the word.
             */
        }
    }
}
