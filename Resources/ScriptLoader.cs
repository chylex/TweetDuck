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
                string contents = File.ReadAllText(Path.Combine(Program.ScriptPath, name), Encoding.UTF8);
                int separator;

                // first line can be either:
                // #<version>\r\n
                // #<version>\n

                if (contents[0] != '#'){
                    ShowLoadError(silent, sync, $"File {name} appears to be corrupted, please try reinstalling the app.");
                    separator = 0;
                }
                else{
                    separator = contents.IndexOf('\n');
                    string fileVersion = contents.Substring(1, separator-1).TrimEnd();

                    if (fileVersion != Program.VersionTag){
                        ShowLoadError(silent, sync, $"File {name} is made for a different version of TweetDuck ({fileVersion}) and may not function correctly in this version, please try reinstalling the app.");
                    }
                }

                return contents.Substring(separator).TrimStart();
            }catch(Exception ex){
                ShowLoadError(silent, sync, $"Could not load {name}. The program will continue running with limited functionality.\n\n{ex.Message}");
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

        private static void ShowLoadError(bool silent, Control sync, string message){
            if (silent){
                return;
            }

            if (sync == null){
                FormMessage.Error("Resource Error", message, FormMessage.OK);
            }
            else{
                sync.InvokeSafe(() => FormMessage.Error("Resource Error", message, FormMessage.OK));
            }
        }
    }
}
