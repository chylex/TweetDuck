namespace TweetDuck.Dialogs.Settings {
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
	        this.btnSendFeedback = new System.Windows.Forms.Button();
	        this.toolTip = new System.Windows.Forms.ToolTip(this.components);
	        this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
	        this.labelFeedback = new System.Windows.Forms.Label();
	        this.flowPanel.SuspendLayout();
	        this.SuspendLayout();
	        //
	        // btnSendFeedback
	        //
	        this.btnSendFeedback.AutoSize = true;
	        this.btnSendFeedback.Font = new System.Drawing.Font("Segoe UI", 9F);
	        this.btnSendFeedback.Location = new System.Drawing.Point(5, 23);
	        this.btnSendFeedback.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
	        this.btnSendFeedback.Name = "btnSendFeedback";
	        this.btnSendFeedback.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
	        this.btnSendFeedback.Size = new System.Drawing.Size(170, 25);
	        this.btnSendFeedback.TabIndex = 1;
	        this.btnSendFeedback.Text = "Send Feedback / Bug Report";
	        this.btnSendFeedback.UseVisualStyleBackColor = true;
	        //
	        // flowPanel
	        //
	        this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
	        this.flowPanel.Controls.Add(this.labelFeedback);
	        this.flowPanel.Controls.Add(this.btnSendFeedback);
	        this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
	        this.flowPanel.Location = new System.Drawing.Point(9, 9);
	        this.flowPanel.Name = "flowPanel";
	        this.flowPanel.Size = new System.Drawing.Size(300, 462);
	        this.flowPanel.TabIndex = 0;
	        this.flowPanel.WrapContents = false;
	        //
	        // labelFeedback
	        //
	        this.labelFeedback.AutoSize = true;
	        this.labelFeedback.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
	        this.labelFeedback.Location = new System.Drawing.Point(0, 0);
	        this.labelFeedback.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
	        this.labelFeedback.Name = "labelFeedback";
	        this.labelFeedback.Size = new System.Drawing.Size(75, 19);
	        this.labelFeedback.TabIndex = 0;
	        this.labelFeedback.Text = "FEEDBACK";
	        //
	        // TabSettingsFeedback
	        //
	        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
	        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
	        this.Controls.Add(this.flowPanel);
	        this.Font = TweetDuck.Controls.ControlExtensions.DefaultFont;
	        this.Name = "TabSettingsFeedback";
	        this.Size = new System.Drawing.Size(631, 480);
	        this.flowPanel.ResumeLayout(false);
	        this.flowPanel.PerformLayout();
	        this.ResumeLayout(false);
        }

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnSendFeedback;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Label labelFeedback;

        #endregion
    }
}
