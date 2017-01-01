using CefSharp;
using CefSharp.WinForms;
using System.Windows.Forms;
using TweetDck.Core.Controls;
using TweetDck.Core.Other;

namespace TweetDck.Core.Handling {
    class JavaScriptDialogHandler : IJsDialogHandler{
        bool IJsDialogHandler.OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage){
            if (dialogType != CefJsDialogType.Alert && dialogType != CefJsDialogType.Confirm){
                return false;
            }

            ((ChromiumWebBrowser)browserControl).InvokeSafe(() => {
                FormMessage form = new FormMessage(Program.BrandName, messageText, MessageBoxIcon.None);
                Button btnConfirm;

                if (dialogType == CefJsDialogType.Alert){
                    btnConfirm = form.AddButton("OK");
                }
                else if (dialogType == CefJsDialogType.Confirm){
                    form.AddButton("No");
                    btnConfirm = form.AddButton("Yes");
                }
                else{
                    return;
                }

                if (form.ShowDialog() == DialogResult.OK && form.ClickedButton == btnConfirm){
                    callback.Continue(true);
                }
                else{
                    callback.Continue(false);
                }

                form.Dispose();
            });

            return true;
        }

        bool IJsDialogHandler.OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback){
            return false;
        }

        void IJsDialogHandler.OnResetDialogState(IWebBrowser browserControl, IBrowser browser){}
        void IJsDialogHandler.OnDialogClosed(IWebBrowser browserControl, IBrowser browser){}
    }
}
