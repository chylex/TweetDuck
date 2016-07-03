using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TweetDck.Core.Other.Settings.Export{
    sealed class ExportManager{
        public static readonly string CookiesPath = Path.Combine(Program.StoragePath,"Cookies");
        public static readonly string TempCookiesPath = Path.Combine(Program.StoragePath,"CookiesTmp");

        public Exception LastException { get; private set; }

        private readonly string file;

        public ExportManager(string file){
            this.file = file;
        }

        public bool Export(bool includeSession){
            try{
                using(CombinedFileStream stream = new CombinedFileStream(new FileStream(file,FileMode.Create,FileAccess.Write,FileShare.None))){
                    stream.WriteFile("config",Program.ConfigFilePath);

                    if (includeSession){
                        stream.WriteFile("cookies",CookiesPath);
                    }

                    stream.Flush();
                }

                return true;
            }catch(Exception e){
                LastException = e;
                return false;
            }
        }

        public bool Import(){
            try{
                using(CombinedFileStream stream = new CombinedFileStream(new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.None))){
                    CombinedFileStream.Entry entry;

                    while((entry = stream.ReadFile()) != null){
                        switch(entry.Identifier){
                            case "config":
                                entry.WriteToFile(Program.ConfigFilePath);
                                Program.ReloadConfig();
                                break;

                            case "cookies":
                                if (MessageBox.Show("Do you want to import the login session? This will restart "+Program.BrandName+".","Importing "+Program.BrandName+" Settings",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.Yes){
                                    entry.WriteToFile(Path.Combine(Program.StoragePath,TempCookiesPath));

                                    // okay to and restart, 'cookies' is always the last entry
                                    Process.Start(Application.ExecutablePath,"-restart -importcookies");
                                    Application.Exit();
                                }

                                break;
                        }
                    }
                }

                return true;
            }catch(Exception e){
                LastException = e;
                return false;
            }
        }
    }
}
