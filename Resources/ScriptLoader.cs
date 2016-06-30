using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using CefSharp;
using CefSharp.WinForms;

namespace TweetDck.Resources{
    static class ScriptLoader{
        private const string UrlPrefix = "td:";

        public static string LoadResource(string name){
            try{
                return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,name),Encoding.UTF8);
            }catch(Exception ex){
                MessageBox.Show("Unfortunately, "+Program.BrandName+" could not load the "+name+" file. The program will continue running with limited functionality.\r\n\r\n"+ex.Message,Program.BrandName+" Has Failed :(",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return null;
            }
        }

        public static void ExecuteFile(ChromiumWebBrowser browser, string file, string identifier){
            ExecuteScript(browser,LoadResource(file),identifier);
        }

        public static void ExecuteFile(IFrame frame, string file, string identifier){
            ExecuteScript(frame,LoadResource(file),identifier);
        }

        public static void ExecuteScript(ChromiumWebBrowser browser, string script, string identifier){
            if (script == null)return;

            using(IFrame frame = browser.GetMainFrame()){
                frame.ExecuteJavaScriptAsync(script,UrlPrefix+identifier);
            }
        }

        public static void ExecuteScript(IFrame frame, string script, string identifier){
            if (script == null)return;

            frame.ExecuteJavaScriptAsync(script,UrlPrefix+identifier);
        }
    }
}
