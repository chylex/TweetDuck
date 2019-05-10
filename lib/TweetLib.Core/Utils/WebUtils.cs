﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace TweetLib.Core.Utils{
    public static class WebUtils{
        private static bool HasMicrosoftBeenBroughtTo2008Yet;

        private static void EnsureTLS12(){
            if (!HasMicrosoftBeenBroughtTo2008Yet){
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                ServicePointManager.SecurityProtocol &= ~(SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11);
                HasMicrosoftBeenBroughtTo2008Yet = true;
            }
        }

        public static WebClient NewClient(string userAgent){
            EnsureTLS12();

            WebClient client = new WebClient{ Proxy = null };
            client.Headers[HttpRequestHeader.UserAgent] = userAgent;
            return client;
        }

        public static AsyncCompletedEventHandler FileDownloadCallback(string file, Action? onSuccess, Action<Exception>? onFailure){
            return (sender, args) => {
                if (args.Cancelled){
                    try{
                        File.Delete(file);
                    }catch{
                        // didn't want it deleted anyways
                    }
                }
                else if (args.Error != null){
                    onFailure?.Invoke(args.Error);
                }
                else{
                    onSuccess?.Invoke();
                }
            };
        }
    }
}
