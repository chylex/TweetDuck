using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TweetDuck.Utils;
using TweetLib.Core.Systems.Updates;
using TweetLib.Core.Utils;
using JsonObject = System.Collections.Generic.IDictionary<string, object>;

namespace TweetDuck.Updates{
    sealed class UpdateCheckClient : IUpdateCheckClient{
        private const string ApiLatestRelease = "https://api.github.com/repos/chylex/TweetDuck/releases/latest";
        private const string UpdaterAssetName = "TweetDuck.Update.exe";

        private readonly string installerFolder;

        public UpdateCheckClient(string installerFolder){
            this.installerFolder = installerFolder;
        }

        bool IUpdateCheckClient.CanCheck => Program.Config.User.EnableUpdateCheck;

        Task<UpdateInfo> IUpdateCheckClient.Check(){
            TaskCompletionSource<UpdateInfo> result = new TaskCompletionSource<UpdateInfo>();

            WebClient client = WebUtils.NewClient(BrowserUtils.UserAgentVanilla);
            client.Headers[HttpRequestHeader.Accept] = "application/vnd.github.v3+json";

            client.DownloadStringTaskAsync(ApiLatestRelease).ContinueWith(task => {
                if (task.IsCanceled){
                    result.SetCanceled();
                }
                else if (task.IsFaulted){
                    result.SetException(ExpandWebException(task.Exception.InnerException));
                }
                else{
                    try{
                        result.SetResult(ParseFromJson(task.Result));
                    }catch(Exception e){
                        result.SetException(e);
                    }
                }
            });
            
            return result.Task;
        }

        private UpdateInfo ParseFromJson(string json){
            static bool IsUpdaterAsset(JsonObject obj){
                return UpdaterAssetName == (string)obj["name"];
            }

            static string AssetDownloadUrl(JsonObject obj){
                return (string)obj["browser_download_url"];
            }
            
            JsonObject root = (JsonObject)new JavaScriptSerializer().DeserializeObject(json);

            string versionTag = (string)root["tag_name"];
            string releaseNotes = (string)root["body"];
            string downloadUrl = ((Array)root["assets"]).Cast<JsonObject>().Where(IsUpdaterAsset).Select(AssetDownloadUrl).FirstOrDefault();

            return new UpdateInfo(versionTag, releaseNotes, downloadUrl, installerFolder);
        }

        private static Exception ExpandWebException(Exception e){
            if (e is WebException we && we.Response is HttpWebResponse response){
                try{
                    using var stream = response.GetResponseStream();
                    using var reader = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet ?? "utf-8"));
                    return new Reporter.ExpandedLogException(e, reader.ReadToEnd());
                }catch{
                    // whatever
                }
            }

            return e;
        }
    }
}
