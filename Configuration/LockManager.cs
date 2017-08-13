using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TweetDuck.Core.Utils;
using TweetLib.Communication;

namespace TweetDuck.Configuration{
    sealed class LockManager{
        private const int RetryDelay = 250;

        public enum Result{
            Success, HasProcess, Fail
        }

        private readonly string file;
        private FileStream lockStream;
        private Process lockingProcess;

        public LockManager(string file){
            this.file = file;
        }

        // Lock file

        private bool ReleaseLockFileStream(){
            if (lockStream != null){
                lockStream.Dispose();
                lockStream = null;
                return true;
            }
            else{
                return false;
            }
        }

        private Result TryCreateLockFile(){
            void CreateLockFileStream(){
                lockStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
                lockStream.Write(BitConverter.GetBytes(WindowsUtils.CurrentProcessID), 0, sizeof(int));
                lockStream.Flush(true);
            }

            try{
                CreateLockFileStream();
                return Result.Success;
            }catch(DirectoryNotFoundException){
                try{
                    CreateLockFileStream();
                    return Result.Success;
                }catch{
                    ReleaseLockFileStream();
                    return Result.Fail;
                }
            }catch(IOException){
                return Result.HasProcess;
            }catch{
                ReleaseLockFileStream();
                return Result.Fail;
            }
        }

        // Lock management

        public Result Lock(){
            if (lockStream != null){
                return Result.Success;
            }

            Result initialResult = TryCreateLockFile();

            if (initialResult == Result.HasProcess){
                try{
                    int pid;

                    using(FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)){
                        byte[] bytes = new byte[sizeof(int)];
                        fileStream.Read(bytes, 0, bytes.Length);
                        pid = BitConverter.ToInt32(bytes, 0);
                    }

                    try{
                        Process foundProcess = Process.GetProcessById(pid);

                        using(Process currentProcess = Process.GetCurrentProcess()){
                            if (foundProcess.MainModule.FileVersionInfo.InternalName == currentProcess.MainModule.FileVersionInfo.InternalName){
                                lockingProcess = foundProcess;
                            }
                            else{
                                foundProcess.Close();
                            }
                        }
                    }catch{
                        // GetProcessById throws ArgumentException if the process is missing
                        // Process.MainModule can throw exceptions in some cases
                    }

                    return lockingProcess == null ? Result.Fail : Result.HasProcess;
                }catch{
                    return Result.Fail;
                }
            }

            return initialResult;
        }

        public Result LockWait(int timeout){
            for(int elapsed = 0; elapsed < timeout; elapsed += RetryDelay){
                Result result = Lock();

                if (result == Result.HasProcess){
                    Thread.Sleep(RetryDelay);
                }
                else{
                    return result;
                }
            }

            return Lock();
        }

        public bool Unlock(){
            if (ReleaseLockFileStream()){
                try{
                    File.Delete(file);
                }catch(Exception e){
                    Program.Reporter.Log(e.ToString());
                    return false;
                }
            }

            return true;
        }

        // Locking process

        public bool RestoreLockingProcess(int failTimeout){
            if (lockingProcess != null){
                if (lockingProcess.MainWindowHandle == IntPtr.Zero){ // restore if the original process is in tray
                    Comms.BroadcastMessage(Program.WindowRestoreMessage, (uint)lockingProcess.Id, 0);

                    if (WindowsUtils.TrySleepUntil(() => CheckLockingProcessExited() || (lockingProcess.MainWindowHandle != IntPtr.Zero && lockingProcess.Responding), failTimeout, RetryDelay)){
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CloseLockingProcess(int closeTimeout, int killTimeout){
            if (lockingProcess != null){
                try{
                    if (lockingProcess.CloseMainWindow()){
                        WindowsUtils.TrySleepUntil(CheckLockingProcessExited, closeTimeout, RetryDelay);
                    }

                    if (!lockingProcess.HasExited){
                        lockingProcess.Kill();
                        WindowsUtils.TrySleepUntil(CheckLockingProcessExited, killTimeout, RetryDelay);
                    }

                    if (lockingProcess.HasExited){
                        lockingProcess.Dispose();
                        lockingProcess = null;
                        return true;
                    }
                }catch(Exception ex){
                    if (ex is InvalidOperationException || ex is Win32Exception){
                        if (lockingProcess != null){
                            bool hasExited = CheckLockingProcessExited();
                            lockingProcess.Dispose();
                            return hasExited;
                        }
                    }
                    else throw;
                }
            }

            return false;
        }

        private bool CheckLockingProcessExited(){
            lockingProcess.Refresh();
            return lockingProcess.HasExited;
        }
    }
}
