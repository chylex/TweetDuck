using System;
using System.Windows.Forms;

namespace TweetDuck.Video{
    static class Program{
        [STAThread]
        private static void Main(){
            try{
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormPlayer());
            }catch(Exception){
                // TODO
                Environment.Exit(2);
            }
        }
    }
}
