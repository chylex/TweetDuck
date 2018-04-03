using CefSharp;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;

namespace TweetDuck.Resources{
    static class ScriptLoader{
        private const string UrlPrefix = "td:";

        public static string LoadResource(string name, bool silent = false, Control sync = null){
            try{
                return File.ReadAllText(Path.Combine(Program.ScriptPath, name), Encoding.UTF8);
            }catch(Exception ex){
                if (!silent){
                    ShowLoadError(sync, "Unfortunately, TweetDuck could not load the "+name+" file. The program will continue running with limited functionality.\n\n"+ex.Message);
                }

                return null;
            }
        }

        public static bool ExecuteFile(IFrame frame, string file, Control sync = null){
            string script = LoadResource(file, sync == null, sync);
            ExecuteScript(frame, script, GetRootIdentifier(file));
            return script != null;
        }

        public static void ExecuteScript(IFrame frame, string script, string identifier){
            if (script != null){
                frame.ExecuteJavaScriptAsync(script, UrlPrefix+identifier, 1);
            }
        }

        public static string GetRootIdentifier(string file){
            return "root:"+Path.GetFileNameWithoutExtension(file);
        }

        private static void ShowLoadError(Control sync, string message){
            if (sync == null){
                FormMessage.Error("TweetDuck Has Failed :(", message, FormMessage.OK);
            }
            else{
                sync.InvokeAsyncSafe(() => FormMessage.Error("TweetDuck Has Failed :(", message, FormMessage.OK));
            }
        }
    }
}
