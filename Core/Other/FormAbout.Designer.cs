using TweetDck.Core.Controls;

namespace TweetDck.Core.Other {
    sealed partial class FormAbout {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this.pictureLogo = new System.Windows.Forms.PictureBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelSourceCode = new System.Windows.Forms.LinkLabel();
            this.labelWebsite = new System.Windows.Forms.LinkLabel();
            this.tablePanelLinks = new System.Windows.Forms.TableLayoutPanel();
            this.labelIssues = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).BeginInit();
            this.tablePanelLinks.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureLogo
            // 
            this.pictureLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureLogo.ErrorImage = null;
            this.pictureLogo.Image = ((System.Drawing.Image)(resources.GetObject("pictureLogo.Image")));
            this.pictureLogo.InitialImage = null;
            this.pictureLogo.Location = new System.Drawing.Point(12, 12);
            this.pictureLogo.Name = "pictureLogo";
            this.pictureLogo.Size = new System.Drawing.Size(96, 96);
            this.pictureLogo.TabIndex = 0;
            this.pictureLogo.TabStop = false;
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelDescription.Location = new System.Drawing.Point(114, 12);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(232, 109);
            this.labelDescription.TabIndex = 1;
            // 
            // labelSourceCode
            // 
            this.labelSourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSourceCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelSourceCode.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.labelSourceCode.Location = new System.Drawing.Point(117, 0);
            this.labelSourceCode.Margin = new System.Windows.Forms.Padding(0);
            this.labelSourceCode.Name = "labelSourceCode";
            this.labelSourceCode.Size = new System.Drawing.Size(99, 16);
            this.labelSourceCode.TabIndex = 3;
            this.labelSourceCode.Text = "Source Code";
            this.labelSourceCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelSourceCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // labelWebsite
            // 
            this.labelWebsite.AutoSize = true;
            this.labelWebsite.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWebsite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelWebsite.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.labelWebsite.Location = new System.Drawing.Point(0, 0);
            this.labelWebsite.Margin = new System.Windows.Forms.Padding(0);
            this.labelWebsite.Name = "labelWebsite";
            this.labelWebsite.Size = new System.Drawing.Size(117, 16);
            this.labelWebsite.TabIndex = 2;
            this.labelWebsite.Text = "Official Website";
            this.labelWebsite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // tablePanelLinks
            // 
            this.tablePanelLinks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tablePanelLinks.ColumnCount = 3;
            this.tablePanelLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.16F));
            this.tablePanelLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.7F));
            this.tablePanelLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.14F));
            this.tablePanelLinks.Controls.Add(this.labelIssues, 2, 0);
            this.tablePanelLinks.Controls.Add(this.labelWebsite, 0, 0);
            this.tablePanelLinks.Controls.Add(this.labelSourceCode, 1, 0);
            this.tablePanelLinks.Location = new System.Drawing.Point(12, 124);
            this.tablePanelLinks.Name = "tablePanelLinks";
            this.tablePanelLinks.RowCount = 1;
            this.tablePanelLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelLinks.Size = new System.Drawing.Size(334, 16);
            this.tablePanelLinks.TabIndex = 4;
            // 
            // labelIssues
            // 
            this.labelIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelIssues.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelIssues.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.labelIssues.Location = new System.Drawing.Point(216, 0);
            this.labelIssues.Margin = new System.Windows.Forms.Padding(0);
            this.labelIssues.Name = "labelIssues";
            this.labelIssues.Size = new System.Drawing.Size(118, 16);
            this.labelIssues.TabIndex = 4;
            this.labelIssues.Text = "Report an Issue";
            this.labelIssues.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelIssues.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(358, 152);
            this.Controls.Add(this.tablePanelLinks);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.pictureLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.pictureLogo)).EndInit();
            this.tablePanelLinks.ResumeLayout(false);
            this.tablePanelLinks.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureLogo;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.LinkLabel labelSourceCode;
        private System.Windows.Forms.LinkLabel labelWebsite;
        private System.Windows.Forms.TableLayoutPanel tablePanelLinks;
        private System.Windows.Forms.LinkLabel labelIssues;

    }
}