using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Core.Utils;

namespace UnitTests.Core.Utils{
    [TestClass]
    public class TestInjectedHTML{
        private static IEnumerable<InjectedHTML.Position> Positions => Enum.GetValues(typeof(InjectedHTML.Position)).Cast<InjectedHTML.Position>();

        [TestMethod]
        public void TestFailedMatches(){
            foreach(var pos in Positions){
                Assert.AreEqual(string.Empty, new InjectedHTML(pos, "b", "b").Inject(string.Empty));
                Assert.AreEqual("aaaa", new InjectedHTML(pos, "b", "b").Inject("aaaa"));
            }
        }

        [TestMethod]
        public void TestEmptySearch(){
            foreach(var pos in Positions){
                Assert.AreEqual("b", new InjectedHTML(pos, string.Empty, "b").Inject(string.Empty));
                Assert.AreEqual("baaaa", new InjectedHTML(pos, string.Empty, "b").Inject("aaaa"));
            }
        }

        [TestMethod]
        public void TestEmptyHTML(){
            foreach(var pos in Positions){
                Assert.AreEqual(string.Empty, new InjectedHTML(pos, string.Empty, string.Empty).Inject(string.Empty));
                Assert.AreEqual("aaaa", new InjectedHTML(pos, string.Empty, string.Empty).Inject("aaaa"));
                Assert.AreEqual("aaaa", new InjectedHTML(pos, "a", string.Empty).Inject("aaaa"));
                Assert.AreEqual("aaaa", new InjectedHTML(pos, "b", string.Empty).Inject("aaaa"));
            }
        }

        [TestMethod]
        public void TestInvalidPosition(){
            Assert.AreEqual("aaaa", new InjectedHTML((InjectedHTML.Position)(Positions.Count()+1), "a", "b").Inject("aaaa"));
        }

        [TestMethod]
        public void TestPositions(){
            Assert.AreEqual("aaabcxaaa", new InjectedHTML(InjectedHTML.Position.Before, "x", "bc").Inject("aaaxaaa"));
            Assert.AreEqual("aaaxbcaaa", new InjectedHTML(InjectedHTML.Position.After, "x", "bc").Inject("aaaxaaa"));

            Assert.AreEqual("bcxaaa", new InjectedHTML(InjectedHTML.Position.Before, "x", "bc").Inject("xaaa"));
            Assert.AreEqual("xbcaaa", new InjectedHTML(InjectedHTML.Position.After, "x", "bc").Inject("xaaa"));

            Assert.AreEqual("aaabcx", new InjectedHTML(InjectedHTML.Position.Before, "x", "bc").Inject("aaax"));
            Assert.AreEqual("aaaxbc", new InjectedHTML(InjectedHTML.Position.After, "x", "bc").Inject("aaax"));
        }

        [TestMethod]
        public void TestFirstOccurrence(){
            Assert.AreEqual("bcaaaa", new InjectedHTML(InjectedHTML.Position.Before, "a", "bc").Inject("aaaa"));
            Assert.AreEqual("abcaaa", new InjectedHTML(InjectedHTML.Position.After, "a", "bc").Inject("aaaa"));

            Assert.AreEqual("bcaaaa", new InjectedHTML(InjectedHTML.Position.Before, "aa", "bc").Inject("aaaa"));
            Assert.AreEqual("aabcaa", new InjectedHTML(InjectedHTML.Position.After, "aa", "bc").Inject("aaaa"));
        }
    }
}
