﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace TweetLib.Utils.Static {
	public static class WebUtils {
		public static string DefaultUserAgent { get; set; } = "";

		private static bool hasMicrosoftBeenBroughtTo2008Yet;
		private static bool hasSystemProxyBeenEnabled;

		private static void EnsureModernTLS() {
			if (!hasMicrosoftBeenBroughtTo2008Yet) {
				#pragma warning disable CS0618
				ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
				ServicePointManager.SecurityProtocol &= ~(SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11);
				#pragma warning restore CS0618
				hasMicrosoftBeenBroughtTo2008Yet = true;
			}
		}

		private static bool UseSystemProxy { get; set; } = false;

		public static void EnableSystemProxy() {
			if (!hasSystemProxyBeenEnabled) {
				UseSystemProxy = true;
				hasSystemProxyBeenEnabled = true;
			}
		}

		public static WebClient NewClient(string? userAgent = null) {
			EnsureModernTLS();

			WebClient client = new WebClient();

			if (!UseSystemProxy) {
				client.Proxy = null;
			}

			client.Headers[HttpRequestHeader.UserAgent] = userAgent ?? DefaultUserAgent;
			return client;
		}

		public static AsyncCompletedEventHandler FileDownloadCallback(string file, Action? onSuccess, Action<Exception>? onFailure) {
			return (_, args) => {
				if (args.Cancelled) {
					TryDeleteFile(file);
				}
				else if (args.Error != null) {
					TryDeleteFile(file);
					onFailure?.Invoke(args.Error);
				}
				else {
					onSuccess?.Invoke();
				}
			};
		}

		private static void TryDeleteFile(string file) {
			try {
				File.Delete(file);
			} catch {
				// didn't want it deleted anyways
			}
		}
	}
}
