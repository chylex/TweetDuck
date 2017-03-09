using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace TweetDck.Configuration{
    sealed class LockManager{
        public enum Result{
            Success, HasProcess, Fail
        }

        public Process LockingProcess { get; private set; }

        private readonly string file;
        private FileStream lockStream;

        public LockManager(string file){
            this.file = file;
        }

        private void CreateLockFileStream(){
            lockStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Read);
            WriteIntToStream(lockStream, GetCurrentProcessId());
            lockStream.Flush(true);
        }

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
            if (lockStream != null){
                throw new InvalidOperationException("Lock file already exists.");
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

        public Result Lock(){
            if (lockStream != null){
                return Result.Success;
            }

            Result initialResult = TryCreateLockFile();

            if (initialResult == Result.HasProcess){
                try{
                    int pid;

                    using(FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)){
                        pid = ReadIntFromStream(fileStream);
                    }

                    try{
                        Process foundProcess = Process.GetProcessById(pid);

                        using(Process currentProcess = Process.GetCurrentProcess()){
                            if (foundProcess.MainModule.FileVersionInfo.InternalName == currentProcess.MainModule.FileVersionInfo.InternalName){
                                LockingProcess = foundProcess;
                            }
                            else{
                                foundProcess.Close();
                            }
                        }
                    }catch{
                        // GetProcessById throws ArgumentException if the process is missing
                        // Process.MainModule can throw exceptions in some cases
                    }

                    return LockingProcess == null ? Result.Fail : Result.HasProcess;
                }catch{
                    return Result.Fail;
                }
            }

            return initialResult;
        }

        public bool Unlock(){
            bool result = true;

            if (ReleaseLockFileStream()){
                try{
                    File.Delete(file);
                }catch(Exception e){
                    Program.Reporter.Log(e.ToString());
                    result = false;
                }
            }

            return result;
        }

        public bool CloseLockingProcess(int closeTimeout, int killTimeout){
            if (LockingProcess != null){
                try{
                    if (LockingProcess.CloseMainWindow()){
                        for(int waited = 0; waited < closeTimeout && !LockingProcess.HasExited; waited += 250){
                            LockingProcess.Refresh();
                            Thread.Sleep(250);
                        }
                    }

                    if (!LockingProcess.HasExited){
                        LockingProcess.Kill();

                        for(int waited = 0; waited < killTimeout && !LockingProcess.HasExited; waited += 250){
                            LockingProcess.Refresh();
                            Thread.Sleep(250);
                        }
                    }

                    if (LockingProcess.HasExited){
                        LockingProcess.Dispose();
                        LockingProcess = null;
                        return true;
                    }
                }catch(Exception ex){
                    if (ex is InvalidOperationException || ex is Win32Exception){
                        if (LockingProcess != null){
                            LockingProcess.Refresh();

                            bool hasExited = LockingProcess.HasExited;
                            LockingProcess.Dispose();
                            return hasExited;
                        }
                    }
                    else throw;
                }
            }

            return false;
        }

        // Utility functions

        private static void WriteIntToStream(Stream stream, int value){
            byte[] id = BitConverter.GetBytes(value);
            stream.Write(id, 0, id.Length);
        }

        private static int ReadIntFromStream(Stream stream){
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        private static int GetCurrentProcessId(){
            using(Process process = Process.GetCurrentProcess()){
                return process.Id;
            }
        }
    }
}
