namespace TweetDuck.Core.Other.Settings.Dialogs {
    partial class DialogSettingsCefArgs {
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
            this.textBoxArgs = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.labelWarning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxArgs
            // 
            this.textBoxArgs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxArgs.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxArgs.Location = new System.Drawing.Point(12, 28);
            this.textBoxArgs.Multiline = true;
            this.textBoxArgs.Name = "textBoxArgs";
            this.textBoxArgs.Size = new System.Drawing.Size(460, 193);
            this.textBoxArgs.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(354, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnCancel.Size = new System.Drawing.Size(56, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(416, 227);
            this.btnApply.Name = "btnApply";
            this.btnApply.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnApply.Size = new System.Drawing.Size(56, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHelp.AutoSize = true;
            this.btnHelp.Location = new System.Drawing.Point(12, 227);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnHelp.Size = new System.Drawing.Size(124, 23);
            this.btnHelp.TabIndex = 4;
            this.btnHelp.Text = "List of Chromium Args";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // labelWarning
            // 
            this.labelWarning.AutoSize = true;
            this.labelWarning.Location = new System.Drawing.Point(12, 9);
            this.labelWarning.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(423, 13);
            this.labelWarning.TabIndex = 0;
            this.labelWarning.Text = "Warning: Some arguments may cause the program to stop working, edit at your own r" +
    "isk.";
            // 
            // DialogSettingsCefArgs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 262);
            this.Controls.Add(this.labelWarning);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.textBoxArgs);
            this.MinimumSize = new System.Drawing.Size(500, 160);
            this.Name = "DialogSettingsCefArgs";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxArgs;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label labelWarning;
    }
}