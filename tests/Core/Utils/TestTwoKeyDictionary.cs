using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDck.Core.Utils;
using System.Collections.Generic;

namespace UnitTests.Core.Utils{
    [TestClass]
    public class TestTwoKeyDictionary{
        private static TwoKeyDictionary<string, int, string> CreateDict(){
            TwoKeyDictionary<string, int, string> dict = new TwoKeyDictionary<string, int, string>();

            dict.Add("aaa", 0, "x");
            dict.Add("aaa", 1, "y");
            dict.Add("aaa", 2, "z");

            dict.Add("bbb", 0, "test 1");
            dict.Add("bbb", 10, "test 2");
            dict.Add("bbb", 20, "test 3");
            dict.Add("bbb", 30, "test 4");

            dict.Add("ccc", -5, "");
            dict.Add("", 0, "");

            return dict;
        }

        [TestMethod]
        public void TestAdd(){
            CreateDict();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddDuplicate(){
            var dict = new TwoKeyDictionary<string, int, string>();

            dict.Add("aaa", 0, "test");
            dict.Add("aaa", 0, "oops");
        }

        [TestMethod]
        public void TestAccessor(){
            var dict = CreateDict();

            // get

            Assert.AreEqual("x", dict["aaa", 0]);
            Assert.AreEqual("y", dict["aaa", 1]);
            Assert.AreEqual("z", dict["aaa", 2]);
            
            Assert.AreEqual("test 3", dict["bbb", 20]);
            
            Assert.AreEqual("", dict["ccc", -5]);
            Assert.AreEqual("", dict["", 0]);

            // set

            dict["aaa", 0] = "replaced entry";
            Assert.AreEqual("replaced entry", dict["aaa", 0]);

            dict["aaa", 3] = "new entry";
            Assert.AreEqual("new entry", dict["aaa", 3]);

            dict["xxxxx", 150] = "new key and entry";
            Assert.AreEqual("new key and entry", dict["xxxxx", 150]);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestAccessorMissingKey1(){
            var _ = CreateDict()["missing", 0];
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestAccessorMissingKey2(){
            var _ = CreateDict()["aaa", 3];
        }

        [TestMethod]
        public void TestClear(){
            var dict = CreateDict();
            
            Assert.IsTrue(dict.Contains("bbb"));
            dict.Clear("bbb");
            Assert.IsTrue(dict.Contains("bbb"));
            
            Assert.IsTrue(dict.Contains(""));
            dict.Clear("");
            Assert.IsTrue(dict.Contains(""));
            
            Assert.IsTrue(dict.Contains("aaa"));
            Assert.IsTrue(dict.Contains("ccc"));
            dict.Clear();
            Assert.IsFalse(dict.Contains("aaa"));
            Assert.IsFalse(dict.Contains("ccc"));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestClearMissingKey(){
            CreateDict().Clear("missing");
        }

        [TestMethod]
        public void TestContains(){
            var dict = CreateDict();
            
            // positive

            Assert.IsTrue(dict.Contains("aaa"));
            Assert.IsTrue(dict.Contains("aaa", 0));
            Assert.IsTrue(dict.Contains("aaa", 1));
            Assert.IsTrue(dict.Contains("aaa", 2));
            
            Assert.IsTrue(dict.Contains("ccc"));
            Assert.IsTrue(dict.Contains("ccc", -5));

            Assert.IsTrue(dict.Contains(""));
            Assert.IsTrue(dict.Contains("", 0));

            // negative

            Assert.IsFalse(dict.Contains("missing"));
            Assert.IsFalse(dict.Contains("missing", 999));

            Assert.IsFalse(dict.Contains("aaa", 3));
            Assert.IsFalse(dict.Contains("", -1));
        }

        [TestMethod]
        public void TestCount(){
            var dict = CreateDict();

            Assert.AreEqual(9, dict.Count());
            Assert.AreEqual(3, dict.Count("aaa"));
            Assert.AreEqual(4, dict.Count("bbb"));
            Assert.AreEqual(1, dict.Count("ccc"));
            Assert.AreEqual(1, dict.Count(""));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void TestCountMissingKey(){
            CreateDict().Count("missing");
        }

        [TestMethod]
        public void TestRemove(){
            var dict = CreateDict();

            // negative
            Assert.IsFalse(dict.Remove("missing"));
            Assert.IsFalse(dict.Remove("aaa", 3));
            
            // positive

            Assert.IsTrue(dict.Contains("aaa"));
            Assert.IsTrue(dict.Remove("aaa"));
            Assert.IsFalse(dict.Contains("aaa"));

            Assert.IsTrue(dict.Contains("bbb", 10));
            Assert.IsTrue(dict.Remove("bbb", 10));
            Assert.IsFalse(dict.Contains("bbb", 10));
            Assert.IsTrue(dict.Contains("bbb"));
            Assert.IsTrue(dict.Contains("bbb", 20));

            Assert.IsTrue(dict.Remove("bbb", 0));
            Assert.IsTrue(dict.Remove("bbb", 20));
            Assert.IsTrue(dict.Remove("bbb", 30));
            Assert.IsFalse(dict.Contains("bbb"));
            
            Assert.IsTrue(dict.Contains(""));
            Assert.IsTrue(dict.Remove("", 0));
            Assert.IsFalse(dict.Contains(""));
        }

        [TestMethod]
        public void TestTryGetValue(){
            var dict = CreateDict();
            string val;

            // positive

            Assert.IsTrue(dict.TryGetValue("bbb", 10, out val));
            Assert.AreEqual("test 2", val);

            Assert.IsTrue(dict.TryGetValue("ccc", -5, out val));
            Assert.AreEqual("", val);

            Assert.IsTrue(dict.TryGetValue("", 0, out val));
            Assert.AreEqual("", val);

            // negative

            Assert.IsFalse(dict.TryGetValue("ccc", -50, out val));
            Assert.IsFalse(dict.TryGetValue("", 1, out val));
            Assert.IsFalse(dict.TryGetValue("missing", 0, out val));
        }
    }
}
