using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDick.Core.Other;
using TweetDick.Migration.Helpers;

namespace TweetDick.Migration{
    static class MigrationManager{
        private static readonly string TweetDeckPathParent = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"twitter");
        private static readonly string TweetDeckPath = Path.Combine(TweetDeckPathParent,"TweetDeck");

        public static void Run(){
            if (!Program.UserConfig.IgnoreMigration && Directory.Exists(TweetDeckPath)){
                FormMigrationQuestion formQuestion = new FormMigrationQuestion();
                MigrationDecision decision = formQuestion.ShowDialog() == DialogResult.OK ? formQuestion.Decision : MigrationDecision.AskLater;

                switch(decision){
                    case MigrationDecision.MigratePurge:
                    case MigrationDecision.Migrate:
                    case MigrationDecision.Copy:
                        FormBackgroundWork formWait = new FormBackgroundWork();

                        formWait.ShowWorkDialog(() => {
                            if (!BeginMigration(decision,ex => formWait.Invoke(new Action(() => {
                                if (ex != null){
                                    MessageBox.Show(ex.Message); // TODO
                                }

                                formWait.Close();

                                Program.UserConfig.IgnoreMigration = true;
                                Program.UserConfig.Save();
                            })))){
                                formWait.Close();
                            }
                        });

                        break;

                    case MigrationDecision.Ignore:
                        Program.UserConfig.IgnoreMigration = true;
                        Program.UserConfig.Save();
                        break;
                }
            }
        }

        private static bool BeginMigration(MigrationDecision decision, Action<Exception> onFinished){
            if (decision != MigrationDecision.MigratePurge && decision != MigrationDecision.Migrate && decision != MigrationDecision.Copy){
                return false;
            }

            Task task = new Task(() => {
                Directory.CreateDirectory(Program.StoragePath);
                Directory.CreateDirectory(Path.Combine(Program.StoragePath,"localStorage"));
                Directory.CreateDirectory(Path.Combine(Program.StoragePath,"Local Storage"));

                CopyFile("Cookies");
                CopyFile("Cookies-journal");
                CopyFile("localStorage"+Path.DirectorySeparatorChar+"qrc__0.localstorage");
                CopyFile("Local Storage"+Path.DirectorySeparatorChar+"https_tweetdeck.twitter.com_0.localstorage");
                CopyFile("Local Storage"+Path.DirectorySeparatorChar+"https_tweetdeck.twitter.com_0.localstorage-journal");

                if (decision == MigrationDecision.Migrate || decision == MigrationDecision.MigratePurge){
                    Directory.Delete(TweetDeckPath,true);

                    try{
                        Directory.Delete(TweetDeckPathParent,false);
                    }catch(IOException){
                        // most likely not empty, ignore
                    }
                }

                if (decision == MigrationDecision.MigratePurge){
                    // kill process if running
                    Process runningProcess = ProgramProcessSearch.FindProcessWithWindowByName("TweetDeck");

                    if (runningProcess != null){
                        runningProcess.CloseMainWindow();

                        for(int wait = 0; wait < 100 && !runningProcess.HasExited; wait++){ // 10 seconds
                            runningProcess.Refresh();
                            Thread.Sleep(100);
                        }
                            
                        runningProcess.Close();
                    }

                    // update the lnk files wherever possible (desktop icons, pinned taskbar)
                    string[] locations = {
                        Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory),
                        Environment.ExpandEnvironmentVariables(@"%APPDATA%\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar")
                    };

                    foreach(string location in locations){
                        string linkFile = Path.Combine(location,"TweetDeck.lnk");

                        if (File.Exists(linkFile)){
                            LnkEditor lnk = new LnkEditor(linkFile);
                            lnk.SetPath(Application.ExecutablePath);
                            lnk.SetWorkingDirectory(Environment.CurrentDirectory);
                            lnk.SetComment("TweetDick"); // TODO add a tagline
                            lnk.Save();

                            File.Move(linkFile,Path.Combine(location,"TweetDick.lnk"));
                        }
                    }

                    // uninstall in the background
                    string guid = ProgramRegistrySearch.FindByDisplayName("TweetDeck");

                    if (guid != null){
                        Process uninstaller = Process.Start("msiexec.exe","/x"+guid+" /quiet /qn");

                        if (uninstaller != null){
                            uninstaller.WaitForExit();
                        }
                    }

                    // migration finished like a boss
                }
            });

            task.ContinueWith(originalTask => onFinished(originalTask.Exception),TaskContinuationOptions.ExecuteSynchronously);
            task.Start();

            return true;
        }

        private static void CopyFile(string relativePath){
            File.Copy(Path.Combine(TweetDeckPath,relativePath),Path.Combine(Program.StoragePath,relativePath),true);
        }
    }
}
