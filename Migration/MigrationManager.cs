using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweetDick.Core;

namespace TweetDick.Migration{
    static class MigrationManager{
        private static readonly string TweetDeckPathParent = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"twitter");
        private static readonly string TweetDeckPath = Path.Combine(TweetDeckPathParent,"TweetDeck");

        public static void Run(){
            if (!Program.UserConfig.IgnoreMigration && Directory.Exists(TweetDeckPath)){
                FormMigrationQuestion formQuestion = new FormMigrationQuestion();
                MigrationDecision decision = formQuestion.ShowDialog() == DialogResult.OK ? formQuestion.Decision : MigrationDecision.AskLater;

                switch(decision){
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
            if (decision != MigrationDecision.Migrate && decision != MigrationDecision.Copy){
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

                if (decision == MigrationDecision.Migrate){
                    Directory.Delete(TweetDeckPath,true);

                    try{
                        Directory.Delete(TweetDeckPathParent,false);
                    }catch(IOException){
                        // most likely not empty, ignore
                    }
                }
            });

            task.ContinueWith(originalTask => {
                onFinished(originalTask.Exception);
            },TaskContinuationOptions.ExecuteSynchronously);
            task.Start();

            return true;
        }

        private static void CopyFile(string relativePath){
            File.Copy(Path.Combine(TweetDeckPath,relativePath),Path.Combine(Program.StoragePath,relativePath),true);
        }
    }
}
