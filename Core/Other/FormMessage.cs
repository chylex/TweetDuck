using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Core.Controls;
using TweetDuck.Core.Utils;

namespace TweetDuck.Core.Other{
    sealed partial class FormMessage : Form{
        public Button ClickedButton { get; private set; }

        public int ActionPanelY => panelActions.Location.Y;

        private int ClientWidth{
            get => ClientSize.Width;
            set => ClientSize = new Size(value, ClientSize.Height);
        }

        private int ButtonDistance{
            get => BrowserUtils.Scale(96, dpiScale);
        }

        private readonly Icon icon;
        private readonly bool isReady;
        private readonly float dpiScale;

        private int realFormWidth, minFormWidth;
        private int buttonCount;
        private int prevLabelWidth, prevLabelHeight;
        private bool wasLabelMultiline;

        public FormMessage(string caption, string text, MessageBoxIcon messageIcon){
            InitializeComponent();

            this.dpiScale = this.GetDPIScale();

            this.prevLabelWidth = labelMessage.Width;
            this.prevLabelHeight = labelMessage.Height;
            this.minFormWidth = BrowserUtils.Scale(40, dpiScale);

            switch(messageIcon){
                case MessageBoxIcon.Information:
                    icon = SystemIcons.Information;
                    break;

                case MessageBoxIcon.Warning:
                    icon = SystemIcons.Warning;
                    break;
                    
                case MessageBoxIcon.Error:
                    icon = SystemIcons.Error;
                    break;

                case MessageBoxIcon.Question:
                    icon = SystemIcons.Question;
                    break;

                default:
                    icon = null;
                    labelMessage.Location = new Point(labelMessage.Location.X-38, labelMessage.Location.Y);
                    break;
            }

            this.isReady = true;

            this.Text = caption;
            this.labelMessage.Text = text.Replace("\n", Environment.NewLine); // TODO replace all \r\n
        }

        private void FormMessage_SizeChanged(object sender, EventArgs e){
            RecalculateButtonLocation();
        }

        public Button AddButton(string title, DialogResult result = DialogResult.OK){
            Button button = new Button{
                Anchor = AnchorStyles.Bottom,
                Font = SystemFonts.MessageBoxFont,
                Location = new Point(0, 12),
                Size = new Size(BrowserUtils.Scale(88, dpiScale), BrowserUtils.Scale(26, dpiScale)),
                TabIndex = buttonCount,
                Text = title,
                UseVisualStyleBackColor = true
            };

            button.Click += (sender, args) => {
                ClickedButton = (Button)sender;
                DialogResult = result;
                Close();
            };

            panelActions.Controls.Add(button);
            ++buttonCount;

            minFormWidth += ButtonDistance;
            ClientWidth = Math.Max(realFormWidth, minFormWidth);
            RecalculateButtonLocation();

            return button;
        }

        public void AddActionControl(Control control){
            panelActions.Controls.Add(control);
            
            control.Size = new Size(BrowserUtils.Scale(control.Width, dpiScale), BrowserUtils.Scale(control.Height, dpiScale));

            minFormWidth += control.Width+control.Margin.Horizontal;
            ClientWidth = Math.Max(realFormWidth, minFormWidth);
        }
        
        private void RecalculateButtonLocation(){
            int dist = ButtonDistance;
            int start = ClientWidth-dist-BrowserUtils.Scale(1, dpiScale);

            for(int index = 0; index < buttonCount; index++){
                Control control = panelActions.Controls[index];
                control.Location = new Point(start-index*dist, control.Location.Y);
            }
        }

        private void labelMessage_SizeChanged(object sender, EventArgs e){
            if (!isReady){
                return;
            }

            bool isMultiline = labelMessage.Height > labelMessage.MinimumSize.Height;
            int labelOffset = BrowserUtils.Scale(8, dpiScale);

            if (isMultiline && !wasLabelMultiline){
                labelMessage.Location = new Point(labelMessage.Location.X, labelMessage.Location.Y-labelOffset);
                prevLabelHeight += labelOffset;
            }
            else if (!isMultiline && wasLabelMultiline){
                labelMessage.Location = new Point(labelMessage.Location.X, labelMessage.Location.Y+labelOffset);
                prevLabelHeight -= labelOffset;
            }

            realFormWidth = ClientWidth-(icon == null ? 50 : 0)+labelMessage.Width-prevLabelWidth;
            ClientWidth = Math.Max(realFormWidth, minFormWidth);
            Height += labelMessage.Height-prevLabelHeight;

            prevLabelWidth = labelMessage.Width;
            prevLabelHeight = labelMessage.Height;
            wasLabelMultiline = isMultiline;
        }

        protected override void OnPaint(PaintEventArgs e){
            if (icon != null){
                e.Graphics.DrawIcon(icon, 25, 26);
            }

            base.OnPaint(e);
        }
    }
}
