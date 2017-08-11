using System;
using System.Runtime.InteropServices;

namespace TweetDuck.Video{
    static class NativeMethods{
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT{
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
