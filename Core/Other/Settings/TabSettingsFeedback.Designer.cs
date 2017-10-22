namespace TweetDuck.Core.Other.Settings {
    partial class TabSettingsFeedback {
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
            this.panelFeedback = new System.Windows.Forms.Panel();
            this.btnViewReport = new System.Windows.Forms.Button();
            this.btnSendFeedback = new System.Windows.Forms.Button();
            this.labelDataCollectionLink = new System.Windows.Forms.LinkLabel();
            this.checkDataCollection = new System.Windows.Forms.CheckBox();
            this.labelDataCollection = new System.Windows.Forms.Label();
            this.labelFeedback = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelFeedback.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelFeedback
            // 
            this.panelFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFeedback.Controls.Add(this.btnViewReport);
            this.panelFeedback.Controls.Add(this.btnSendFeedback);
            this.panelFeedback.Controls.Add(this.labelDataCollectionLink);
            this.panelFeedback.Controls.Add(this.checkDataCollection);
            this.panelFeedback.Controls.Add(this.labelDataCollection);
            this.panelFeedback.Location = new System.Drawing.Point(9, 31);
            this.panelFeedback.Name = "panelFeedback";
            this.panelFeedback.Size = new System.Drawing.Size(322, 112);
            this.panelFeedback.TabIndex = 1;
            // 
            // btnViewReport
            // 
            this.btnViewReport.AutoSize = true;
            this.btnViewReport.Location = new System.Drawing.Point(6, 82);
            this.btnViewReport.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnViewReport.Name = "btnViewReport";
            this.btnViewReport.Size = new System.Drawing.Size(144, 23);
            this.btnViewReport.TabIndex = 4;
            this.btnViewReport.Text = "View My Analytics Report";
            this.btnViewReport.UseVisualStyleBackColor = true;
            // 
            // btnSendFeedback
            // 
            this.btnSendFeedback.AutoSize = true;
            this.btnSendFeedback.Location = new System.Drawing.Point(5, 3);
            this.btnSendFeedback.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnSendFeedback.Name = "btnSendFeedback";
            this.btnSendFeedback.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnSendFeedback.Size = new System.Drawing.Size(164, 23);
            this.btnSendFeedback.TabIndex = 0;
            this.btnSendFeedback.Text = "Send Feedback / Bug Report";
            this.btnSendFeedback.UseVisualStyleBackColor = true;
            // 
            // labelDataCollectionLink
            // 
            this.labelDataCollectionLink.AutoSize = true;
            this.labelDataCollectionLink.LinkArea = new System.Windows.Forms.LinkArea(1, 10);
            this.labelDataCollectionLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelDataCollectionLink.Location = new System.Drawing.Point(141, 60);
            this.labelDataCollectionLink.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelDataCollectionLink.Name = "labelDataCollectionLink";
            this.labelDataCollectionLink.Size = new System.Drawing.Size(66, 17);
            this.labelDataCollectionLink.TabIndex = 3;
            this.labelDataCollectionLink.TabStop = true;
            this.labelDataCollectionLink.Text = "(learn more)";
            this.labelDataCollectionLink.UseCompatibleTextRendering = true;
            // 
            // checkDataCollection
            // 
            this.checkDataCollection.AutoSize = true;
            this.checkDataCollection.Location = new System.Drawing.Point(6, 59);
            this.checkDataCollection.Margin = new System.Windows.Forms.Padding(6, 5, 0, 3);
            this.checkDataCollection.Name = "checkDataCollection";
            this.checkDataCollection.Size = new System.Drawing.Size(135, 17);
            this.checkDataCollection.TabIndex = 2;
            this.checkDataCollection.Text = "Send Anonymous Data";
            this.checkDataCollection.UseVisualStyleBackColor = true;
            // 
            // labelDataCollection
            // 
            this.labelDataCollection.AutoSize = true;
            this.labelDataCollection.Location = new System.Drawing.Point(3, 41);
            this.labelDataCollection.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelDataCollection.Name = "labelDataCollection";
            this.labelDataCollection.Size = new System.Drawing.Size(79, 13);
            this.labelDataCollection.TabIndex = 1;
            this.labelDataCollection.Text = "Data Collection";
            // 
            // labelFeedback
            // 
            this.labelFeedback.AutoSize = true;
            this.labelFeedback.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelFeedback.Location = new System.Drawing.Point(6, 8);
            this.labelFeedback.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelFeedback.Name = "labelFeedback";
            this.labelFeedback.Size = new System.Drawing.Size(80, 20);
            this.labelFeedback.TabIndex = 0;
            this.labelFeedback.Text = "Feedback";
            // 
            // TabSettingsFeedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelFeedback);
            this.Controls.Add(this.labelFeedback);
            this.Name = "TabSettingsFeedback";
            this.Size = new System.Drawing.Size(340, 151);
            this.panelFeedback.ResumeLayout(false);
            this.panelFeedback.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelFeedback;
        private System.Windows.Forms.CheckBox checkDataCollection;
        private System.Windows.Forms.Label labelDataCollection;
        private System.Windows.Forms.Label labelFeedback;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.LinkLabel labelDataCollectionLink;
        private System.Windows.Forms.Button btnSendFeedback;
        private System.Windows.Forms.Button btnViewReport;
    }
}
