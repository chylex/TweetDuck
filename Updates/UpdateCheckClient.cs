using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TweetDuck.Core.Utils;
using JsonObject = System.Collections.Generic.IDictionary<string, object>;

namespace TweetDuck.Updates{
    sealed class UpdateCheckClient{
        private const string ApiLatestRelease = "https://api.github.com/repos/chylex/TweetDuck/releases/latest";
        private const string UpdaterAssetName = "TweetDuck.Update.exe";

        private readonly UpdaterSettings settings;

        public UpdateCheckClient(UpdaterSettings settings){
            this.settings = settings;
        }

        public Task<UpdateInfo> Check(){
            TaskCompletionSource<UpdateInfo> result = new TaskCompletionSource<UpdateInfo>();

            WebClient client = BrowserUtils.CreateWebClient();
            client.Headers[HttpRequestHeader.Accept] = "application/vnd.github.v3+json"; // TODO could use .html to avoid custom markdown parsing

            client.DownloadStringTaskAsync(ApiLatestRelease).ContinueWith(task => {
                if (task.IsCanceled){
                    result.SetCanceled();
                }
                else if (task.IsFaulted){
                    result.SetException(task.Exception.InnerException);
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
            bool IsUpdaterAsset(JsonObject obj){
                return UpdaterAssetName == (string)obj["name"];
            }

            string AssetDownloadUrl(JsonObject obj){
                return (string)obj["browser_download_url"];
            }
            
            JsonObject root = (JsonObject)new JavaScriptSerializer().DeserializeObject(json);

            string versionTag = (string)root["tag_name"];
            string releaseNotes = (string)root["body"];
            string downloadUrl = ((Array)root["assets"]).Cast<JsonObject>().Where(IsUpdaterAsset).Select(AssetDownloadUrl).FirstOrDefault();

            return new UpdateInfo(settings, versionTag, releaseNotes, downloadUrl);
        }
    }
}
