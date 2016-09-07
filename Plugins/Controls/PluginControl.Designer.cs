namespace TweetDck.Plugins.Controls {
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
            this.btnToggleState = new System.Windows.Forms.Button();
            this.labelName = new System.Windows.Forms.Label();
            this.panelDescription = new System.Windows.Forms.Panel();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.flowLayoutInfo = new System.Windows.Forms.FlowLayoutPanel();
            this.labelWebsite = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.btnOpenConfig = new System.Windows.Forms.Button();
            this.panelDescription.SuspendLayout();
            this.flowLayoutInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnToggleState
            // 
            this.btnToggleState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToggleState.Location = new System.Drawing.Point(459, 80);
            this.btnToggleState.Name = "btnToggleState";
            this.btnToggleState.Size = new System.Drawing.Size(65, 23);
            this.btnToggleState.TabIndex = 0;
            this.btnToggleState.Text = "Disable";
            this.btnToggleState.UseVisualStyleBackColor = true;
            this.btnToggleState.Click += new System.EventHandler(this.btnToggleState_Click);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelName.Location = new System.Drawing.Point(7, 7);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(61, 24);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            // 
            // panelDescription
            // 
            this.panelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDescription.AutoScroll = true;
            this.panelDescription.Controls.Add(this.labelDescription);
            this.panelDescription.Location = new System.Drawing.Point(11, 35);
            this.panelDescription.Name = "panelDescription";
            this.panelDescription.Size = new System.Drawing.Size(513, 39);
            this.panelDescription.TabIndex = 2;
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
            this.labelDescription.Size = new System.Drawing.Size(13, 39);
            this.labelDescription.TabIndex = 3;
            this.labelDescription.Text = "a\r\nb\r\nc";
            // 
            // labelAuthor
            // 
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Location = new System.Drawing.Point(3, 0);
            this.labelAuthor.Margin = new System.Windows.Forms.Padding(3, 0, 32, 0);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(38, 13);
            this.labelAuthor.TabIndex = 3;
            this.labelAuthor.Text = "Author";
            // 
            // flowLayoutInfo
            // 
            this.flowLayoutInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutInfo.Controls.Add(this.labelAuthor);
            this.flowLayoutInfo.Controls.Add(this.labelWebsite);
            this.flowLayoutInfo.Location = new System.Drawing.Point(11, 85);
            this.flowLayoutInfo.Name = "flowLayoutInfo";
            this.flowLayoutInfo.Size = new System.Drawing.Size(368, 18);
            this.flowLayoutInfo.TabIndex = 4;
            this.flowLayoutInfo.WrapContents = false;
            // 
            // labelWebsite
            // 
            this.labelWebsite.AutoSize = true;
            this.labelWebsite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelWebsite.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelWebsite.ForeColor = System.Drawing.Color.Blue;
            this.labelWebsite.Location = new System.Drawing.Point(76, 0);
            this.labelWebsite.Name = "labelWebsite";
            this.labelWebsite.Size = new System.Drawing.Size(46, 13);
            this.labelWebsite.TabIndex = 5;
            this.labelWebsite.Text = "Website";
            this.labelWebsite.Click += new System.EventHandler(this.labelWebsite_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Location = new System.Drawing.Point(14, 12);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(513, 13);
            this.labelVersion.TabIndex = 5;
            this.labelVersion.Text = "Version";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnOpenConfig
            // 
            this.btnOpenConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenConfig.Location = new System.Drawing.Point(385, 80);
            this.btnOpenConfig.Name = "btnOpenConfig";
            this.btnOpenConfig.Size = new System.Drawing.Size(68, 23);
            this.btnOpenConfig.TabIndex = 6;
            this.btnOpenConfig.Text = "Configure";
            this.btnOpenConfig.UseVisualStyleBackColor = true;
            this.btnOpenConfig.Click += new System.EventHandler(this.btnOpenConfig_Click);
            // 
            // PluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOpenConfig);
            this.Controls.Add(this.flowLayoutInfo);
            this.Controls.Add(this.panelDescription);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.btnToggleState);
            this.Controls.Add(this.labelVersion);
            this.MaximumSize = new System.Drawing.Size(65535, 109);
            this.MinimumSize = new System.Drawing.Size(0, 61);
            this.Name = "PluginControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(530, 109);
            this.panelDescription.ResumeLayout(false);
            this.panelDescription.PerformLayout();
            this.flowLayoutInfo.ResumeLayout(false);
            this.flowLayoutInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button btnOpenConfig;
    }
}
