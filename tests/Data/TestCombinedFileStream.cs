using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TweetDuck.Data;

namespace UnitTests.Data{
    [TestClass]
    public class TestCombinedFileStream{
        [TestMethod]
        public void TestNoFiles(){
            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.WriteFile("cfs_empty"))){
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("cfs_empty"));

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.ReadFile("cfs_empty"))){
                Assert.IsNull(cfs.ReadFile());
            }

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.ReadFile("cfs_empty"))){
                Assert.IsNull(cfs.SkipFile());
            }
        }

        [TestMethod]
        public void TestEmptyFiles(){
            TestUtils.WriteText("cfs_input_empty_1", string.Empty);
            TestUtils.WriteText("cfs_input_empty_2", string.Empty);

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.WriteFile("cfs_blank_files"))){
                cfs.WriteFile("id1", "cfs_input_empty_1");
                cfs.WriteFile("id2", "cfs_input_empty_2");
                cfs.WriteFile("id2_clone", "cfs_input_empty_2");
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("cfs_blank_files"));

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.ReadFile("cfs_blank_files"))){
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

                entry1.WriteToFile("cfs_blank_file_1");
                entry3.WriteToFile("cfs_blank_file_2");
                TestUtils.DeleteFileOnExit("cfs_blank_file_1");
                TestUtils.DeleteFileOnExit("cfs_blank_file_2");
            }

            Assert.IsTrue(File.Exists("cfs_blank_file_1"));
            Assert.IsTrue(File.Exists("cfs_blank_file_2"));
            Assert.AreEqual(string.Empty, TestUtils.ReadText("cfs_blank_file_1"));
            Assert.AreEqual(string.Empty, TestUtils.ReadText("cfs_blank_file_2"));
        }

        [TestMethod]
        public void TestTextFilesAndComplexKeys(){
            TestUtils.WriteText("cfs_input_text_1", "Hello World!"+Environment.NewLine);

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.WriteFile("cfs_text_files"))){
                cfs.WriteFile(new string[]{ "key1", "a", "bb", "ccc", "dddd" }, "cfs_input_text_1");
                cfs.WriteFile(new string[]{ "key2", "a", "bb", "ccc", "dddd" }, "cfs_input_text_1");
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("cfs_text_files"));

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.ReadFile("cfs_text_files"))){
                CombinedFileStream.Entry entry = cfs.ReadFile();

                Assert.AreEqual("key2", cfs.SkipFile());
                Assert.IsNull(cfs.ReadFile());
                Assert.IsNull(cfs.SkipFile());

                Assert.AreEqual("key1|a|bb|ccc|dddd", entry.Identifier);
                Assert.AreEqual("key1", entry.KeyName);
                CollectionAssert.AreEqual(new string[]{ "a", "bb", "ccc", "dddd" }, entry.KeyValue);

                entry.WriteToFile("cfs_text_file_1");
                TestUtils.DeleteFileOnExit("cfs_text_file_1");
            }

            Assert.IsTrue(File.Exists("cfs_text_file_1"));
            Assert.AreEqual("Hello World!"+Environment.NewLine, TestUtils.ReadText("cfs_text_file_1"));
        }

        [TestMethod]
        public void TestEntryWriteWithDirectory(){
            if (Directory.Exists("cfs_directory")){
                Directory.Delete("cfs_directory", true);
            }

            TestUtils.WriteText("cfs_input_dir_1", "test");

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.WriteFile("cfs_dir_test"))){
                cfs.WriteFile("key1", "cfs_input_dir_1");
                cfs.WriteFile("key2", "cfs_input_dir_1");
                cfs.WriteFile("key3", "cfs_input_dir_1");
                cfs.WriteFile("key4", "cfs_input_dir_1");
                cfs.Flush();
            }

            Assert.IsTrue(File.Exists("cfs_dir_test"));

            using(CombinedFileStream cfs = new CombinedFileStream(TestUtils.ReadFile("cfs_dir_test"))){
                try{
                    cfs.ReadFile().WriteToFile("cfs_directory/cfs_dir_test_file", false);
                    Assert.Fail("WriteToFile did not trigger an exception.");
                }catch(DirectoryNotFoundException){}

                cfs.ReadFile().WriteToFile("cfs_directory/cfs_dir_test_file", true);
                cfs.ReadFile().WriteToFile("cfs_dir_test_file", true);
                cfs.ReadFile().WriteToFile("cfs_dir_test_file.txt", true);
                TestUtils.DeleteFileOnExit("cfs_dir_test_file");
                TestUtils.DeleteFileOnExit("cfs_dir_test_file.txt");
            }

            Assert.IsTrue(Directory.Exists("cfs_directory"));
            Assert.IsTrue(File.Exists("cfs_directory/cfs_dir_test_file"));
            Assert.IsTrue(File.Exists("cfs_dir_test_file"));
            Assert.IsTrue(File.Exists("cfs_dir_test_file.txt"));
            Assert.AreEqual("test", TestUtils.ReadText("cfs_directory/cfs_dir_test_file"));
            Assert.AreEqual("test", TestUtils.ReadText("cfs_dir_test_file"));
            Assert.AreEqual("test", TestUtils.ReadText("cfs_dir_test_file.txt"));

            Directory.Delete("cfs_directory", true);
        }
    }
}
