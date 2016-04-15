using System;
using System.Diagnostics;
using System.Linq;

namespace TweetDck.Migration.Helpers{
    static class ProgramProcessSearch{
        public static Process FindProcessWithWindowByName(string name){
            try{
                return Process.GetProcessesByName(name).FirstOrDefault(process => process.MainWindowHandle != IntPtr.Zero);
            }catch(Exception){
                return null;
            }
        }
    }
}
