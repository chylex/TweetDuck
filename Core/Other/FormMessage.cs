using System;
using System.Drawing;
using System.Windows.Forms;

namespace TweetDck.Core.Other{
    sealed partial class FormMessage : Form{
        public Button ClickedButton { get; private set; }

        private readonly Icon icon;
        private readonly bool isReady;

        private int realFormWidth, minFormWidth;
        private int buttonCount;
        private int prevLabelWidth, prevLabelHeight;
        private bool wasLabelMultiline;

        public FormMessage(string caption, string text, MessageBoxIcon messageIcon){
            InitializeComponent();

            this.prevLabelWidth = labelMessage.Width;
            this.prevLabelHeight = labelMessage.Height;
            this.minFormWidth = 18;

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
                    labelMessage.Location = new Point(labelMessage.Location.X-37, labelMessage.Location.Y);
                    break;
            }

            this.isReady = true;

            this.Text = caption;
            this.labelMessage.Text = text;
        }

        public Button AddButton(string title, DialogResult result = DialogResult.OK){
            Button button = new Button{
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Font = SystemFonts.MessageBoxFont,
                Location = new Point(Width-112-buttonCount*96, 12),
                Size = new Size(88, 26),
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

            minFormWidth += 96;
            Width = Math.Max(realFormWidth, minFormWidth);

            ++buttonCount;
            return button;
        }

        public void AddActionControl(Control control){
            panelActions.Controls.Add(control);

            minFormWidth += control.Width+control.Margin.Horizontal;
            Width = Math.Max(realFormWidth, minFormWidth);
        }

        private void labelMessage_SizeChanged(object sender, EventArgs e){
            if (!isReady){
                return;
            }

            bool isMultiline = labelMessage.Height > labelMessage.MinimumSize.Height;

            if (isMultiline && !wasLabelMultiline){
                labelMessage.Location = new Point(labelMessage.Location.X, labelMessage.Location.Y-8);
                prevLabelHeight += 8;
            }
            else if (!isMultiline && wasLabelMultiline){
                labelMessage.Location = new Point(labelMessage.Location.X, labelMessage.Location.Y+8);
                prevLabelHeight -= 8;
            }

            realFormWidth = Width-(icon == null ? 32+35+(labelMessage.Margin.Left-labelMessage.Margin.Right) : 0)+labelMessage.Margin.Right+labelMessage.Width-prevLabelWidth;
            Width = Math.Max(realFormWidth, minFormWidth);
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
