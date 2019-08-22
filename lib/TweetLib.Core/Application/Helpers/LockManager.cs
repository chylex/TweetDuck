using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace TweetLib.Core.Application.Helpers{
    public sealed class LockManager{
        private const int RetryDelay = 250;

        public enum Result{
            Success, HasProcess, Fail
        }

        private readonly string file;
        private FileStream? lockStream;
        private Process? lockingProcess;

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
                lockStream.Write(BitConverter.GetBytes(CurrentProcessID), 0, sizeof(int));
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

                        if (MatchesCurrentProcess(foundProcess)){
                            lockingProcess = foundProcess;
                        }
                        else{
                            foundProcess.Close();
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
                    App.ErrorHandler.Log(e.ToString());
                    return false;
                }
            }

            return true;
        }

        // Locking process
        
        public bool RestoreLockingProcess(){
            return lockingProcess != null && App.LockHandler.RestoreProcess(lockingProcess);
        }

        public bool CloseLockingProcess(){
            if (lockingProcess != null && App.LockHandler.CloseProcess(lockingProcess)){
                lockingProcess = null;
                return true;
            }

            return false;
        }

        // Utilities

        private static int CurrentProcessID{
            get{
                using Process me = Process.GetCurrentProcess();
                return me.Id;
            }
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static bool MatchesCurrentProcess(Process process){
            using Process current = Process.GetCurrentProcess();
            return current.MainModule.FileVersionInfo.InternalName == process.MainModule.FileVersionInfo.InternalName;
        }
    }
}
