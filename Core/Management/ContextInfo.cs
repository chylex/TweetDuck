using CefSharp;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Management{
    sealed class ContextInfo{
        public LinkInfo Link { get; private set; }
        public ChirpInfo Chirp { get; private set; }
        
        public ContextInfo(){
            Reset();
        }
        
        public void SetLink(string type, string url){
            Link = string.IsNullOrEmpty(url) ? new LinkInfo() : new LinkInfo(type, url);
        }

        public void SetChirp(string tweetUrl, string quoteUrl, string chirpAuthors, string chirpImages){
            Chirp = new ChirpInfo(tweetUrl, quoteUrl, chirpAuthors, chirpImages);
        }

        public void Reset(){
            Link = new LinkInfo();
            Chirp = new ChirpInfo();
        }

        // Data structures

        public enum LinkType{
            Unknown, Generic, Image, Video
        }

        public struct LinkInfo{
            public LinkType Type { get; }

            public string Url { get; }
            public string UnsafeUrl { get; }

            public LinkInfo(string type, string url){
                switch(type){
                    case "link":  Type = LinkType.Generic; break;
                    case "image": Type = LinkType.Image;   break;
                    case "video": Type = LinkType.Video;   break;
                    default:      Type = LinkType.Unknown; break;
                }

                Url = url;
                UnsafeUrl = url;
            }

            public LinkInfo(IContextMenuParams parameters){
                ContextMenuType type = parameters.TypeFlags;

                if (type.HasFlag(ContextMenuType.Media) && parameters.HasImageContents){
                    Type = LinkType.Image;
                    Url = parameters.SourceUrl;
                    UnsafeUrl = parameters.SourceUrl;
                }
                else if (type.HasFlag(ContextMenuType.Link)){
                    Type = LinkType.Generic;
                    Url = parameters.LinkUrl;
                    UnsafeUrl = parameters.UnfilteredLinkUrl;
                }
                else{
                    Type = LinkType.Unknown;
                    Url = string.Empty;
                    UnsafeUrl = string.Empty;
                }
            }
        }

        public struct ChirpInfo{
            public string TweetUrl { get; }
            public string QuoteUrl { get; }

            public string[] Authors => chirpAuthors?.Split(';') ?? StringUtils.EmptyArray;
            public string[] Images => chirpImages?.Split(';') ?? StringUtils.EmptyArray;
            
            private readonly string chirpAuthors;
            private readonly string chirpImages;

            public ChirpInfo(string tweetUrl, string quoteUrl, string chirpAuthors, string chirpImages){
                this.TweetUrl = tweetUrl;
                this.QuoteUrl = quoteUrl;
                this.chirpAuthors = chirpAuthors;
                this.chirpImages = chirpImages;
            }
        }
    }
}
