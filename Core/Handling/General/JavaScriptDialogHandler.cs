using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Other;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Handling.General{
    class JavaScriptDialogHandler : IJsDialogHandler{
        bool IJsDialogHandler.OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage){
            ((ChromiumWebBrowser)browserControl).InvokeSafe(() => {
                FormMessage form;
                TextBox input = null;

                if (dialogType == CefJsDialogType.Alert){
                    form = new FormMessage("Browser Message", messageText, MessageBoxIcon.None);
                    form.AddButton(FormMessage.OK, ControlType.Accept | ControlType.Focused);
                }
                else if (dialogType == CefJsDialogType.Confirm){
                    form = new FormMessage("Browser Confirmation", messageText, MessageBoxIcon.None);
                    form.AddButton(FormMessage.No, DialogResult.No, ControlType.Cancel);
                    form.AddButton(FormMessage.Yes, ControlType.Focused);
                }
                else if (dialogType == CefJsDialogType.Prompt){
                    form = new FormMessage("Browser Prompt", messageText, MessageBoxIcon.None);
                    form.AddButton(FormMessage.Cancel, DialogResult.Cancel, ControlType.Cancel);
                    form.AddButton(FormMessage.OK, ControlType.Accept | ControlType.Focused);

                    float dpiScale = form.GetDPIScale();

                    input = new TextBox{
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                        Location = new Point(BrowserUtils.Scale(22, dpiScale), form.ActionPanelY-BrowserUtils.Scale(46, dpiScale)),
                        Size = new Size(form.ClientSize.Width-BrowserUtils.Scale(44, dpiScale), 20)
                    };

                    form.Controls.Add(input);
                    form.ActiveControl = input;
                    form.Height += input.Size.Height+input.Margin.Vertical;
                }
                else{
                    callback.Continue(false);
                    return;
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
