using System.Diagnostics;

namespace TweetLib.Core.Application{
    public interface IAppLockHandler{
        bool RestoreProcess(Process process);
        bool CloseProcess(Process process);
    }
}
