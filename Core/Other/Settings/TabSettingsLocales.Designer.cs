namespace TweetDuck.Core.Other.Settings {
    partial class TabSettingsLocales {
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
            this.checkSpellCheck = new System.Windows.Forms.CheckBox();
            this.labelLocales = new System.Windows.Forms.Label();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.labelSpellCheckLanguage = new System.Windows.Forms.Label();
            this.comboBoxSpellCheckLanguage = new System.Windows.Forms.ComboBox();
            this.labelTranslations = new System.Windows.Forms.Label();
            this.labelTranslationTarget = new System.Windows.Forms.Label();
            this.comboBoxTranslationTarget = new System.Windows.Forms.ComboBox();
            this.flowPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkSpellCheck
            // 
            this.checkSpellCheck.AutoSize = true;
            this.checkSpellCheck.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkSpellCheck.Location = new System.Drawing.Point(6, 26);
            this.checkSpellCheck.Margin = new System.Windows.Forms.Padding(6, 6, 3, 2);
            this.checkSpellCheck.Name = "checkSpellCheck";
            this.checkSpellCheck.Size = new System.Drawing.Size(125, 19);
            this.checkSpellCheck.TabIndex = 1;
            this.checkSpellCheck.Text = "Enable Spell Check";
            this.checkSpellCheck.UseVisualStyleBackColor = true;
            // 
            // labelLocales
            // 
            this.labelLocales.AutoSize = true;
            this.labelLocales.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelLocales.Location = new System.Drawing.Point(0, 0);
            this.labelLocales.Margin = new System.Windows.Forms.Padding(0);
            this.labelLocales.Name = "labelLocales";
            this.labelLocales.Size = new System.Drawing.Size(58, 20);
            this.labelLocales.TabIndex = 0;
            this.labelLocales.Text = "Locales";
            // 
            // flowPanel
            // 
            this.flowPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowPanel.Controls.Add(this.labelLocales);
            this.flowPanel.Controls.Add(this.checkSpellCheck);
            this.flowPanel.Controls.Add(this.labelSpellCheckLanguage);
            this.flowPanel.Controls.Add(this.comboBoxSpellCheckLanguage);
            this.flowPanel.Controls.Add(this.labelTranslations);
            this.flowPanel.Controls.Add(this.labelTranslationTarget);
            this.flowPanel.Controls.Add(this.comboBoxTranslationTarget);
            this.flowPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanel.Location = new System.Drawing.Point(9, 9);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(322, 201);
            this.flowPanel.TabIndex = 0;
            this.flowPanel.WrapContents = false;
            // 
            // labelSpellCheckLanguage
            // 
            this.labelSpellCheckLanguage.AutoSize = true;
            this.labelSpellCheckLanguage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelSpellCheckLanguage.Location = new System.Drawing.Point(3, 59);
            this.labelSpellCheckLanguage.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelSpellCheckLanguage.Name = "labelSpellCheckLanguage";
            this.labelSpellCheckLanguage.Size = new System.Drawing.Size(123, 15);
            this.labelSpellCheckLanguage.TabIndex = 2;
            this.labelSpellCheckLanguage.Text = "Spell Check Language";
            // 
            // comboBoxSpellCheckLanguage
            // 
            this.comboBoxSpellCheckLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSpellCheckLanguage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.comboBoxSpellCheckLanguage.FormattingEnabled = true;
            this.comboBoxSpellCheckLanguage.Location = new System.Drawing.Point(5, 77);
            this.comboBoxSpellCheckLanguage.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comboBoxSpellCheckLanguage.Name = "comboBoxSpellCheckLanguage";
            this.comboBoxSpellCheckLanguage.Size = new System.Drawing.Size(311, 23);
            this.comboBoxSpellCheckLanguage.TabIndex = 3;
            // 
            // labelTranslations
            // 
            this.labelTranslations.AutoSize = true;
            this.labelTranslations.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTranslations.Location = new System.Drawing.Point(0, 123);
            this.labelTranslations.Margin = new System.Windows.Forms.Padding(0, 20, 0, 0);
            this.labelTranslations.Name = "labelTranslations";
            this.labelTranslations.Size = new System.Drawing.Size(109, 20);
            this.labelTranslations.TabIndex = 4;
            this.labelTranslations.Text = "Bing Translator";
            // 
            // labelTranslationTarget
            // 
            this.labelTranslationTarget.AutoSize = true;
            this.labelTranslationTarget.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelTranslationTarget.Location = new System.Drawing.Point(3, 155);
            this.labelTranslationTarget.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
            this.labelTranslationTarget.Name = "labelTranslationTarget";
            this.labelTranslationTarget.Size = new System.Drawing.Size(96, 15);
            this.labelTranslationTarget.TabIndex = 5;
            this.labelTranslationTarget.Text = "Target Language";
            // 
            // comboBoxTranslationTarget
            // 
            this.comboBoxTranslationTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTranslationTarget.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.comboBoxTranslationTarget.FormattingEnabled = true;
            this.comboBoxTranslationTarget.Location = new System.Drawing.Point(5, 173);
            this.comboBoxTranslationTarget.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.comboBoxTranslationTarget.Name = "comboBoxTranslationTarget";
            this.comboBoxTranslationTarget.Size = new System.Drawing.Size(311, 23);
            this.comboBoxTranslationTarget.TabIndex = 6;
            // 
            // TabSettingsLocales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowPanel);
            this.Name = "TabSettingsLocales";
            this.Size = new System.Drawing.Size(340, 219);
            this.flowPanel.ResumeLayout(false);
            this.flowPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox checkSpellCheck;
        private System.Windows.Forms.Label labelLocales;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.ComboBox comboBoxTranslationTarget;
        private System.Windows.Forms.Label labelTranslationTarget;
        private System.Windows.Forms.ComboBox comboBoxSpellCheckLanguage;
        private System.Windows.Forms.Label labelTranslations;
        private System.Windows.Forms.Label labelSpellCheckLanguage;
    }
}
