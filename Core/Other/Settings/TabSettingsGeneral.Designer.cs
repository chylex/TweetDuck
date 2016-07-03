namespace TweetDck.Core.Other.Settings {
    partial class TabSettingsGeneral {
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
            this.checkExpandLinks = new System.Windows.Forms.CheckBox();
            this.comboBoxTrayType = new System.Windows.Forms.ComboBox();
            this.labelTrayType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkExpandLinks
            // 
            this.checkExpandLinks.AutoSize = true;
            this.checkExpandLinks.Location = new System.Drawing.Point(9, 10);
            this.checkExpandLinks.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.checkExpandLinks.Name = "checkExpandLinks";
            this.checkExpandLinks.Size = new System.Drawing.Size(166, 17);
            this.checkExpandLinks.TabIndex = 14;
            this.checkExpandLinks.Text = "Expand Links When Hovered";
            this.checkExpandLinks.UseVisualStyleBackColor = true;
            this.checkExpandLinks.CheckedChanged += new System.EventHandler(this.checkExpandLinks_CheckedChanged);
            // 
            // comboBoxTrayType
            // 
            this.comboBoxTrayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrayType.FormattingEnabled = true;
            this.comboBoxTrayType.Location = new System.Drawing.Point(9, 57);
            this.comboBoxTrayType.Name = "comboBoxTrayType";
            this.comboBoxTrayType.Size = new System.Drawing.Size(171, 21);
            this.comboBoxTrayType.TabIndex = 13;
            this.comboBoxTrayType.SelectedIndexChanged += new System.EventHandler(this.comboBoxTrayType_SelectedIndexChanged);
            // 
            // labelTrayType
            // 
            this.labelTrayType.AutoSize = true;
            this.labelTrayType.Location = new System.Drawing.Point(6, 41);
            this.labelTrayType.Margin = new System.Windows.Forms.Padding(3, 11, 3, 0);
            this.labelTrayType.Name = "labelTrayType";
            this.labelTrayType.Size = new System.Drawing.Size(52, 13);
            this.labelTrayType.TabIndex = 12;
            this.labelTrayType.Text = "Tray Icon";
            // 
            // TabSettingsGeneral
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkExpandLinks);
            this.Controls.Add(this.comboBoxTrayType);
            this.Controls.Add(this.labelTrayType);
            this.Name = "TabSettingsGeneral";
            this.Size = new System.Drawing.Size(219, 99);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkExpandLinks;
        private System.Windows.Forms.ComboBox comboBoxTrayType;
        private System.Windows.Forms.Label labelTrayType;
    }
}
