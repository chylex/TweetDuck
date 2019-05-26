using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetLib.Core.Serialization;

namespace TweetTest.Data{
    [TestClass]
    public class TestFileSerializer : TestIO{
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private enum TestEnum{
            A, B, C, D, E
        }

        private class SerializationTestBasic{
            public bool TestBool { get; set; }
            public int TestInt { get; set; }
            public string TestStringBasic { get; set; }
            public string TestStringNewLine { get; set; }
            public string TestStringBackslash { get; set; }
            public string TestStringNull { get; set; }
            public TestEnum TestEnum { get; set; }
        }

        [TestMethod]
        public void TestBasicWriteRead(){
            FileSerializer<SerializationTestBasic> serializer = new FileSerializer<SerializationTestBasic>();
            
            SerializationTestBasic write = new SerializationTestBasic{
                TestBool = true,
                TestInt = -100,
                TestStringBasic = "hello123",
                TestStringNewLine = "abc"+Environment.NewLine+"def"+Environment.NewLine,
                TestStringBackslash = @"C:\Test\\\Abc\",
                TestStringNull = null,
                TestEnum = TestEnum.D
            };

            serializer.Write("basic_wr", write);
            Assert.IsTrue(File.Exists("basic_wr"));

            SerializationTestBasic read = new SerializationTestBasic();
            serializer.Read("basic_wr", read);

            Assert.IsTrue(read.TestBool);
            Assert.AreEqual(-100, read.TestInt);
            Assert.AreEqual("hello123", read.TestStringBasic);
            Assert.AreEqual("abc"+Environment.NewLine+"def"+Environment.NewLine, read.TestStringNewLine);
            Assert.AreEqual(@"C:\Test\\\Abc\", read.TestStringBackslash);
            Assert.IsNull(read.TestStringNull);
            Assert.AreEqual(TestEnum.D, read.TestEnum);
        }

        // TODO more complex tests
    }
}
