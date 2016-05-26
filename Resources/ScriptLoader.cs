using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace TweetDck.Resources{
    static class ScriptLoader{
        public static string LoadResource(string name){
            try{
                return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,name),Encoding.UTF8);
            }catch(Exception ex){
                MessageBox.Show("Unfortunately, "+Program.BrandName+" could not load the "+name+" file. The program will continue running with limited functionality.\r\n\r\n"+ex.Message,Program.BrandName+" Has Failed :(",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return null;
            }
        }

        public static IEnumerable<string> LoadResources(params string[] names){
            return names.Select(LoadResource);
        }
    }
}
