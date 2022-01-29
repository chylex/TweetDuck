using TweetDuck.Controls;

namespace TweetDuck.Dialogs {
    partial class FormPlugins {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.flowLayoutPlugins = new FlowLayoutPanelNoHScroll();
            this.timerLayout = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AutoSize = true;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnClose.Location = new System.Drawing.Point(642, 433);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnClose.Size = new System.Drawing.Size(50, 25);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReload.AutoSize = true;
            this.btnReload.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnReload.Location = new System.Drawing.Point(141, 433);
            this.btnReload.Name = "btnReload";
            this.btnReload.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnReload.Size = new System.Drawing.Size(74, 25);
            this.btnReload.TabIndex = 2;
            this.btnReload.Text = "Reload All";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenFolder.AutoSize = true;
            this.btnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            this.btnOpenFolder.Location = new System.Drawing.Point(12, 433);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnOpenFolder.Size = new System.Drawing.Size(123, 25);
            this.btnOpenFolder.TabIndex = 3;
            this.btnOpenFolder.Text = "Open Plugin Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
            // 
            // flowLayoutPlugins
            // 
            this.flowLayoutPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPlugins.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPlugins.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPlugins.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPlugins.Name = "flowLayoutPlugins";
            this.flowLayoutPlugins.Size = new System.Drawing.Size(680, 415);
            this.flowLayoutPlugins.TabIndex = 0;
            this.flowLayoutPlugins.WrapContents = false;
            this.flowLayoutPlugins.Resize += new System.EventHandler(this.flowLayoutPlugins_Resize);
            // 
            // timerLayout
            // 
            this.timerLayout.Interval = 99;
            this.timerLayout.Tick += new System.EventHandler(this.timerLayout_Tick);
            // 
            // FormPlugins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 470);
            this.Controls.Add(this.flowLayoutPlugins);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnClose);
            this.Icon = global::TweetDuck.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "FormPlugins";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnOpenFolder;
        private FlowLayoutPanelNoHScroll flowLayoutPlugins;
        private System.Windows.Forms.Timer timerLayout;
    }
}