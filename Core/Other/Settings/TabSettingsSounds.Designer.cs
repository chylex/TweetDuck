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
            this.tbCustomSound = new System.Windows.Forms.TextBox();
            this.labelVolumeValue = new System.Windows.Forms.Label();
            this.btnPlaySound = new System.Windows.Forms.Button();
            this.btnResetSound = new System.Windows.Forms.Button();
            this.btnBrowseSound = new System.Windows.Forms.Button();
            this.labelSoundNotification = new System.Windows.Forms.Label();
            this.panelSoundNotification = new System.Windows.Forms.Panel();
            this.labelVolume = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panelVolume = new System.Windows.Forms.Panel();
            this.volumeUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.panelSoundNotification.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.flowPanel.SuspendLayout();
            this.panelVolume.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbCustomSound
            // 
            this.tbCustomSound.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCustomSound.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tbCustomSound.Location = new System.Drawing.Point(3, 3);
            this.tbCustomSound.Name = "tbCustomSound";
            this.tbCustomSound.Size = new System.Drawing.Size(316, 23);
            this.tbCustomSound.TabIndex = 0;
            // 
            // labelVolumeValue
            // 
            this.labelVolumeValue.BackColor = System.Drawing.Color.Transparent;
            this.labelVolumeValue.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelVolumeValue.Location = new System.Drawing.Point(147, 4);
            this.labelVolumeValue.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.labelVolumeValue.Name = "labelVolumeValue";
            this.labelVolumeValue.Size = new System.Drawing.Size(38, 15);
            this.labelVolumeValue.TabIndex = 1;
            this.labelVolumeValue.Text = "100%";
            this.labelVolumeValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnPlaySound
            // 
            this.btnPlaySound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlaySound.AutoSize = true;
            this.btnPlaySound.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnPlaySound.Location = new System.Drawing.Point(202, 32);
            this.btnPlaySound.Name = "btnPlaySound";
            this.btnPlaySound.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnPlaySound.Size = new System.Drawing.Size(43, 25);
            this.btnPlaySound.TabIndex = 2;
            this.btnPlaySound.Text = "Play";
            this.btnPlaySound.UseVisualStyleBackColor = true;
            // 
            // btnResetSound
            // 
            this.btnResetSound.AutoSize = true;
            this.btnResetSound.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnResetSound.Location = new System.Drawing.Point(3, 32);
            this.btnResetSound.Name = "btnResetSound";
            this.btnResetSound.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnResetSound.Size = new System.Drawing.Size(49, 25);
            this.btnResetSound.TabIndex = 3;
            this.btnResetSound.Text = "Reset";
            this.btnResetSound.UseVisualStyleBackColor = true;
            // 
            // btnBrowseSound
            // 
            this.btnBrowseSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseSound.AutoSize = true;
            this.btnBrowseSound.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBrowseSound.Location = new System.Drawing.Point(251, 32);
            this.btnBrowseSound.Name = "btnBrowseSound";
            this.btnBrowseSound.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.btnBrowseSound.Size = new System.Drawing.Size(68, 25);
            this.btnBrowseSound.TabIndex = 1;
            this.btnBrowseSound.Text = "Browse...";
            this.btnBrowseSound.UseVisualStyleBackColor = true;
            // 
            // labelSoundNotification
            // 
            this.labelSoundNotification.AutoSize = true;
            this.labelSoundNotification.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.labelSoundNotification.Location = new System.Drawing.Point(0, 0);
            this.labelSoundNotification.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.labelSoundNotification.Name = "labelSoundNotification";
            this.labelSoundNotification.Size = new System.Drawing.Size(160, 19);
            this.labelSoundNotification.TabIndex = 0;
            this.labelSoundNotification.Text = "SOUND NOTIFICATION";
            // 
            // panelSoundNotification
            // 
            this.panelSoundNotification.Controls.Add(this.btnPlaySound);
            this.panelSoundNotification.Controls.Add(this.tbCustomSound);
            this.panelSoundNotification.Controls.Add(this.btnResetSound);
            this.panelSoundNotification.Controls.Add(this.btnBrowseSound);
            this.panelSoundNotification.Location = new System.Drawing.Point(0, 20);
            this.panelSoundNotification.Margin = new System.Windows.Forms.Padding(0);
            this.panelSoundNotification.Name = "panelSoundNotification";
            this.panelSoundNotification.Size = new System.Drawing.Size(322, 59);
            this.panelSoundNotification.TabIndex = 1;
            // 
            // labelVolume
            // 
            this.labelVolume.AutoSize = true;
            this.labelVolume.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelVolume.Location = new System.Drawing.Point(3, 91);
            this.labelVolume.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(48, 15);
            this.labelVolume.TabIndex = 2;
            this.labelVolume.Text = "Volume";
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.AutoSize = false;
            this.trackBarVolume.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarVolume.Location = new System.Drawing.Point(3, 3);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(148, 30);
            this.trackBarVolume.TabIndex = 0;
            this.trackBarVolume.TickFrequency = 10;
            this.trackBarVolume.Value = 100;
            this.trackBarVolume.ValueChanged += new System.EventHandler(this.trackBarVolume_ValueChanged);
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowPanel.Controls.Add(this.labelSoundNotification);
            this.flowPanel.Controls.Add(this.panelSoundNotification);
            this.flowPanel.Controls.Add(this.labelVolume);
            this.flowPanel.Controls.Add(this.panelVolume);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(9, 9);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(322, 462);
            this.flowPanel.TabIndex = 0;
            this.flowPanel.WrapContents = false;
            // 
            // panelVolume
            // 
            this.panelVolume.Controls.Add(this.trackBarVolume);
            this.panelVolume.Controls.Add(this.labelVolumeValue);
            this.panelVolume.Location = new System.Drawing.Point(0, 106);
            this.panelVolume.Margin = new System.Windows.Forms.Padding(0);
            this.panelVolume.Name = "panelVolume";
            this.panelVolume.Size = new System.Drawing.Size(322, 36);
            this.panelVolume.TabIndex = 3;
            // 
            // volumeUpdateTimer
            // 
            this.volumeUpdateTimer.Interval = 250;
            this.volumeUpdateTimer.Tick += new System.EventHandler(this.volumeUpdateTimer_Tick);
            // 
            // TabSettingsSounds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowPanel);
            this.Name = "TabSettingsSounds";
            this.Size = new System.Drawing.Size(631, 480);
            this.panelSoundNotification.ResumeLayout(false);
            this.panelSoundNotification.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.panelVolume.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnResetSound;
        private System.Windows.Forms.Button btnBrowseSound;
        private System.Windows.Forms.TextBox tbCustomSound;
        private System.Windows.Forms.Button btnPlaySound;
        private System.Windows.Forms.Label labelSoundNotification;
        private System.Windows.Forms.Panel panelSoundNotification;
        private System.Windows.Forms.Label labelVolume;
        private System.Windows.Forms.Label labelVolumeValue;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Panel panelVolume;
        private System.Windows.Forms.Timer volumeUpdateTimer;
    }
}
