using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDck.Core.Utils;

namespace UnitTests.Core.Utils{
    [TestClass]
    public class TestBrowserUtils{
        [TestMethod]
        public void TestIsValidUrl(){
            Assert.IsTrue(BrowserUtils.IsValidUrl("http://google.com")); // base
            Assert.IsTrue(BrowserUtils.IsValidUrl("http://www.google.com")); // www.
            Assert.IsTrue(BrowserUtils.IsValidUrl("http://google.co.uk")); // co.uk
            
            Assert.IsTrue(BrowserUtils.IsValidUrl("https://google.com")); // https
            Assert.IsTrue(BrowserUtils.IsValidUrl("ftp://google.com")); // ftp
            Assert.IsTrue(BrowserUtils.IsValidUrl("mailto:someone@google.com")); // mailto

            Assert.IsTrue(BrowserUtils.IsValidUrl("http://google.com/")); // trailing slash
            Assert.IsTrue(BrowserUtils.IsValidUrl("http://google.com/?")); // trailing question mark
            Assert.IsTrue(BrowserUtils.IsValidUrl("http://google.com/?a=5&b=x")); // parameters
            Assert.IsTrue(BrowserUtils.IsValidUrl("http://google.com/#hash")); // parameters + hash
            Assert.IsTrue(BrowserUtils.IsValidUrl("http://google.com/?a=5&b=x#hash")); // parameters + hash

            foreach(string tld in new string[]{ "accountants", "blackfriday", "cancerresearch", "coffee", "cool", "foo", "travelersinsurance" }){
                Assert.IsTrue(BrowserUtils.IsValidUrl("http://test."+tld)); // long and unusual TLDs
            }

            Assert.IsFalse(BrowserUtils.IsValidUrl("explorer")); // file
            Assert.IsFalse(BrowserUtils.IsValidUrl("explorer.exe")); // file
            Assert.IsFalse(BrowserUtils.IsValidUrl("://explorer.exe")); // file-sorta
            Assert.IsFalse(BrowserUtils.IsValidUrl("file://explorer.exe")); // file-proper
            
            Assert.IsFalse(BrowserUtils.IsValidUrl("")); // empty
            Assert.IsFalse(BrowserUtils.IsValidUrl("lol")); // random

            Assert.IsFalse(BrowserUtils.IsValidUrl("gopher://nobody.cares")); // lmao rekt
        }

        [TestMethod]
        public void TestGetFileNameFromUrl(){
            Assert.AreEqual("index.html", BrowserUtils.GetFileNameFromUrl("http://test.com/index.html"));
            Assert.AreEqual("index.html", BrowserUtils.GetFileNameFromUrl("http://test.com/index.html?"));
            Assert.AreEqual("index.html", BrowserUtils.GetFileNameFromUrl("http://test.com/index.html?param1=abc&param2=false"));
            
            Assert.AreEqual("index", BrowserUtils.GetFileNameFromUrl("http://test.com/index"));
            Assert.AreEqual("index.", BrowserUtils.GetFileNameFromUrl("http://test.com/index."));

            Assert.IsNull(BrowserUtils.GetFileNameFromUrl("http://test.com/"));
        }
    }
}
