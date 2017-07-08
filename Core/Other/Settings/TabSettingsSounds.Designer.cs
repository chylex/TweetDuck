namespace TweetDuck.Core.Other.Settings {
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
            this.btnPlaySound = new System.Windows.Forms.Button();
            this.btnResetSound = new System.Windows.Forms.Button();
            this.btnBrowseSound = new System.Windows.Forms.Button();
            this.tbCustomSound = new System.Windows.Forms.TextBox();
            this.labelSoundNotification = new System.Windows.Forms.Label();
            this.panelSoundNotification = new System.Windows.Forms.Panel();
            this.panelSoundNotification.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPlaySound
            // 
            this.btnPlaySound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlaySound.AutoSize = true;
            this.btnPlaySound.Location = new System.Drawing.Point(203, 29);
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
            this.btnResetSound.Location = new System.Drawing.Point(3, 29);
            this.btnResetSound.Name = "btnResetSound";
            this.btnResetSound.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnResetSound.Size = new System.Drawing.Size(51, 23);
            this.btnResetSound.TabIndex = 3;
            this.btnResetSound.Text = "Reset";
            this.btnResetSound.UseVisualStyleBackColor = true;
            // 
            // btnBrowseSound
            // 
            this.btnBrowseSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSound.AutoSize = true;
            this.btnBrowseSound.Location = new System.Drawing.Point(252, 29);
            this.btnBrowseSound.Name = "btnBrowseSound";
            this.btnBrowseSound.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnBrowseSound.Size = new System.Drawing.Size(67, 23);
            this.btnBrowseSound.TabIndex = 1;
            this.btnBrowseSound.Text = "Browse...";
            this.btnBrowseSound.UseVisualStyleBackColor = true;
            // 
            // tbCustomSound
            // 
            this.tbCustomSound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCustomSound.Location = new System.Drawing.Point(3, 3);
            this.tbCustomSound.Name = "tbCustomSound";
            this.tbCustomSound.Size = new System.Drawing.Size(316, 20);
            this.tbCustomSound.TabIndex = 0;
            this.toolTip.SetToolTip(this.tbCustomSound, "When empty, the default TweetDeck sound notification is used.");
            // 
            // labelSoundNotification
            // 
            this.labelSoundNotification.AutoSize = true;
            this.labelSoundNotification.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelSoundNotification.Location = new System.Drawing.Point(6, 8);
            this.labelSoundNotification.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelSoundNotification.Name = "labelSoundNotification";
            this.labelSoundNotification.Size = new System.Drawing.Size(198, 20);
            this.labelSoundNotification.TabIndex = 1;
            this.labelSoundNotification.Text = "Custom Sound Notification";
            // 
            // panelSoundNotification
            // 
            this.panelSoundNotification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSoundNotification.Controls.Add(this.btnPlaySound);
            this.panelSoundNotification.Controls.Add(this.tbCustomSound);
            this.panelSoundNotification.Controls.Add(this.btnResetSound);
            this.panelSoundNotification.Controls.Add(this.btnBrowseSound);
            this.panelSoundNotification.Location = new System.Drawing.Point(9, 31);
            this.panelSoundNotification.Name = "panelSoundNotification";
            this.panelSoundNotification.Size = new System.Drawing.Size(322, 56);
            this.panelSoundNotification.TabIndex = 2;
            // 
            // TabSettingsSounds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelSoundNotification);
            this.Controls.Add(this.labelSoundNotification);
            this.Name = "TabSettingsSounds";
            this.Size = new System.Drawing.Size(340, 97);
            this.panelSoundNotification.ResumeLayout(false);
            this.panelSoundNotification.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnResetSound;
        private System.Windows.Forms.Button btnBrowseSound;
        private System.Windows.Forms.TextBox tbCustomSound;
        private System.Windows.Forms.Button btnPlaySound;
        private System.Windows.Forms.Label labelSoundNotification;
        private System.Windows.Forms.Panel panelSoundNotification;
    }
}
