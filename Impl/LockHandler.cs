using System;
using System.ComponentModel;
using System.Diagnostics;
using TweetDuck.Core.Utils;
using TweetLib.Core.Application;

namespace TweetDuck.Impl{
    class LockHandler : IAppLockHandler{
        private const int WaitRetryDelay = 250;
        private const int RestoreFailTimeout = 2000;
        private const int CloseNaturallyTimeout = 10000;
        private const int CloseKillTimeout = 5000;

        bool IAppLockHandler.RestoreProcess(Process process){
            if (process.MainWindowHandle == IntPtr.Zero){ // restore if the original process is in tray
                NativeMethods.BroadcastMessage(Program.WindowRestoreMessage, (uint)process.Id, 0);

                if (WindowsUtils.TrySleepUntil(() => CheckProcessExited(process) || (process.MainWindowHandle != IntPtr.Zero && process.Responding), RestoreFailTimeout, WaitRetryDelay)){
                    return true;
                }
            }

            return false;
        }

        bool IAppLockHandler.CloseProcess(Process process){
            try{
                if (process.CloseMainWindow()){
                    // ReSharper disable once AccessToDisposedClosure
                    WindowsUtils.TrySleepUntil(() => CheckProcessExited(process), CloseNaturallyTimeout, WaitRetryDelay);
                }

                if (!process.HasExited){
                    process.Kill();
                    // ReSharper disable once AccessToDisposedClosure
                    WindowsUtils.TrySleepUntil(() => CheckProcessExited(process), CloseKillTimeout, WaitRetryDelay);
                }

                if (process.HasExited){
                    process.Dispose();
                    return true;
                }
                else{
                    return false;
                }
            }catch(Exception ex) when (ex is InvalidOperationException || ex is Win32Exception){
                bool hasExited = CheckProcessExited(process);
                process.Dispose();
                return hasExited;
            }
        }

        private static bool CheckProcessExited(Process process){
            process.Refresh();
            return process.HasExited;
        }
    }
}
