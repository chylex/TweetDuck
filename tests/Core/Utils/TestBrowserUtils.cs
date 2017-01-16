using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDck.Core.Utils;

namespace UnitTests.Core.Utils{
    [TestClass]
    public class TestBrowserUtils{
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
