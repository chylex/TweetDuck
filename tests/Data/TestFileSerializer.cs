using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Data.Serialization;

namespace UnitTests.Data{
    [TestClass]
    public class TestFileSerializer : UnitTestIO{
        private enum TestEnum{
            A, B, C, D, E
        }

        private class SerializationTestBasic{
            public bool TestBool { get; set; }
            public int TestInt { get; set; }
            public string TestString { get; set; }
            public string TestStringNull { get; set; }
            public TestEnum TestEnum { get; set; }
        }

        [TestMethod]
        public void TestBasicWriteRead(){
            FileSerializer<SerializationTestBasic> serializer = new FileSerializer<SerializationTestBasic>();
            
            SerializationTestBasic write = new SerializationTestBasic{
                TestBool = true,
                TestInt = -100,
                TestString = "abc"+Environment.NewLine+"def",
                TestStringNull = null,
                TestEnum = TestEnum.D
            };

            serializer.Write("serialized_basic", write);
            Assert.IsTrue(File.Exists("serialized_basic"));

            SerializationTestBasic read = new SerializationTestBasic();
            serializer.Read("serialized_basic", read);

            Assert.IsTrue(read.TestBool);
            Assert.AreEqual(-100, read.TestInt);
            Assert.AreEqual("abc"+Environment.NewLine+"def", read.TestString);
            Assert.IsNull(read.TestStringNull);
            Assert.AreEqual(TestEnum.D, read.TestEnum);
        }

        // TODO more complex tests
    }
}
