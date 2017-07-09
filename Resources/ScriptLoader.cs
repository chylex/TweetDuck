using CefSharp;
using CefSharp.WinForms;
using System;
using System.IO;
using System.Text;
using TweetDuck.Core.Other;

namespace TweetDuck.Resources{
    static class ScriptLoader{
        private const string UrlPrefix = "td:";

        public static string LoadResource(string name, bool silent = false){
            try{
                return File.ReadAllText(Path.Combine(Program.ScriptPath, name), Encoding.UTF8);
            }catch(Exception ex){
                if (!silent){
                    FormMessage.Error(Program.BrandName+" Has Failed :(", "Unfortunately, "+Program.BrandName+" could not load the "+name+" file. The program will continue running with limited functionality.\n\n"+ex.Message, FormMessage.OK);
                }

                return null;
            }
        }

        public static void ExecuteFile(ChromiumWebBrowser browser, string file){
            ExecuteScript(browser, LoadResource(file), GetRootIdentifier(file));
        }

        public static void ExecuteFile(IFrame frame, string file){
            ExecuteScript(frame, LoadResource(file), GetRootIdentifier(file));
        }

        public static void ExecuteScript(ChromiumWebBrowser browser, string script, string identifier){
            if (script == null)return;

            using(IFrame frame = browser.GetMainFrame()){
                frame.ExecuteJavaScriptAsync(script, UrlPrefix+identifier, 1);
            }
        }

        public static void ExecuteScript(IFrame frame, string script, string identifier){
            if (script == null)return;

            frame.ExecuteJavaScriptAsync(script, UrlPrefix+identifier, 1);
        }

        public static string GetRootIdentifier(string file){
            return "root:"+Path.GetFileNameWithoutExtension(file);
        }
    }
}
