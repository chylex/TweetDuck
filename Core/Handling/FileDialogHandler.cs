using CefSharp;
using System.Collections.Generic;
using TweetDck.Core.Bridge;
using TweetDck.Core.Controls;

namespace TweetDck.Core.Handling{
    class FileDialogHandler : IDialogHandler{
        private readonly FormBrowser form;

        public FileDialogHandler(FormBrowser form){
            this.form = form;
        }

        public bool OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback){
            if (!string.IsNullOrEmpty(TweetDeckBridge.ClipboardImagePath)){
                callback.Continue(selectedAcceptFilter, new List<string>{ TweetDeckBridge.ClipboardImagePath });

                form.InvokeSafe(() => {
                    TweetDeckBridge.ClipboardImagePath = string.Empty;
                });

                return true;
            }

            return false;
        }
    }
}
