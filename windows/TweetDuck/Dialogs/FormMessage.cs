using System;
using System.Drawing;
using System.Windows.Forms;
using TweetDuck.Controls;
using TweetDuck.Utils;

namespace TweetDuck.Dialogs {
	[Flags]
	enum ControlType {
		None = 0,
		Accept = 1, // triggered by pressing enter when a non-button is focused
		Cancel = 2, // triggered by closing the dialog without pressing a button
		Focused = 4 // active control after the dialog is showed
	}

	sealed partial class FormMessage : Form {
		public const string OK = "OK";
		public const string Yes = "Yes";
		public const string No = "No";
		public const string Cancel = "Cancel";
		public const string Retry = "Retry";
		public const string Ignore = "Ignore";
		public const string Exit = "Exit";

		public static bool Information(string caption, string text, string buttonAccept, string? buttonCancel = null) {
			return Show(caption, text, MessageBoxIcon.Information, buttonAccept, buttonCancel);
		}

		public static bool Warning(string caption, string text, string buttonAccept, string? buttonCancel = null) {
			return Show(caption, text, MessageBoxIcon.Warning, buttonAccept, buttonCancel);
		}

		public static bool Error(string caption, string text, string buttonAccept, string? buttonCancel = null) {
			return Show(caption, text, MessageBoxIcon.Error, buttonAccept, buttonCancel);
		}

		public static bool Question(string caption, string text, string buttonAccept, string? buttonCancel = null) {
			return Show(caption, text, MessageBoxIcon.Question, buttonAccept, buttonCancel);
		}

		public static bool Show(string caption, string text, MessageBoxIcon icon, string buttonAccept, string? buttonCancel = null) {
			using FormMessage message = new FormMessage(caption, text, icon);

			if (buttonCancel == null) {
				message.AddButton(buttonAccept, DialogResult.OK, ControlType.Cancel | ControlType.Focused);
			}
			else {
				message.AddButton(buttonCancel, DialogResult.Cancel, ControlType.Cancel);
				message.AddButton(buttonAccept, DialogResult.OK, ControlType.Accept | ControlType.Focused);
			}

			return message.ShowDialog() == DialogResult.OK;
		}

		// Instance

		public Button? ClickedButton { get; private set; }

		public bool HasIcon => icon != null;
		public int ActionPanelY => panelActions.Location.Y;

		private int ClientWidth {
			get => ClientSize.Width;
			set => ClientSize = ClientSize with { Width = value };
		}

		private int ButtonDistance {
			get => BrowserUtils.Scale(96, dpiScale);
		}

		private readonly Icon? icon;
		private readonly bool isReady;
		private readonly float dpiScale;

		private int realFormWidth, minFormWidth;
		private int buttonCount;
		private int prevLabelWidth, prevLabelHeight;
		private bool wasLabelMultiline;

		public FormMessage(string caption, string text, MessageBoxIcon messageIcon) {
			InitializeComponent();

			this.dpiScale = this.GetDPIScale();

			this.prevLabelWidth = labelMessage.Width;
			this.prevLabelHeight = labelMessage.Height;
			this.minFormWidth = BrowserUtils.Scale(42, dpiScale);

			switch (messageIcon) {
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
					labelMessage.Location = labelMessage.Location with { X = BrowserUtils.Scale(19, dpiScale) }; // 19 instead of 9 due to larger height
					break;
			}

			this.isReady = true;

			this.Text = caption;
			this.labelMessage.Text = text.Replace("\r", "").Replace("\n", Environment.NewLine);
		}

		private void FormMessage_SizeChanged(object? sender, EventArgs e) {
			RecalculateButtonLocation();
		}

		public Button AddButton(string title, ControlType type) {
			return AddButton(title, DialogResult.OK, type);
		}

		public Button AddButton(string title, DialogResult result = DialogResult.OK, ControlType type = ControlType.None) {
			Button button = CreateButton(title);

			button.Click += (_, _) => {
				ClickedButton = button;
				DialogResult = result;
				Close();
			};

			panelActions.Controls.Add(button);
			++buttonCount;

			minFormWidth += ButtonDistance;
			ClientWidth = Math.Max(realFormWidth, minFormWidth);
			RecalculateButtonLocation();

			if (type.HasFlag(ControlType.Accept)) {
				AcceptButton = button;
			}

			if (type.HasFlag(ControlType.Cancel)) {
				CancelButton = button;
			}

			if (type.HasFlag(ControlType.Focused)) {
				ActiveControl = button;
			}

			return button;
		}

		public Button CreateButton(string title, int x = 0, int width = 88) {
			return new Button {
				Anchor = AnchorStyles.Bottom,
				Font = SystemFonts.MessageBoxFont,
				Location = new Point(x, 12),
				Size = new Size(BrowserUtils.Scale(width, dpiScale), BrowserUtils.Scale(26, dpiScale)),
				TabIndex = 256 - buttonCount,
				Text = title,
				UseVisualStyleBackColor = true
			};
		}

		public void AddActionControl(Control control) {
			panelActions.Controls.Add(control);

			control.Size = new Size(BrowserUtils.Scale(control.Width, dpiScale), BrowserUtils.Scale(control.Height, dpiScale));

			minFormWidth += control.Width + control.Margin.Horizontal;
			ClientWidth = Math.Max(realFormWidth, minFormWidth);
		}

		private void RecalculateButtonLocation() {
			int dist = ButtonDistance;
			int start = ClientWidth - dist;

			for (int index = 0; index < buttonCount; index++) {
				Control control = panelActions.Controls[index];
				control.Location = control.Location with { X = start - index * dist };
			}
		}

		private void labelMessage_SizeChanged(object? sender, EventArgs e) {
			if (!isReady) {
				return;
			}

			bool isMultiline = labelMessage.Height > labelMessage.MinimumSize.Height;
			int labelOffset = BrowserUtils.Scale(8, dpiScale);

			if (isMultiline && !wasLabelMultiline) {
				labelMessage.Location = labelMessage.Location with { Y = labelMessage.Location.Y - labelOffset };
				prevLabelHeight += labelOffset;
			}
			else if (!isMultiline && wasLabelMultiline) {
				labelMessage.Location = labelMessage.Location with { Y = labelMessage.Location.Y + labelOffset };
				prevLabelHeight -= labelOffset;
			}

			realFormWidth = ClientWidth - (icon == null ? BrowserUtils.Scale(50, dpiScale) : 0) + labelMessage.Width - prevLabelWidth;
			ClientWidth = Math.Max(realFormWidth, minFormWidth);
			Height += labelMessage.Height - prevLabelHeight;

			prevLabelWidth = labelMessage.Width;
			prevLabelHeight = labelMessage.Height;
			wasLabelMultiline = isMultiline;
		}

		protected override void OnPaint(PaintEventArgs e) {
			if (icon != null) {
				e.Graphics.DrawIcon(icon, BrowserUtils.Scale(25, dpiScale), 1 + BrowserUtils.Scale(25, dpiScale));
			}

			base.OnPaint(e);
		}
	}
}
