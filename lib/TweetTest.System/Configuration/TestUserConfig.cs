using System;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Configuration;
using TweetDuck.Core.Other;

namespace TweetTest.Configuration{
    [TestClass]
    public class TestUserConfig : TestIO{
        private static void WriteTestConfig(string file, bool withBackup){
            UserConfig cfg = UserConfig.Load(file);
            cfg.ZoomLevel = 123;
            cfg.Save();

            if (withBackup){
                cfg.Save();
            }
        }

        [Pure] // used to display a warning when not using the return value
        private static bool CheckTestConfig(string file){
            return UserConfig.Load(file).ZoomLevel == 123;
        }

        [TestMethod]
        public void TestMissing(){
            Assert.IsNotNull(UserConfig.Load("missing"));
            Assert.IsFalse(File.Exists("missing"));
        }

        [TestMethod]
        public void TestBasic(){
            Assert.IsFalse(CheckTestConfig("basic"));
            WriteTestConfig("basic", false);
            Assert.IsTrue(CheckTestConfig("basic"));
        }

        [TestMethod]
        public void TestBackupName(){
            Assert.AreEqual("name.bak", UserConfig.GetBackupFile("name"));
            Assert.AreEqual("name.cfg.bak", UserConfig.GetBackupFile("name.cfg"));
            Assert.AreEqual("name.bak.bak", UserConfig.GetBackupFile("name.bak"));
        }

        [TestMethod]
        public void TestBackupCreate(){
            WriteTestConfig("nobackup", false);
            Assert.IsTrue(File.Exists("nobackup"));
            Assert.IsFalse(File.Exists(UserConfig.GetBackupFile("nobackup")));

            WriteTestConfig("withbackup", true);
            Assert.IsTrue(File.Exists("withbackup"));
            Assert.IsTrue(File.Exists(UserConfig.GetBackupFile("withbackup")));
        }

        [TestMethod]
        public void TestBackupRestore(){
            WriteTestConfig("gone", true);
            Assert.IsTrue(File.Exists("gone"));
            Assert.IsTrue(File.Exists(UserConfig.GetBackupFile("gone")));
            File.Delete("gone");
            Assert.IsTrue(CheckTestConfig("gone"));
            
            WriteTestConfig("corrupted", true);
            Assert.IsTrue(File.Exists("corrupted"));
            Assert.IsTrue(File.Exists(UserConfig.GetBackupFile("corrupted")));
            File.WriteAllText("corrupted", "oh no corrupt");
            Assert.IsTrue(CheckTestConfig("corrupted"));
        }

        [TestMethod]
        public void TestReload(){
            UserConfig cfg = UserConfig.Load("reloaded");
            cfg.ZoomLevel = 123;
            cfg.Save();

            cfg.ZoomLevel = 200;
            cfg.Reload();
            Assert.AreEqual(123, cfg.ZoomLevel);
        }

        [TestMethod]
        public void TestReset(){
            UserConfig cfg = UserConfig.Load("reset");
            cfg.ZoomLevel = 123;
            cfg.Save();
            
            File.Delete("reset");
            cfg.Reload();
            Assert.AreEqual(100, cfg.ZoomLevel);
            Assert.IsTrue(File.Exists("reset"));
        }

        [TestMethod]
        public void TestEventsNoTrigger(){
            void Fail(object sender, EventArgs args) => Assert.Fail();
            
            UserConfig cfg = UserConfig.Load("events");
            cfg.MuteNotifications = true;
            cfg.TrayBehavior = TrayIcon.Behavior.Combined;
            cfg.ZoomLevel = 99;

            cfg.MuteToggled += Fail;
            cfg.TrayBehaviorChanged += Fail;
            cfg.ZoomLevelChanged += Fail;

            cfg.MuteNotifications = true;
            cfg.TrayBehavior = TrayIcon.Behavior.Combined;
            cfg.ZoomLevel = 99;
        }

        [TestMethod]
        public void TestEventsTrigger(){
            int triggers = 0;
            void Trigger(object sender, EventArgs args) => ++triggers;
            
            UserConfig cfg = UserConfig.Load("events");
            cfg.MuteNotifications = false;
            cfg.TrayBehavior = TrayIcon.Behavior.Disabled;
            cfg.ZoomLevel = 100;

            cfg.MuteToggled += Trigger;
            cfg.TrayBehaviorChanged += Trigger;
            cfg.ZoomLevelChanged += Trigger;

            cfg.MuteNotifications = true;
            cfg.TrayBehavior = TrayIcon.Behavior.Combined;
            cfg.ZoomLevel = 99;

            cfg.MuteNotifications = false;
            cfg.TrayBehavior = TrayIcon.Behavior.Disabled;
            cfg.ZoomLevel = 100;

            Assert.AreEqual(6, triggers);
        }
    }
}
