using System;
using System.IO;
using Shell32;

namespace TweetDck.Migration.Helpers{
    sealed class LnkEditor{
        private readonly ShellLinkObject obj;

        public LnkEditor(string file){
            try{
                Shell shell = new Shell();
                Folder folder = shell.NameSpace(Path.GetDirectoryName(file));
                FolderItem item = folder.Items().Item(Path.GetFileName(file));

                obj = item.GetLink as ShellLinkObject;
            }catch(Exception){
                obj = null;
            }
        }

        public void SetComment(string newComment){
            if (obj == null)return;
            obj.Description = newComment;
        }

        public void SetPath(string newPath){
            if (obj == null)return;
            obj.Path = newPath;
            obj.SetIconLocation(newPath, 0);
        }

        public void SetWorkingDirectory(string newWorkingDirectory){
            if (obj == null)return;
            obj.WorkingDirectory = newWorkingDirectory;
        }

        public void Save(){
            if (obj == null)return;
            obj.Save();
        }
    }
}
