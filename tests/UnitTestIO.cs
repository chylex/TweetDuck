using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests{
    [TestClass]
    public class UnitTestIO{
        private static readonly HashSet<string> CreatedFolders = new HashSet<string>();

        [TestInitialize]
        public void InitTest(){
            string folder = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, GetType().Name);
            CreatedFolders.Add(folder);
            Directory.CreateDirectory(folder);
            Directory.SetCurrentDirectory(folder);
        }
        
        [AssemblyCleanup]
        public static void DeleteFilesOnExit(){
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

            foreach(string folder in CreatedFolders){
                try{
                    Directory.Delete(folder, true);
                }catch(Exception){
                    // ignore
                }
            }
        }
    }
}
