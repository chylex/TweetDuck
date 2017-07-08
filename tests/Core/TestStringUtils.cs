using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Core.Utils;

namespace UnitTests.Core{
    [TestClass]
    public class TestStringUtils{
        [TestMethod]
        public void TestExtractBefore(){
            Assert.AreEqual("missing", StringUtils.ExtractBefore("missing", '_'));
            Assert.AreEqual("", StringUtils.ExtractBefore("_empty", '_'));
            Assert.AreEqual("some", StringUtils.ExtractBefore("some_text", '_'));
            Assert.AreEqual("first", StringUtils.ExtractBefore("first_separator_only", '_'));
            Assert.AreEqual("start_index", StringUtils.ExtractBefore("start_index_test", '_', 8));
        }

        [TestMethod]
        public void TestParseInts(){
            CollectionAssert.AreEqual(new int[0], StringUtils.ParseInts("", ','));
            CollectionAssert.AreEqual(new int[]{ 1 }, StringUtils.ParseInts("1", ','));
            CollectionAssert.AreEqual(new int[]{ 1, 2, 3 }, StringUtils.ParseInts("1,2,3", ','));
            CollectionAssert.AreEqual(new int[]{ 1, 2, 3 }, StringUtils.ParseInts("1,2,3,", ','));
            CollectionAssert.AreEqual(new int[]{ 1, 2, 3 }, StringUtils.ParseInts(",1,2,,3,", ','));
            CollectionAssert.AreEqual(new int[]{ -50, 50 }, StringUtils.ParseInts("-50,50", ','));
        }

        [TestMethod]
        public void TestConvertPascalCaseToScreamingSnakeCase(){
            Assert.AreEqual("HELP", StringUtils.ConvertPascalCaseToScreamingSnakeCase("Help"));
            Assert.AreEqual("HELP_ME", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HelpMe"));
            Assert.AreEqual("HELP_ME_PLEASE", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HelpMePlease"));

            Assert.AreEqual("HTML_CODE", StringUtils.ConvertPascalCaseToScreamingSnakeCase("HTMLCode"));
            Assert.AreEqual("CHECK_OUT_MY_HTML_CODE", StringUtils.ConvertPascalCaseToScreamingSnakeCase("CheckOutMyHTMLCode"));
        }
    }
}
