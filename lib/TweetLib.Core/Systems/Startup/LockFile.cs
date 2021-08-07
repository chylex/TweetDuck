using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using TweetLib.Core.Utils;

namespace TweetLib.Core.Systems.Startup {
	public sealed class LockFile {
		private static int CurrentProcessID {
			get {
				using Process me = Process.GetCurrentProcess();
				return me.Id;
			}
		}

		private readonly string path;
		private FileStream? stream;

		public LockFile(string path) {
			this.path = path;
		}

		private void CreateLockFileStream() {
			stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
			stream.Write(BitConverter.GetBytes(CurrentProcessID), 0, sizeof(int));
			stream.Flush(true);
		}

		private bool ReleaseLockFileStream() {
			if (stream != null) {
				stream.Dispose();
				stream = null;
				return true;
			}
			else {
				return false;
			}
		}

		public LockResult Lock() {
			if (stream != null) {
				return LockResult.Success;
			}

			try {
				CreateLockFileStream();
				return LockResult.Success;
			} catch (DirectoryNotFoundException) {
				try {
					FileUtils.CreateDirectoryForFile(path);
					CreateLockFileStream();
					return LockResult.Success;
				} catch (Exception e) {
					ReleaseLockFileStream();
					return new LockResult.Fail(e);
				}
			} catch (IOException e) {
				return DetermineLockingProcessOrFail(e);
			} catch (Exception e) {
				ReleaseLockFileStream();
				return new LockResult.Fail(e);
			}
		}

		public LockResult LockWait(int timeout, int retryDelay) {
			for (int elapsed = 0; elapsed < timeout; elapsed += retryDelay) {
				var result = Lock();

				if (result is LockResult.HasProcess info) {
					info.Dispose();
					Thread.Sleep(retryDelay);
				}
				else {
					return result;
				}
			}

			return Lock();
		}

		public bool Unlock() {
			if (ReleaseLockFileStream()) {
				try {
					File.Delete(path);
				} catch (Exception e) {
					App.ErrorHandler.Log(e.ToString());
					return false;
				}
			}

			return true;
		}

		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		private LockResult DetermineLockingProcessOrFail(Exception originalException) {
			try {
				int pid;

				using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
					byte[] bytes = new byte[sizeof(int)];
					fileStream.Read(bytes, 0, bytes.Length);
					pid = BitConverter.ToInt32(bytes, 0);
				}

				try {
					var foundProcess = Process.GetProcessById(pid);
					using var currentProcess = Process.GetCurrentProcess();

					if (currentProcess.MainModule.FileVersionInfo.InternalName == foundProcess.MainModule.FileVersionInfo.InternalName) {
						return new LockResult.HasProcess(foundProcess);
					}
					else {
						foundProcess.Close();
					}
				} catch {
					// GetProcessById throws ArgumentException if the process is missing
					// Process.MainModule can throw exceptions in some cases
				}

				return new LockResult.Fail(originalException);
			} catch (Exception e) {
				return new LockResult.Fail(e);
			}
		}
	}
}
