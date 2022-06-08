using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TweetLib.Core.Systems.Updates;
using TweetLib.Utils.Static;

namespace TweetDuck.Updates {
	sealed class UpdateCheckClient : IUpdateCheckClient {
		private const string ApiLatestRelease = "https://api.github.com/repos/chylex/TweetDuck/releases/latest";
		private const string UpdaterAssetName = "TweetDuck.Update.exe";

		private readonly string installerFolder;

		public UpdateCheckClient(string installerFolder) {
			this.installerFolder = installerFolder;
		}

		bool IUpdateCheckClient.CanCheck => Program.Config.User.EnableUpdateCheck;

		Task<UpdateInfo> IUpdateCheckClient.Check() {
			var result = new TaskCompletionSource<UpdateInfo>();

			WebClient client = WebUtils.NewClient();
			client.Headers[HttpRequestHeader.Accept] = "application/vnd.github.v3+json";

			client.DownloadStringTaskAsync(ApiLatestRelease).ContinueWith(task => {
				if (task.IsCanceled) {
					result.SetCanceled();
				}
				else if (task.IsFaulted) {
					result.SetException(ExpandWebException(task.Exception!.InnerException));
				}
				else {
					try {
						result.SetResult(ParseFromJson(task.Result));
					} catch (Exception e) {
						result.SetException(e);
					}
				}
			});

			return result.Task;
		}

		private UpdateInfo ParseFromJson(string json) {
			static bool IsUpdaterAsset(JsonElement obj) {
				return UpdaterAssetName == obj.GetProperty("name").GetString();
			}

			static string AssetDownloadUrl(JsonElement obj) {
				return obj.GetProperty("browser_download_url").GetString()!;
			}

			var root = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

			string versionTag = root["tag_name"].GetString()!;
			string releaseNotes = root["body"].GetString()!;
			string? downloadUrl = root["assets"].EnumerateArray().Where(IsUpdaterAsset).Select(AssetDownloadUrl).FirstOrDefault();

			return new UpdateInfo(versionTag, releaseNotes, downloadUrl, installerFolder);
		}

		private static Exception ExpandWebException(Exception? e) {
			if (e is WebException { Response: HttpWebResponse response } ) {
				try {
					using var stream = response.GetResponseStream();
					using var reader = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet ?? "utf-8"));
					return new Reporter.ExpandedLogException(e, reader.ReadToEnd());
				} catch {
					// whatever
				}
			}

			return e!;
		}
	}
}
