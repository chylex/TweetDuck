using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Data.Serialization;

namespace UnitTests.Data{
    [TestClass]
    public class TestFileSerializer{
        private class SerializationTestBasic : ISerializedObject{
            public bool TestBool { get; set; }
            public int TestInt { get; set; }
            public string TestString { get; set; }
            public string TestStringNull { get; set; }

            bool ISerializedObject.OnReadUnknownProperty(string property, string value){
                return false;
            }
        }

        [TestMethod]
        public void TestBasicWriteRead(){
            FileSerializer<SerializationTestBasic> serializer = new FileSerializer<SerializationTestBasic>();
            
            SerializationTestBasic write = new SerializationTestBasic{
                TestBool = true,
                TestInt = -100,
                TestString = "abc"+Environment.NewLine+"def",
                TestStringNull = null
            };

            serializer.Write("serialized_basic", write);

            Assert.IsTrue(File.Exists("serialized_basic"));
            TestUtils.DeleteFileOnExit("serialized_basic");

            SerializationTestBasic read = new SerializationTestBasic();
            serializer.Read("serialized_basic", read);

            Assert.IsTrue(read.TestBool);
            Assert.AreEqual(-100, read.TestInt);
            Assert.AreEqual("abc"+Environment.NewLine+"def", read.TestString);
            Assert.IsNull(read.TestStringNull);
        }
    }
}
