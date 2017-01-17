using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests{
    public static class TestUtils{
        private static readonly HashSet<string> CreatedFiles = new HashSet<string>();

        public static void WriteText(string file, string text){
            DeleteFileOnExit(file);
            File.WriteAllText(file, text, Encoding.UTF8);
        }

        public static void WriteLines(string file, IEnumerable<string> lines){
            DeleteFileOnExit(file);
            File.WriteAllLines(file, lines, Encoding.UTF8);
        }

        public static string ReadText(string file){
            try{
                return File.ReadAllText(file, Encoding.UTF8);
            }catch(Exception){
                return string.Empty;
            }
        }

        public static IEnumerable<string> ReadLines(string file){
            try{
                return File.ReadLines(file, Encoding.UTF8);
            }catch(Exception){
                return Enumerable.Empty<string>();
            }
        }

        public static void DeleteFileOnExit(string file){
            CreatedFiles.Add(file);
        }
        
        [AssemblyCleanup]
        public static void RunExitCleanup(){
            foreach(string file in CreatedFiles){
                try{
                    File.Delete(file);
                }catch(Exception){
                    // ignore
                }
            }
        }
    }
}
