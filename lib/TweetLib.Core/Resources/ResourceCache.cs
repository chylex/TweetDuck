using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TweetLib.Browser.Request;
using IOFile = System.IO.File;

namespace TweetLib.Core.Resources {
	public sealed class ResourceCache {
		private readonly Dictionary<string, SchemeResource> cache = new ();

		public void ClearCache() {
			cache.Clear();
		}

		internal SchemeResource ReadFile(string path) {
			string key = new Uri(path).LocalPath;

			if (cache.TryGetValue(key, out var cachedResource)) {
				return cachedResource;
			}

			SchemeResource resource;
			try {
				resource = new SchemeResource.File(IOFile.ReadAllBytes(path), Path.GetExtension(path));
			} catch (FileNotFoundException) {
				resource = new SchemeResource.Status(HttpStatusCode.NotFound, "File not found.");
			} catch (DirectoryNotFoundException) {
				resource = new SchemeResource.Status(HttpStatusCode.NotFound, "Directory not found.");
			} catch (Exception e) {
				resource = new SchemeResource.Status(HttpStatusCode.InternalServerError, e.Message);
			}

			cache[key] = resource;
			return resource;
		}
	}
}
