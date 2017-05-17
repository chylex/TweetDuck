using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Core.Utils;

namespace UnitTests.Core.Utils{
    [TestClass]
    public class TestCommandLineArgsParser{
        [TestMethod]
        public void TestEmptyString(){
            Assert.AreEqual(0, CommandLineArgsParser.ReadCefArguments("").Count);
            Assert.AreEqual(0, CommandLineArgsParser.ReadCefArguments("     ").Count);
        }

        [TestMethod]
        public void TestValidString(){
            CommandLineArgs args = CommandLineArgsParser.ReadCefArguments("--aaa --bbb --first-value=123 --SECOND-VALUE=\"a b c d e\" --ccc");
            // cef has no flags, flag arguments have a value of 1
            // the processing removes all dashes in front of each key

            Assert.AreEqual(5, args.Count);
            Assert.IsTrue(args.HasValue("aaa"));
            Assert.IsTrue(args.HasValue("bbb"));
            Assert.IsTrue(args.HasValue("ccc"));
            Assert.IsTrue(args.HasValue("first-value"));
            Assert.IsTrue(args.HasValue("second-value"));
            Assert.AreEqual("1", args.GetValue("aaa", string.Empty));
            Assert.AreEqual("1", args.GetValue("bbb", string.Empty));
            Assert.AreEqual("1", args.GetValue("ccc", string.Empty));
            Assert.AreEqual("123", args.GetValue("first-value", string.Empty));
            Assert.AreEqual("a b c d e", args.GetValue("second-value", string.Empty));
        }
    }
}
