namespace TweetDuck.Dialogs {
    partial class FormMessage {
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
            this.panelActions = new System.Windows.Forms.Panel();
            this.labelMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // panelActions
            // 
            this.panelActions.BackColor = System.Drawing.SystemColors.Control;
            this.panelActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelActions.Location = new System.Drawing.Point(0, 84);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(98, 49);
            this.panelActions.TabIndex = 0;
            // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.AutoSize = true;
            this.labelMessage.Font = System.Drawing.SystemFonts.MessageBoxFont;
            this.labelMessage.Location = new System.Drawing.Point(62, 34);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(53, 24, 27, 24);
            this.labelMessage.MaximumSize = new System.Drawing.Size(600, 0);
            this.labelMessage.MinimumSize = new System.Drawing.Size(0, 24);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(0, 24);
            this.labelMessage.TabIndex = 1;
            this.labelMessage.SizeChanged += new System.EventHandler(this.labelMessage_SizeChanged);
            // 
            // FormMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(98, 133);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.panelActions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMessage";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.SizeChanged += new System.EventHandler(this.FormMessage_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelActions;
        private System.Windows.Forms.Label labelMessage;
    }
}