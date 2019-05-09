using System;
using System.IO;

namespace TweetLib.Core.Utils{
    public static class FileUtils{
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

        public static bool FileExistsAndNotEmpty(string path){
            try{
                return new FileInfo(path).Length > 0;
            }catch{
                return false;
            }
        }
    }
}
