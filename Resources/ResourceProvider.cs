using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using CefSharp;
using TweetLib.Core.Browser;

namespace TweetDuck.Resources {
	internal sealed class ResourceProvider : IResourceProvider<IResourceHandler> {
		private readonly Dictionary<string, ICachedResource> cache = new Dictionary<string, ICachedResource>();

		public IResourceHandler Status(HttpStatusCode code, string message) {
			return CreateStatusHandler(code, message);
		}

		public IResourceHandler File(string path) {
			string key = new Uri(path).LocalPath;

			if (cache.TryGetValue(key, out var cachedResource)) {
				return cachedResource.GetResource();
			}

			cachedResource = FileWithCaching(path);
			cache[key] = cachedResource;
			return cachedResource.GetResource();
		}

		private ICachedResource FileWithCaching(string path) {
			try {
				return new CachedFile(System.IO.File.ReadAllBytes(path), Path.GetExtension(path));
			} catch (FileNotFoundException) {
				return new CachedStatus(HttpStatusCode.NotFound, "File not found.");
			} catch (DirectoryNotFoundException) {
				return new CachedStatus(HttpStatusCode.NotFound, "Directory not found.");
			} catch (Exception e) {
				return new CachedStatus(HttpStatusCode.InternalServerError, e.Message);
			}
		}

		public void ClearCache() {
			cache.Clear();
		}

		private static ResourceHandler CreateHandler(byte[] bytes) {
			var handler = ResourceHandler.FromStream(new MemoryStream(bytes), autoDisposeStream: true);
			handler.Headers.Set("Access-Control-Allow-Origin", "*");
			return handler;
		}

		private static IResourceHandler CreateFileContentsHandler(byte[] bytes, string extension) {
			if (bytes.Length == 0) {
				return CreateStatusHandler(HttpStatusCode.NoContent, "File is empty."); // FromByteArray crashes CEF internals with no contents
			}
			else {
				var handler = CreateHandler(bytes);
				handler.MimeType = Cef.GetMimeType(extension);
				return handler;
			}
		}

		private static IResourceHandler CreateStatusHandler(HttpStatusCode code, string message) {
			var handler = CreateHandler(Encoding.UTF8.GetBytes(message));
			handler.StatusCode = (int) code;
			return handler;
		}

		private interface ICachedResource {
			IResourceHandler GetResource();
		}

		private sealed class CachedFile : ICachedResource {
			private readonly byte[] bytes;
			private readonly string extension;

			public CachedFile(byte[] bytes, string extension) {
				this.bytes = bytes;
				this.extension = extension;
			}

			public IResourceHandler GetResource() {
				return CreateFileContentsHandler(bytes, extension);
			}
		}

		private sealed class CachedStatus : ICachedResource {
			private readonly HttpStatusCode code;
			private readonly string message;

			public CachedStatus(HttpStatusCode code, string message) {
				this.code = code;
				this.message = message;
			}

			public IResourceHandler GetResource() {
				return CreateStatusHandler(code, message);
			}
		}
	}
}
