using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TweetDck.Core.Utils{
    static class NativeMethods{
        public static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);

        public const int HWND_TOPMOST = -1;
        public const uint SWP_NOACTIVATE = 0x0010;

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        public const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;

        public const int SB_HORZ = 0;
        public const int BCM_SETSHIELD = 0x160C;

        public const int WH_MOUSE_LL = 14;
        public const int WH_MOUSEWHEEL = 0x020A;

        public enum MouseButton{
            Left, Right
        }

        private struct LASTINPUTINFO{
            public static readonly uint Size = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));

            // ReSharper disable once NotAccessedField.Local
            public uint cbSize;
            #pragma warning disable 649
            public uint dwTime;
            #pragma warning restore 649
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(int hWnd, int hWndOrder, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO info);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string messageName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        public static void SetFormPos(Form form, int hWndOrder, uint flags){
            SetWindowPos(form.Handle.ToInt32(), hWndOrder, form.Left, form.Top, form.Width, form.Height, flags);
        }

        public static void SimulateMouseClick(MouseButton button){
            int flagHold, flagRelease;

            switch(button){
                case MouseButton.Left:
                    flagHold = SystemInformation.MouseButtonsSwapped ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_LEFTDOWN;
                    flagRelease = SystemInformation.MouseButtonsSwapped ? MOUSEEVENTF_RIGHTUP : MOUSEEVENTF_LEFTUP;
                    break;

                case MouseButton.Right:
                    flagHold = SystemInformation.MouseButtonsSwapped ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;
                    flagRelease = SystemInformation.MouseButtonsSwapped ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;
                    break;

                default: return;
            }

            mouse_event(flagHold, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            mouse_event(flagRelease, Cursor.Position.X, Cursor.Position.Y, 0, 0);
        }

        public static int GetIdleSeconds(){
            LASTINPUTINFO info = new LASTINPUTINFO();
            info.cbSize = LASTINPUTINFO.Size;

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
    }
}
