using CefSharp;
using CefSharp.WinForms;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;

namespace TweetDuck.Core.Handling {
    class JavaScriptDialogHandler : IJsDialogHandler{
        bool IJsDialogHandler.OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage){
            ((ChromiumWebBrowser)browserControl).InvokeSafe(() => {
                FormMessage form = new FormMessage(Program.BrandName, messageText, MessageBoxIcon.None);
                TextBox input = null;

                if (dialogType == CefJsDialogType.Alert){
                    form.AddButton("OK", ControlType.Accept | ControlType.Focused);
                }
                else if (dialogType == CefJsDialogType.Confirm){
                    form.AddButton("No", DialogResult.No, ControlType.Cancel);
                    form.AddButton("Yes", ControlType.Focused);
                }
                else if (dialogType == CefJsDialogType.Prompt){
                    form.AddButton("Cancel", DialogResult.Cancel, ControlType.Cancel);
                    form.AddButton("OK", ControlType.Accept | ControlType.Focused);

                    input = new TextBox{
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                        Location = new Point(27, form.ActionPanelY-46),
                        Size = new Size(form.ClientSize.Width-54, 20)
                    };

                    form.Controls.Add(input);
                    form.ActiveControl = input;
                    form.Height += input.Size.Height+input.Margin.Vertical;
                }

                bool success = form.ShowDialog() == DialogResult.OK;

                if (input == null){
                    callback.Continue(success);
                }
                else{
                    callback.Continue(success, input.Text);
                    input.Dispose();
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
