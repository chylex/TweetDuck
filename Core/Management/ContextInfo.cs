﻿using CefSharp;

namespace TweetDuck.Core.Management{
    sealed class ContextInfo{
        public LinkInfo Link { get; private set; }
        public ChirpInfo Chirp { get; private set; }
        
        public ContextInfo(){
            Reset();
        }
        
        public void SetLink(string type, string url){
            Link = new LinkInfo(string.IsNullOrEmpty(url) ? null : type, url);
        }

        public void SetChirp(string tweetUrl, string quoteUrl, string chirpAuthors, string chirpImages){
            Chirp = new ChirpInfo(tweetUrl, quoteUrl, chirpAuthors, chirpImages);
        }

        public void Reset(){
            Link = new LinkInfo();
            Chirp = new ChirpInfo();
        }

        // Data structures

        public struct LinkInfo{
            public bool IsLink => type == "link";
            public bool IsImage => type == "image";
            public bool IsVideo => type == "video";

            public string GetUrl(IContextMenuParams parameters, bool safe){
                return IsLink ? url : (safe ? parameters.LinkUrl : parameters.UnfilteredLinkUrl);
            }

            public string GetMediaSource(IContextMenuParams parameters){
                return IsImage || IsVideo ? url : parameters.SourceUrl;
            }

            private readonly string type;
            private readonly string url;

            public LinkInfo(string type, string url){
                this.type = type;
                this.url = url;
            }
        }

        public struct ChirpInfo{
            public string TweetUrl { get; }
            public string QuoteUrl { get; }

            public string[] Authors => chirpAuthors.Split(';');
            public string[] Images => chirpImages.Split(';');
            
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
