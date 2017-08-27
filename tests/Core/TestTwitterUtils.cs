using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Core.Utils;

namespace UnitTests.Core{
    [TestClass]
    public class TestTwitterUtils{
        [TestMethod]
        public void TestAccountRegex(){
            Assert.IsTrue(TwitterUtils.RegexAccount.IsMatch("http://twitter.com/chylexmc"));
            Assert.IsTrue(TwitterUtils.RegexAccount.IsMatch("https://twitter.com/chylexmc"));
            Assert.IsTrue(TwitterUtils.RegexAccount.IsMatch("http://twitter.com/chylexmc/"));
            Assert.IsTrue(TwitterUtils.RegexAccount.IsMatch("https://twitter.com/chylexmc/"));

            Assert.AreEqual("chylexmc", TwitterUtils.RegexAccount.Match("http://twitter.com/chylexmc").Groups[1].Value);
            Assert.AreEqual("123", TwitterUtils.RegexAccount.Match("http://twitter.com/123").Groups[1].Value);
            Assert.AreEqual("_", TwitterUtils.RegexAccount.Match("http://twitter.com/_").Groups[1].Value);

            Assert.AreEqual("Abc_123", TwitterUtils.RegexAccount.Match("http://twitter.com/Abc_123").Groups[1].Value);
            Assert.AreEqual("Abc_123", TwitterUtils.RegexAccount.Match("https://twitter.com/Abc_123/").Groups[1].Value);

            Assert.IsFalse(TwitterUtils.RegexAccount.IsMatch("http://twitter.com/"));
            Assert.IsFalse(TwitterUtils.RegexAccount.IsMatch("http://twitter.com/chylexmc/status"));
            Assert.IsFalse(TwitterUtils.RegexAccount.IsMatch("http://nottwitter.com/chylexmc"));
            Assert.IsFalse(TwitterUtils.RegexAccount.IsMatch("www.twitter.com/chylexmc"));
        }

        [TestMethod]
        public void TestImageQualityLink(){
            Assert.AreEqual("https://pbs.twimg.com/profile_images/123", TwitterUtils.GetMediaLink("https://pbs.twimg.com/profile_images/123", TwitterUtils.ImageQuality.Default));
            Assert.AreEqual("https://pbs.twimg.com/profile_images/123", TwitterUtils.GetMediaLink("https://pbs.twimg.com/profile_images/123", TwitterUtils.ImageQuality.Orig));

            Assert.AreEqual("https://pbs.twimg.com/profile_images/123.jpg", TwitterUtils.GetMediaLink("https://pbs.twimg.com/profile_images/123.jpg", TwitterUtils.ImageQuality.Default));
            Assert.AreEqual("https://pbs.twimg.com/profile_images/123.jpg", TwitterUtils.GetMediaLink("https://pbs.twimg.com/profile_images/123.jpg", TwitterUtils.ImageQuality.Orig));

            Assert.AreEqual("https://pbs.twimg.com/media/123.jpg", TwitterUtils.GetMediaLink("https://pbs.twimg.com/media/123.jpg", TwitterUtils.ImageQuality.Default));
            Assert.AreEqual("https://pbs.twimg.com/media/123.jpg:orig", TwitterUtils.GetMediaLink("https://pbs.twimg.com/media/123.jpg", TwitterUtils.ImageQuality.Orig));

            Assert.AreEqual("https://pbs.twimg.com/media/123.jpg:small", TwitterUtils.GetMediaLink("https://pbs.twimg.com/media/123.jpg:small", TwitterUtils.ImageQuality.Default));
            Assert.AreEqual("https://pbs.twimg.com/media/123.jpg:orig", TwitterUtils.GetMediaLink("https://pbs.twimg.com/media/123.jpg:small", TwitterUtils.ImageQuality.Orig));

            Assert.AreEqual("https://pbs.twimg.com/media/123.jpg:large", TwitterUtils.GetMediaLink("https://pbs.twimg.com/media/123.jpg:large", TwitterUtils.ImageQuality.Default));
            Assert.AreEqual("https://pbs.twimg.com/media/123.jpg:orig", TwitterUtils.GetMediaLink("https://pbs.twimg.com/media/123.jpg:large", TwitterUtils.ImageQuality.Orig));
        }
    }
}
