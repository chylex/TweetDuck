using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetLib.Core.Data;

namespace TweetTest.Data{
    [TestClass]
    public class TestCombinedFileStream : TestIO{
        [TestMethod]
        public void TestNoFiles(){
            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenWrite("empty"))){
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("empty"));

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenRead("empty"))){
                Assert.IsNull(cfs.ReadFile());
            }

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenRead("empty"))){
                Assert.IsNull(cfs.SkipFile());
            }
        }

        [TestMethod]
        public void TestEmptyFiles(){
            File.WriteAllText("input_empty_1", string.Empty);
            File.WriteAllText("input_empty_2", string.Empty);

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenWrite("blank_files"))){
                cfs.WriteFile("id1", "input_empty_1");
                cfs.WriteFile("id2", "input_empty_2");
                cfs.WriteFile("id2_clone", "input_empty_2");
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("blank_files"));

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenRead("blank_files"))){
                CombinedFileStream.Entry entry1 = cfs.ReadFile();
                string entry2key = cfs.SkipFile();
                CombinedFileStream.Entry entry3 = cfs.ReadFile();

                Assert.IsNull(cfs.ReadFile());
                Assert.IsNull(cfs.SkipFile());

                Assert.AreEqual("id1", entry1.KeyName);
                Assert.AreEqual("id1", entry1.Identifier);
                CollectionAssert.AreEqual(new string[0], entry1.KeyValue);

                Assert.AreEqual("id2", entry2key);

                Assert.AreEqual("id2_clone", entry3.KeyName);
                Assert.AreEqual("id2_clone", entry3.Identifier);
                CollectionAssert.AreEqual(new string[0], entry3.KeyValue);

                entry1.WriteToFile("blank_file_1");
                entry3.WriteToFile("blank_file_2");
            }

            Assert.IsTrue(File.Exists("blank_file_1"));
            Assert.IsTrue(File.Exists("blank_file_2"));
            Assert.AreEqual(string.Empty, File.ReadAllText("blank_file_1"));
            Assert.AreEqual(string.Empty, File.ReadAllText("blank_file_2"));
        }

        [TestMethod]
        public void TestTextFilesAndComplexKeys(){
            File.WriteAllText("input_text_1", "Hello World!" + Environment.NewLine);

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenWrite("text_files"))){
                cfs.WriteFile(new string[]{ "key1", "a", "bb", "ccc", "dddd" }, "input_text_1");
                cfs.WriteFile(new string[]{ "key2", "a", "bb", "ccc", "dddd" }, "input_text_1");
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("text_files"));

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenRead("text_files"))){
                CombinedFileStream.Entry entry = cfs.ReadFile();

                Assert.AreEqual("key2", cfs.SkipFile());
                Assert.IsNull(cfs.ReadFile());
                Assert.IsNull(cfs.SkipFile());

                Assert.AreEqual("key1|a|bb|ccc|dddd", entry.Identifier);
                Assert.AreEqual("key1", entry.KeyName);
                CollectionAssert.AreEqual(new string[]{ "a", "bb", "ccc", "dddd" }, entry.KeyValue);

                entry.WriteToFile("text_file_1");
            }

            Assert.IsTrue(File.Exists("text_file_1"));
            Assert.AreEqual("Hello World!" + Environment.NewLine, File.ReadAllText("text_file_1"));
        }

        [TestMethod]
        public void TestEntryWriteWithDirectory(){
            if (Directory.Exists("directory")){
                Directory.Delete("directory", true);
            }

            File.WriteAllText("input_dir_1", "test");

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenWrite("dir_test"))){
                cfs.WriteFile("key1", "input_dir_1");
                cfs.WriteFile("key2", "input_dir_1");
                cfs.WriteFile("key3", "input_dir_1");
                cfs.WriteFile("key4", "input_dir_1");
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("dir_test"));

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenRead("dir_test"))){
                try{
                    cfs.ReadFile().WriteToFile("directory/dir_test_file", false);
                    Assert.Fail("WriteToFile did not trigger an exception.");
                }catch(DirectoryNotFoundException){}

                cfs.ReadFile().WriteToFile("directory/dir_test_file", true);
                cfs.ReadFile().WriteToFile("dir_test_file", true);
                cfs.ReadFile().WriteToFile("dir_test_file.txt", true);
            }

            Assert.IsTrue(Directory.Exists("directory"));
            Assert.IsTrue(File.Exists("directory/dir_test_file"));
            Assert.IsTrue(File.Exists("dir_test_file"));
            Assert.IsTrue(File.Exists("dir_test_file.txt"));
            Assert.AreEqual("test", File.ReadAllText("directory/dir_test_file"));
            Assert.AreEqual("test", File.ReadAllText("dir_test_file"));
            Assert.AreEqual("test", File.ReadAllText("dir_test_file.txt"));

            Directory.Delete("directory", true);
        }

        [TestMethod]
        public void TestLongIdentifierSuccess(){
            File.WriteAllText("long_identifier_fail_in", "test");

            string identifier = string.Join("", Enumerable.Repeat("x", 255));

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenWrite("long_identifier_success"))){
                cfs.WriteFile(identifier, "long_identifier_fail_in");
                cfs.Flush();
            }

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenRead("long_identifier_success"))){
                Assert.AreEqual(identifier, cfs.ReadFile().Identifier);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestLongIdentifierFail(){
            File.WriteAllText("long_identifier_fail_in", "test");

            using(CombinedFileStream cfs = new CombinedFileStream(File.OpenWrite("long_identifier_fail"))){
                cfs.WriteFile(string.Join("", Enumerable.Repeat("x", 256)), "long_identifier_fail_in");
            }
        }
    }
}
