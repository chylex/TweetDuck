using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TweetDck.Core.Utils;

namespace UnitTests.Core.Utils{
    [TestClass]
    public class TestCommandLineArgs{
        [TestMethod]
        public void TestEmpty(){
            CommandLineArgs args = new CommandLineArgs();

            Assert.AreEqual(0, args.Count);
            Assert.AreEqual(string.Empty, args.ToString());

            Assert.IsFalse(args.HasFlag("x"));
            Assert.IsFalse(args.HasValue("x"));
            Assert.AreEqual("default", args.GetValue("x", "default"));

            args.RemoveFlag("x");
            args.RemoveValue("x");

            var dict = new Dictionary<string, string>();
            args.ToDictionary(dict);
            Assert.AreEqual(0, dict.Count);
        }

        [TestMethod]
        public void TestFlags(){
            CommandLineArgs args = new CommandLineArgs();

            args.AddFlag("my_test_flag_1");
            args.AddFlag("my_test_flag_2");
            args.AddFlag("aAaAa");

            Assert.IsFalse(args.HasValue("aAaAa"));

            Assert.AreEqual(3, args.Count);
            Assert.IsTrue(args.HasFlag("my_test_flag_1"));
            Assert.IsTrue(args.HasFlag("my_test_flag_2"));
            Assert.IsTrue(args.HasFlag("aaaaa"));
            Assert.IsTrue(args.HasFlag("AAAAA"));
            Assert.AreEqual("my_test_flag_1 my_test_flag_2 aaaaa", args.ToString());

            args.RemoveFlag("Aaaaa");

            Assert.AreEqual(2, args.Count);
            Assert.IsTrue(args.HasFlag("my_test_flag_1"));
            Assert.IsTrue(args.HasFlag("my_test_flag_2"));
            Assert.IsFalse(args.HasFlag("aaaaa"));
            Assert.AreEqual("my_test_flag_1 my_test_flag_2", args.ToString());
        }

        [TestMethod]
        public void TestValues(){
            CommandLineArgs args = new CommandLineArgs();

            args.SetValue("test_value", "My Test Value");
            args.SetValue("aAaAa", "aaaaa");

            Assert.IsFalse(args.HasFlag("aAaAa"));

            Assert.AreEqual(2, args.Count);
            Assert.IsTrue(args.HasValue("test_value"));
            Assert.IsTrue(args.HasValue("aaaaa"));
            Assert.IsTrue(args.HasValue("AAAAA"));
            Assert.AreEqual("My Test Value", args.GetValue("test_value", string.Empty));
            Assert.AreEqual("aaaaa", args.GetValue("aaaaa", string.Empty));
            Assert.AreEqual("test_value \"My Test Value\" aaaaa \"aaaaa\"", args.ToString());

            args.RemoveValue("Aaaaa");

            Assert.AreEqual(1, args.Count);
            Assert.IsTrue(args.HasValue("test_value"));
            Assert.IsFalse(args.HasValue("aaaaa"));
            Assert.AreEqual("test_value \"My Test Value\"", args.ToString());
        }

        [TestMethod]
        public void TestFlagAndValueMix(){
            CommandLineArgs args = new CommandLineArgs();
            
            args.AddFlag("my_test_flag_1");
            args.AddFlag("my_test_flag_2");
            args.AddFlag("aAaAa");

            args.SetValue("test_value", "My Test Value");
            args.SetValue("aAaAa", "aaaaa");

            Assert.AreEqual(5, args.Count);
            Assert.IsTrue(args.HasFlag("aaaaa"));
            Assert.IsTrue(args.HasValue("aaaaa"));
            Assert.AreEqual("my_test_flag_1 my_test_flag_2 aaaaa test_value \"My Test Value\" aaaaa \"aaaaa\"", args.ToString());

            var dict = new Dictionary<string, string>();
            args.ToDictionary(dict); // loses 'aaaaa' flag

            Assert.AreEqual(4, dict.Count);
            Assert.AreEqual("1", dict["my_test_flag_1"]);
            Assert.AreEqual("1", dict["my_test_flag_2"]);
            Assert.AreEqual("My Test Value", dict["test_value"]);
            Assert.AreEqual("aaaaa", dict["aaaaa"]);
        }

        [TestMethod]
        public void TestClone(){
            CommandLineArgs args = new CommandLineArgs();
            
            args.AddFlag("my_test_flag_1");
            args.AddFlag("my_test_flag_2");
            args.AddFlag("aAaAa");

            args.SetValue("test_value", "My Test Value");
            args.SetValue("aAaAa", "aaaaa");

            CommandLineArgs clone = args.Clone();
            args.RemoveFlag("aaaaa");
            args.RemoveValue("aaaaa");
            clone.RemoveFlag("my_test_flag_1");
            clone.RemoveFlag("my_test_flag_2");
            clone.RemoveValue("test_value");

            Assert.AreEqual(3, args.Count);
            Assert.AreEqual(2, clone.Count);

            Assert.AreEqual("my_test_flag_1 my_test_flag_2 test_value \"My Test Value\"", args.ToString());
            Assert.AreEqual("aaaaa aaaaa \"aaaaa\"", clone.ToString());
        }

        [TestMethod]
        public void TestEmptyStringArray(){
            CommandLineArgs args;
            
            args = CommandLineArgs.FromStringArray('-', new string[0]);
            Assert.AreEqual(0, args.Count);

            args = CommandLineArgs.FromStringArray('-', new string[]{ "", "+fail", "@nope" });
            Assert.AreEqual(0, args.Count);
        }

        [TestMethod]
        public void TestValidStringArray(){
            CommandLineArgs args;
            
            args = CommandLineArgs.FromStringArray('-', new string[]{ "-flag1", "-flag2", "-FLAG3" });
            Assert.AreEqual(3, args.Count);
            Assert.IsTrue(args.HasFlag("-flag1"));
            Assert.IsTrue(args.HasFlag("-flag2"));
            Assert.IsTrue(args.HasFlag("-flag3"));

            args = CommandLineArgs.FromStringArray('-', new string[]{ "-flag", "-value", "Here is some text!" });
            Assert.AreEqual(2, args.Count);
            Assert.IsTrue(args.HasFlag("-flag"));
            Assert.IsTrue(args.HasValue("-value"));
            Assert.AreEqual("Here is some text!", args.GetValue("-value", string.Empty));
        }
    }
}
