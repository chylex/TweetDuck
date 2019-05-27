namespace TweetDuck.Plugins {
    partial class PluginControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.btnToggleState = new System.Windows.Forms.Button();
            this.labelName = new System.Windows.Forms.Label();
            this.panelDescription = new System.Windows.Forms.Panel();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.flowLayoutInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.labelWebsite = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.btnConfigure = new System.Windows.Forms.Button();
            this.labelType = new TweetDuck.Core.Controls.LabelVertical();
            this.timerLayout = new System.Windows.Forms.Timer(this.components);
            this.panelBorder = new System.Windows.Forms.Panel();
            this.panelDescription.SuspendLayout();
            this.flowLayoutInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnToggleState
            // 
            this.btnToggleState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleState.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnToggleState.Location = new System.Drawing.Point(451, 59);
            this.btnToggleState.Name = "btnToggleState";
            this.btnToggleState.Size = new System.Drawing.Size(70, 23);
            this.btnToggleState.TabIndex = 6;
            this.btnToggleState.Text = "Disable";
            this.btnToggleState.UseVisualStyleBackColor = true;
            this.btnToggleState.Click += new System.EventHandler(this.btnToggleState_Click);
            // 
            // labelName
            // 
            this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.labelName.Location = new System.Drawing.Point(0, 0);
            this.labelName.Margin = new System.Windows.Forms.Padding(0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(53, 21);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name";
            this.labelName.UseMnemonic = false;
            // 
            // panelDescription
            // 
            this.panelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDescription.AutoScroll = true;
            this.panelDescription.Controls.Add(this.labelDescription);
            this.panelDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.panelDescription.Location = new System.Drawing.Point(28, 33);
            this.panelDescription.Name = "panelDescription";
            this.panelDescription.Size = new System.Drawing.Size(410, 47);
            this.panelDescription.TabIndex = 4;
            this.panelDescription.Resize += new System.EventHandler(this.panelDescription_Resize);
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(0, 0);
            this.labelDescription.Margin = new System.Windows.Forms.Padding(0);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(14, 45);
            this.labelDescription.TabIndex = 0;
            this.labelDescription.Text = "a\r\nb\r\nc";
            this.labelDescription.UseMnemonic = false;
            // 
            // labelAuthor
            // 
            this.labelAuthor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.labelAuthor.Location = new System.Drawing.Point(53, 5);
            this.labelAuthor.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(44, 15);
            this.labelAuthor.TabIndex = 1;
            this.labelAuthor.Text = "Author";
            this.labelAuthor.UseMnemonic = false;
            // 
            // flowLayoutInfo
            // 
            this.flowLayoutInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutInfo.Controls.Add(this.labelName);
            this.flowLayoutInfo.Controls.Add(this.labelAuthor);
            this.flowLayoutInfo.Controls.Add(this.labelWebsite);
            this.flowLayoutInfo.Location = new System.Drawing.Point(24, 6);
            this.flowLayoutInfo.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutInfo.Name = "flowLayoutInfo";
            this.flowLayoutInfo.Size = new System.Drawing.Size(414, 21);
            this.flowLayoutInfo.TabIndex = 2;
            this.flowLayoutInfo.WrapContents = false;
            // 
            // labelWebsite
            // 
            this.labelWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelWebsite.AutoEllipsis = true;
            this.labelWebsite.AutoSize = true;
            this.labelWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelWebsite.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline);
            this.labelWebsite.ForeColor = System.Drawing.Color.Blue;
            this.labelWebsite.Location = new System.Drawing.Point(100, 5);
            this.labelWebsite.Margin = new System.Windows.Forms.Padding(3, 0, 0, 1);
            this.labelWebsite.Name = "labelWebsite";
            this.labelWebsite.Size = new System.Drawing.Size(49, 15);
            this.labelWebsite.TabIndex = 2;
            this.labelWebsite.Text = "Website";
            this.labelWebsite.UseMnemonic = false;
            this.labelWebsite.Click += new System.EventHandler(this.labelWebsite_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.labelVersion.Location = new System.Drawing.Point(88, 6);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.labelVersion.Size = new System.Drawing.Size(436, 21);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.labelVersion.UseMnemonic = false;
            // 
            // btnConfigure
            // 
            this.btnConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigure.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnConfigure.Location = new System.Drawing.Point(451, 30);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Size = new System.Drawing.Size(70, 23);
            this.btnConfigure.TabIndex = 5;
            this.btnConfigure.Text = "Configure";
            this.btnConfigure.UseVisualStyleBackColor = true;
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // labelType
            // 
            this.labelType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.labelType.BackColor = System.Drawing.Color.DarkGray;
            this.labelType.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold);
            this.labelType.LineHeight = 0;
            this.labelType.Location = new System.Drawing.Point(0, 0);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(18, 88);
            this.labelType.TabIndex = 0;
            this.labelType.Text = "TYPE";
            // 
            // timerLayout
            // 
            this.timerLayout.Interval = 1;
            this.timerLayout.Tick += new System.EventHandler(this.timerLayout_Tick);
            // 
            // panelBorder
            // 
            this.panelBorder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelBorder.BackColor = System.Drawing.Color.DimGray;
            this.panelBorder.Location = new System.Drawing.Point(18, 0);
            this.panelBorder.Margin = new System.Windows.Forms.Padding(0);
            this.panelBorder.Name = "panelBorder";
            this.panelBorder.Size = new System.Drawing.Size(1, 88);
            this.panelBorder.TabIndex = 1;
            // 
            // PluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.btnConfigure);
            this.Controls.Add(this.flowLayoutInfo);
            this.Controls.Add(this.panelBorder);
            this.Controls.Add(this.panelDescription);
            this.Controls.Add(this.btnToggleState);
            this.Controls.Add(this.labelVersion);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(65535, 88);
            this.Name = "PluginControl";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.Size = new System.Drawing.Size(530, 88);
            this.panelDescription.ResumeLayout(false);
            this.panelDescription.PerformLayout();
            this.flowLayoutInfo.ResumeLayout(false);
            this.flowLayoutInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnToggleState;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Panel panelDescription;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutInfo;
        private System.Windows.Forms.Label labelWebsite;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Button btnConfigure;
        private Core.Controls.LabelVertical labelType;
        private System.Windows.Forms.Timer timerLayout;
        private System.Windows.Forms.Panel panelBorder;
    }
}
