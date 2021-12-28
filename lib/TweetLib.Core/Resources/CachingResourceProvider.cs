using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TweetLib.Browser.Interfaces;
using IOFile = System.IO.File;

namespace TweetLib.Core.Resources {
	public sealed class CachingResourceProvider<T> : IResourceProvider<T> {
		private readonly IResourceProvider<T> resourceProvider;
		private readonly Dictionary<string, ICachedResource> cache = new ();

		public CachingResourceProvider(IResourceProvider<T> resourceProvider) {
			this.resourceProvider = resourceProvider;
		}

		public void ClearCache() {
			cache.Clear();
		}

		public T Status(HttpStatusCode code, string message) {
			return resourceProvider.Status(code, message);
		}

		public T File(byte[] contents, string extension) {
			return resourceProvider.File(contents, extension);
		}

		internal T CachedFile(string path) {
			string key = new Uri(path).LocalPath;

			if (cache.TryGetValue(key, out var cachedResource)) {
				return cachedResource.GetResource(resourceProvider);
			}

			ICachedResource resource;
			try {
				resource = new CachedFileResource(IOFile.ReadAllBytes(path), Path.GetExtension(path));
			} catch (FileNotFoundException) {
				resource = new CachedStatusResource(HttpStatusCode.NotFound, "File not found.");
			} catch (DirectoryNotFoundException) {
				resource = new CachedStatusResource(HttpStatusCode.NotFound, "Directory not found.");
			} catch (Exception e) {
				resource = new CachedStatusResource(HttpStatusCode.InternalServerError, e.Message);
			}

			cache[key] = resource;
			return resource.GetResource(resourceProvider);
		}

		private interface ICachedResource {
			T GetResource(IResourceProvider<T> resourceProvider);
		}

		private sealed class CachedFileResource : ICachedResource {
			private readonly byte[] contents;
			private readonly string extension;

			public CachedFileResource(byte[] contents, string extension) {
				this.contents = contents;
				this.extension = extension;
			}

			T ICachedResource.GetResource(IResourceProvider<T> resourceProvider) {
				return resourceProvider.File(contents, extension);
			}
		}

		private sealed class CachedStatusResource : ICachedResource {
			private readonly HttpStatusCode code;
			private readonly string message;

			public CachedStatusResource(HttpStatusCode code, string message) {
				this.code = code;
				this.message = message;
			}

			public T GetResource(IResourceProvider<T> resourceProvider) {
				return resourceProvider.Status(code, message);
			}
		}
	}
}
