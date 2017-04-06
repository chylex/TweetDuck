namespace TweetDck.Core.Other.Settings {
    partial class TabSettingsSounds {
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupCustomSound = new System.Windows.Forms.GroupBox();
            this.btnPlaySound = new System.Windows.Forms.Button();
            this.btnResetSound = new System.Windows.Forms.Button();
            this.btnBrowseSound = new System.Windows.Forms.Button();
            this.tbCustomSound = new System.Windows.Forms.TextBox();
            this.groupCustomSound.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupCustomSound
            // 
            this.groupCustomSound.Controls.Add(this.btnPlaySound);
            this.groupCustomSound.Controls.Add(this.btnResetSound);
            this.groupCustomSound.Controls.Add(this.btnBrowseSound);
            this.groupCustomSound.Controls.Add(this.tbCustomSound);
            this.groupCustomSound.Location = new System.Drawing.Point(9, 9);
            this.groupCustomSound.Name = "groupCustomSound";
            this.groupCustomSound.Size = new System.Drawing.Size(372, 75);
            this.groupCustomSound.TabIndex = 0;
            this.groupCustomSound.TabStop = false;
            this.groupCustomSound.Text = "Custom Sound Notification";
            // 
            // btnPlaySound
            // 
            this.btnPlaySound.AutoSize = true;
            this.btnPlaySound.Location = new System.Drawing.Point(250, 45);
            this.btnPlaySound.Name = "btnPlaySound";
            this.btnPlaySound.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnPlaySound.Size = new System.Drawing.Size(43, 23);
            this.btnPlaySound.TabIndex = 2;
            this.btnPlaySound.Text = "Play";
            this.btnPlaySound.UseVisualStyleBackColor = true;
            // 
            // btnResetSound
            // 
            this.btnResetSound.AutoSize = true;
            this.btnResetSound.Location = new System.Drawing.Point(6, 45);
            this.btnResetSound.Name = "btnResetSound";
            this.btnResetSound.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnResetSound.Size = new System.Drawing.Size(51, 23);
            this.btnResetSound.TabIndex = 3;
            this.btnResetSound.Text = "Reset";
            this.btnResetSound.UseVisualStyleBackColor = true;
            // 
            // btnBrowseSound
            // 
            this.btnBrowseSound.AutoSize = true;
            this.btnBrowseSound.Location = new System.Drawing.Point(299, 45);
            this.btnBrowseSound.Name = "btnBrowseSound";
            this.btnBrowseSound.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnBrowseSound.Size = new System.Drawing.Size(67, 23);
            this.btnBrowseSound.TabIndex = 1;
            this.btnBrowseSound.Text = "Browse...";
            this.btnBrowseSound.UseVisualStyleBackColor = true;
            // 
            // tbCustomSound
            // 
            this.tbCustomSound.Location = new System.Drawing.Point(6, 19);
            this.tbCustomSound.Name = "tbCustomSound";
            this.tbCustomSound.Size = new System.Drawing.Size(360, 20);
            this.tbCustomSound.TabIndex = 0;
            // 
            // TabSettingsSounds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupCustomSound);
            this.Name = "TabSettingsSounds";
            this.Size = new System.Drawing.Size(478, 282);
            this.groupCustomSound.ResumeLayout(false);
            this.groupCustomSound.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox groupCustomSound;
        private System.Windows.Forms.Button btnResetSound;
        private System.Windows.Forms.Button btnBrowseSound;
        private System.Windows.Forms.TextBox tbCustomSound;
        private System.Windows.Forms.Button btnPlaySound;
    }
}
