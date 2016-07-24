namespace TweetDck.Core.Other.Settings {
    partial class TabSettingsAdvanced {
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
            this.btnClearCache = new System.Windows.Forms.Button();
            this.labelMiscellaneous = new System.Windows.Forms.Label();
            this.checkHardwareAcceleration = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btnClearCache
            // 
            this.btnClearCache.Location = new System.Drawing.Point(12, 56);
            this.btnClearCache.Name = "btnClearCache";
            this.btnClearCache.Size = new System.Drawing.Size(171, 23);
            this.btnClearCache.TabIndex = 14;
            this.btnClearCache.Text = "Clear Cache (calculating)";
            this.toolTip.SetToolTip(this.btnClearCache, "Clearing cache will free up space taken by downloaded images and other resources." +
        "");
            this.btnClearCache.UseVisualStyleBackColor = true;
            this.btnClearCache.Click += new System.EventHandler(this.btnClearCache_Click);
            // 
            // labelMiscellaneous
            // 
            this.labelMiscellaneous.AutoSize = true;
            this.labelMiscellaneous.Location = new System.Drawing.Point(9, 40);
            this.labelMiscellaneous.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
            this.labelMiscellaneous.Name = "labelMiscellaneous";
            this.labelMiscellaneous.Size = new System.Drawing.Size(74, 13);
            this.labelMiscellaneous.TabIndex = 13;
            this.labelMiscellaneous.Text = "Miscellaneous";
            // 
            // checkHardwareAcceleration
            // 
            this.checkHardwareAcceleration.AutoSize = true;
            this.checkHardwareAcceleration.Location = new System.Drawing.Point(9, 9);
            this.checkHardwareAcceleration.Name = "checkHardwareAcceleration";
            this.checkHardwareAcceleration.Size = new System.Drawing.Size(134, 17);
            this.checkHardwareAcceleration.TabIndex = 12;
            this.checkHardwareAcceleration.Text = "Hardware Acceleration";
            this.toolTip.SetToolTip(this.checkHardwareAcceleration, "Uses your graphics card to improve performance.\r\nDisable if you experience issues" +
        " with rendering.");
            this.checkHardwareAcceleration.UseVisualStyleBackColor = true;
            this.checkHardwareAcceleration.CheckedChanged += new System.EventHandler(this.checkHardwareAcceleration_CheckedChanged);
            // 
            // TabSettingsAdvanced
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClearCache);
            this.Controls.Add(this.labelMiscellaneous);
            this.Controls.Add(this.checkHardwareAcceleration);
            this.Name = "TabSettingsAdvanced";
            this.Size = new System.Drawing.Size(239, 120);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClearCache;
        private System.Windows.Forms.Label labelMiscellaneous;
        private System.Windows.Forms.CheckBox checkHardwareAcceleration;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
