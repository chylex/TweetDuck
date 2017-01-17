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

        public static FileStream WriteFile(string file){
            DeleteFileOnExit(file);
            return new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None);
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

        public static FileStream ReadFile(string file){
            return new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
        }

        public static void DeleteFileOnExit(string file){
            CreatedFiles.Add(file);
        }
        
        [TestClass]
        public static class Cleanup{
            [AssemblyCleanup]
            public static void DeleteFilesOnExit(){
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
}
