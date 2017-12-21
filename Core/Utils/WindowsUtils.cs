﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace TweetDuck.Core.Utils{
    static class WindowsUtils{
        private static readonly Lazy<Regex> RegexStripHtmlStyles = new Lazy<Regex>(() => new Regex(@"\s?(?:style|class)="".*?"""), false);
        private static readonly Lazy<Regex> RegexOffsetClipboardHtml = new Lazy<Regex>(() => new Regex(@"(?<=EndHTML:|EndFragment:)(\d+)"), false);

        public static int CurrentProcessID { get; }
        public static bool ShouldAvoidToolWindow { get; }

        static WindowsUtils(){
            using(Process me = Process.GetCurrentProcess()){
                CurrentProcessID = me.Id;
            }

            Version ver = Environment.OSVersion.Version;
            ShouldAvoidToolWindow = ver.Major == 6 && ver.Minor == 2; // windows 8/10
        }

        public static void CreateDirectoryForFile(string file){
            string dir = Path.GetDirectoryName(file);

            if (dir == null){
                throw new ArgumentException("Invalid file path: "+file);
            }
            else if (dir.Length > 0){
                Directory.CreateDirectory(dir);
            }
        }

        public static bool CheckFolderWritePermission(string path){
            string testFile = Path.Combine(path, ".test");

            try{
                Directory.CreateDirectory(path);

                using(File.Create(testFile)){}
                File.Delete(testFile);
                return true;
            }catch{
                return false;
            }
        }

        public static bool OpenAssociatedProgram(string file, string arguments = "", bool runElevated = false){
            try{
                using(Process.Start(new ProcessStartInfo{
                    FileName = file,
                    Arguments = arguments,
                    Verb = runElevated ? "runas" : string.Empty,
                    ErrorDialog = true
                })){
                    return true;
                }
            }catch(Win32Exception e) when (e.NativeErrorCode == 0x000004C7){ // operation canceled by the user
                return false;
            }catch(Exception e){
                Program.Reporter.HandleException("Error opening file", e.Message, true, e);
                return false;
            }
        }

        public static bool TrySleepUntil(Func<bool> test, int timeoutMillis, int timeStepMillis){
            for(int waited = 0; waited < timeoutMillis; waited += timeStepMillis){
                if (test()){
                    return true;
                }

                Thread.Sleep(timeStepMillis);
            }

            return false;
        }

        public static void TryDeleteFolderWhenAble(string path, int timeout){
            new Thread(() => {
                TrySleepUntil(() => {
                    try{
                        Directory.Delete(path, true);
                        return true;
                    }catch(DirectoryNotFoundException){
                        return true;
                    }catch{
                        return false;
                    }
                }, timeout, 500);
            }).Start();
        }

        public static void ClipboardStripHtmlStyles(){
            if (!Clipboard.ContainsText(TextDataFormat.Html) || !Clipboard.ContainsText(TextDataFormat.UnicodeText)){
                return;
            }

            string originalText = Clipboard.GetText(TextDataFormat.UnicodeText);
            string originalHtml = Clipboard.GetText(TextDataFormat.Html);

            string updatedHtml = RegexStripHtmlStyles.Value.Replace(originalHtml, string.Empty);

            int removed = originalHtml.Length-updatedHtml.Length;
            updatedHtml = RegexOffsetClipboardHtml.Value.Replace(updatedHtml, match => (int.Parse(match.Value)-removed).ToString().PadLeft(match.Value.Length, '0'));
            
            DataObject obj = new DataObject();
            obj.SetText(originalText, TextDataFormat.UnicodeText);
            obj.SetText(updatedHtml, TextDataFormat.Html);
            SetClipboardData(obj);
        }

        public static void SetClipboard(string text, TextDataFormat format){
            if (string.IsNullOrEmpty(text)){
                return;
            }

            DataObject obj = new DataObject();
            obj.SetText(text, format);
            SetClipboardData(obj);
        }

        private static void SetClipboardData(DataObject obj){
            try{
                Clipboard.SetDataObject(obj);
            }catch(ExternalException e){
                Program.Reporter.HandleException("Clipboard Error", "TweetDuck could not access the clipboard as it is currently used by another process.", true, e);
            }
        }
    }
}
