using System;
using System.Diagnostics;

namespace TweetLib.Core.Systems.Startup{
    public class LockResult{
        private readonly string name;

        private LockResult(string name){
            this.name = name;
        }

        public override string ToString(){
            return name;
        }

        public static LockResult Success { get; } = new LockResult("Success");

        public sealed class Fail : LockResult{
            public Exception Exception { get; }

            public Fail(Exception exception) : base("Fail"){
                this.Exception = exception;
            }
        }

        public sealed class HasProcess : LockResult, IDisposable{
            public Process Process { get; }

            public HasProcess(Process process) : base("HasProcess"){
                this.Process = process;
            }

            public void Dispose(){
                Process.Dispose();
            }
        }
    }
}
