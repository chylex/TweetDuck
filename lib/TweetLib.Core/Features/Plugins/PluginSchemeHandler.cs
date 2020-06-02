using System;
using System.IO;
using System.Linq;
using System.Net;
using TweetLib.Core.Browser;
using TweetLib.Core.Features.Plugins.Enums;

namespace TweetLib.Core.Features.Plugins{
    public sealed class PluginSchemeHandler<T> where T : class{
        public const string Name = "tdp";

        private readonly IResourceProvider<T> resourceProvider;
        private PluginBridge? bridge = null;

        public PluginSchemeHandler(IResourceProvider<T> resourceProvider){
            this.resourceProvider = resourceProvider;
        }

        public void Setup(PluginManager plugins){
            if (this.bridge != null){
                throw new InvalidOperationException("Plugin scheme handler is already setup.");
            }

            this.bridge = plugins.bridge;
        }

        public T? Process(string url){
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Name || !int.TryParse(uri.Authority, out var identifier)){
                return null;
            }

            var segments = uri.Segments.Select(segment => segment.TrimEnd('/')).Where(segment => !string.IsNullOrEmpty(segment)).ToArray();
            
            if (segments.Length > 0){
                var handler = segments[0] switch{
                    "root" => DoReadRootFile(identifier, segments),
                    _ => null
                };

                if (handler != null){
                    return handler;
                }
            }

            return resourceProvider.Status(HttpStatusCode.BadRequest, "Bad URL path: " + uri.AbsolutePath);
        }

        private T? DoReadRootFile(int identifier, string[] segments){
            string path = string.Join("/", segments, 1, segments.Length - 1);

            Plugin? plugin = bridge?.GetPluginFromToken(identifier);
            string fullPath = plugin == null ? string.Empty : plugin.GetFullPathIfSafe(PluginFolder.Root, path);

            if (fullPath.Length == 0){
                return resourceProvider.Status(HttpStatusCode.Forbidden, "File path has to be relative to the plugin root folder.");
            }

            try{
                return resourceProvider.File(File.ReadAllBytes(fullPath), Path.GetExtension(path));
            }catch(FileNotFoundException){
                return resourceProvider.Status(HttpStatusCode.NotFound, "File not found.");
            }catch(DirectoryNotFoundException){
                return resourceProvider.Status(HttpStatusCode.NotFound, "Directory not found.");
            }catch(Exception e){
                return resourceProvider.Status(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
