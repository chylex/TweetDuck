using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CefSharp;

namespace TweetDuck.Core.Handling.General{
    sealed class FileDialogHandler : IDialogHandler{
        public bool OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, CefFileDialogFlags flags, string title, string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback){
            if (mode == CefFileDialogMode.Open || mode == CefFileDialogMode.OpenMultiple){
                string allFilters = string.Join(";", acceptFilters.Select(filter => "*"+filter));

                using(OpenFileDialog dialog = new OpenFileDialog{
                    AutoUpgradeEnabled = true,
                    DereferenceLinks = true,
                    Multiselect = mode == CefFileDialogMode.OpenMultiple,
                    Title = "Open Files",
                    Filter = $"All Supported Formats ({allFilters})|{allFilters}|All Files (*.*)|*.*"
                }){
                    if (dialog.ShowDialog() == DialogResult.OK){
                        string ext = Path.GetExtension(dialog.FileName);
                        callback.Continue(acceptFilters.FindIndex(filter => filter.Equals(ext, StringComparison.OrdinalIgnoreCase)), dialog.FileNames.ToList());
                    }
                    else{
                        callback.Cancel();
                    }

                    callback.Dispose();
                }

                return true;
            }
            else{
                callback.Dispose();
                return false;
            }
        }
    }
}
