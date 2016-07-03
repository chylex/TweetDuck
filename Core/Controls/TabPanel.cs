using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TweetDck.Core.Controls{
    sealed partial class TabPanel : UserControl{
        public IEnumerable<TabButton> Buttons{
            get{
                return panelButtons.Controls.Cast<TabButton>();
            }
        }

        public TabButton ActiveButton { get; private set; }

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public Panel Content{
            get{
                return panelContent;
            }
        }

        private int btnWidth;

        public TabPanel(){
            InitializeComponent();
        }

        public void SetupTabPanel(int buttonWidth){
            this.btnWidth = buttonWidth;
        }

        public TabButton AddButton(string title, Action callback){
            TabButton button = new TabButton();
            button.SetupButton((btnWidth-1)*panelButtons.Controls.Count,btnWidth,title,callback);
            button.Click += (sender, args) => SelectTab((TabButton)sender);

            panelButtons.Controls.Add(button);
            return button;
        }

        public void SelectTab(TabButton button){
            if (ActiveButton != null){
                ActiveButton.BackColor = SystemColors.Control;
            }

            button.BackColor = Color.White;
            button.Callback();

            ActiveButton = button;
        }

        public void ReplaceContent(Control newControl){
            newControl.Dock = DockStyle.Fill;
            Content.SuspendLayout();
            Content.Controls.Clear();
            Content.Controls.Add(newControl);
            Content.ResumeLayout(true);
        }
    }
}
