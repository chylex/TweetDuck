using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TweetDuck.Core.Utils{
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    static class NativeMethods{
        public static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);
        public static readonly IntPtr HOOK_HANDLED = new IntPtr(-1);

        public const int HWND_TOPMOST = -1;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const int WS_DISABLED = 0x08000000;
        public const int GWL_STYLE = -16;

        public const int SB_HORZ = 0;
        public const int EM_SETCUEBANNER = 0x1501;

        public const int WM_MOUSE_LL = 14;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_XBUTTONDOWN = 0x020B;
        public const int WM_XBUTTONUP = 0x020C;
        public const int WM_PARENTNOTIFY = 0x0210;
        
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO{
            public static readonly uint Size = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));

            public uint cbSize;
            public uint dwTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT{
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT{
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(int hWnd, int hWndOrder, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string messageName);

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO info);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);
        
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out bool enabled);
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public static void SetFormPos(Form form, int hWndOrder, uint flags){
            SetWindowPos(form.Handle.ToInt32(), hWndOrder, form.Left, form.Top, form.Width, form.Height, flags);
        }

        public static void SetFormDisabled(Form form, bool disabled){
            if (disabled){
                SetWindowLong(form.Handle, GWL_STYLE, GetWindowLong(form.Handle, GWL_STYLE) | WS_DISABLED);
            }
            else{
                SetWindowLong(form.Handle, GWL_STYLE, GetWindowLong(form.Handle, GWL_STYLE) & ~WS_DISABLED);
            }
        }

        public static void BroadcastMessage(uint msg, uint wParam, int lParam){
            PostMessage(HWND_BROADCAST, msg, new UIntPtr(wParam), new IntPtr(lParam));
        }

        public static int GetMouseHookData(IntPtr ptr){
            return Marshal.PtrToStructure<MSLLHOOKSTRUCT>(ptr).mouseData >> 16;
        }

        public static int GetIdleSeconds(){
            LASTINPUTINFO info = new LASTINPUTINFO{
                cbSize = LASTINPUTINFO.Size
            };

            if (!GetLastInputInfo(ref info)){
                return 0;
            }

            uint ticks;

            unchecked{
                ticks = (uint)Environment.TickCount;
            }

            int seconds = (int)Math.Floor(TimeSpan.FromMilliseconds(ticks-info.dwTime).TotalSeconds);
            return Math.Max(0, seconds); // ignore rollover after several weeks of uptime
        }

        public static void RenderSourceIntoBitmap(IntPtr source, Bitmap target){
            using(Graphics graphics = Graphics.FromImage(target)){
                IntPtr graphicsHandle = graphics.GetHdc();

                try{
                    BitBlt(graphicsHandle, 0, 0, target.Width, target.Height, source, 0, 0, 0x00CC0020);
                }finally{
                    graphics.ReleaseHdc(graphicsHandle);
                }
            }
        }
    }
}
