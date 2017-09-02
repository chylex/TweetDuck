using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Core.Utils;

namespace UnitTests.Core{
    [TestClass]
    public class TestBrowserUtils{
        [TestMethod]
        public void TestIsValidUrl(){
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://google.com")); // base
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://www.google.com")); // www.
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://google.co.uk")); // co.uk
            
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("https://google.com")); // https
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("ftp://google.com")); // ftp
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("mailto:someone@google.com")); // mailto

            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://google.com/")); // trailing slash
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://google.com/?")); // trailing question mark
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://google.com/?a=5&b=x")); // parameters
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://google.com/#hash")); // parameters + hash
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://google.com/?a=5&b=x#hash")); // parameters + hash

            foreach(string tld in new string[]{ "accountants", "blackfriday", "cancerresearch", "coffee", "cool", "foo", "travelersinsurance" }){
                Assert.AreEqual(BrowserUtils.UrlCheckResult.Fine, BrowserUtils.CheckUrl("http://test."+tld)); // long and unusual TLDs
            }

            Assert.AreEqual(BrowserUtils.UrlCheckResult.Tracking, BrowserUtils.CheckUrl("http://t.co/abc")); // tracking
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Tracking, BrowserUtils.CheckUrl("https://t.co/abc")); // tracking

            Assert.AreEqual(BrowserUtils.UrlCheckResult.Invalid, BrowserUtils.CheckUrl("explorer")); // file
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Invalid, BrowserUtils.CheckUrl("explorer.exe")); // file
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Invalid, BrowserUtils.CheckUrl("://explorer.exe")); // file-sorta
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Invalid, BrowserUtils.CheckUrl("file://explorer.exe")); // file-proper
            
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Invalid, BrowserUtils.CheckUrl("")); // empty
            Assert.AreEqual(BrowserUtils.UrlCheckResult.Invalid, BrowserUtils.CheckUrl("lol")); // random

            Assert.AreEqual(BrowserUtils.UrlCheckResult.Invalid, BrowserUtils.CheckUrl("gopher://nobody.cares")); // lmao rekt
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
