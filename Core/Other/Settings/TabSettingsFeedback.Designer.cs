﻿namespace TweetDuck.Core.Other.Settings {
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
            this.panelDataCollection = new System.Windows.Forms.Panel();
            this.labelDataCollectionLink = new System.Windows.Forms.LinkLabel();
            this.checkDataCollection = new System.Windows.Forms.CheckBox();
            this.labelDataCollectionMessage = new System.Windows.Forms.Label();
            this.btnViewReport = new System.Windows.Forms.Button();
            this.btnSendFeedback = new System.Windows.Forms.Button();
            this.labelDataCollection = new System.Windows.Forms.Label();
            this.labelFeedback = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panelDataCollection.SuspendLayout();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDataCollection
            // 
            this.panelDataCollection.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panelDataCollection.Controls.Add(this.labelDataCollectionLink);
            this.panelDataCollection.Controls.Add(this.checkDataCollection);
            this.panelDataCollection.Location = new System.Drawing.Point(0, 74);
            this.panelDataCollection.Margin = new System.Windows.Forms.Padding(0);
            this.panelDataCollection.Name = "panelDataCollection";
            this.panelDataCollection.Size = new System.Drawing.Size(322, 26);
            this.panelDataCollection.TabIndex = 1;
            // 
            // labelDataCollectionLink
            // 
            this.labelDataCollectionLink.AutoSize = true;
            this.labelDataCollectionLink.LinkArea = new System.Windows.Forms.LinkArea(1, 10);
            this.labelDataCollectionLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelDataCollectionLink.Location = new System.Drawing.Point(141, 6);
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
            this.checkDataCollection.Location = new System.Drawing.Point(6, 6);
            this.checkDataCollection.Margin = new System.Windows.Forms.Padding(6, 6, 0, 3);
            this.checkDataCollection.Name = "checkDataCollection";
            this.checkDataCollection.Size = new System.Drawing.Size(135, 17);
            this.checkDataCollection.TabIndex = 2;
            this.checkDataCollection.Text = "Send Anonymous Data";
            this.checkDataCollection.UseVisualStyleBackColor = true;
            // 
            // labelDataCollectionMessage
            // 
            this.labelDataCollectionMessage.Location = new System.Drawing.Point(6, 135);
            this.labelDataCollectionMessage.Margin = new System.Windows.Forms.Padding(6);
            this.labelDataCollectionMessage.Name = "labelDataCollectionMessage";
            this.labelDataCollectionMessage.Size = new System.Drawing.Size(310, 67);
            this.labelDataCollectionMessage.TabIndex = 5;
            // 
            // btnViewReport
            // 
            this.btnViewReport.AutoSize = true;
            this.btnViewReport.Location = new System.Drawing.Point(5, 103);
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
            this.btnSendFeedback.Location = new System.Drawing.Point(5, 23);
            this.btnSendFeedback.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnSendFeedback.Name = "btnSendFeedback";
            this.btnSendFeedback.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnSendFeedback.Size = new System.Drawing.Size(164, 23);
            this.btnSendFeedback.TabIndex = 0;
            this.btnSendFeedback.Text = "Send Feedback / Bug Report";
            this.btnSendFeedback.UseVisualStyleBackColor = true;
            // 
            // labelDataCollection
            // 
            this.labelDataCollection.AutoSize = true;
            this.labelDataCollection.Location = new System.Drawing.Point(3, 61);
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
            this.labelFeedback.Location = new System.Drawing.Point(0, 0);
            this.labelFeedback.Margin = new System.Windows.Forms.Padding(0);
            this.labelFeedback.Name = "labelFeedback";
            this.labelFeedback.Size = new System.Drawing.Size(80, 20);
            this.labelFeedback.TabIndex = 0;
            this.labelFeedback.Text = "Feedback";
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Controls.Add(this.labelFeedback);
            this.flowPanel.Controls.Add(this.btnSendFeedback);
            this.flowPanel.Controls.Add(this.labelDataCollection);
            this.flowPanel.Controls.Add(this.panelDataCollection);
            this.flowPanel.Controls.Add(this.btnViewReport);
            this.flowPanel.Controls.Add(this.labelDataCollectionMessage);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(9, 9);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(322, 209);
            this.flowPanel.TabIndex = 2;
            this.flowPanel.WrapContents = false;
            // 
            // TabSettingsFeedback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowPanel);
            this.Name = "TabSettingsFeedback";
            this.Size = new System.Drawing.Size(340, 227);
            this.panelDataCollection.ResumeLayout(false);
            this.panelDataCollection.PerformLayout();
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDataCollection;
        private System.Windows.Forms.CheckBox checkDataCollection;
        private System.Windows.Forms.Label labelDataCollection;
        private System.Windows.Forms.Label labelFeedback;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.LinkLabel labelDataCollectionLink;
        private System.Windows.Forms.Button btnSendFeedback;
        private System.Windows.Forms.Button btnViewReport;
        private System.Windows.Forms.Label labelDataCollectionMessage;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
    }
}
