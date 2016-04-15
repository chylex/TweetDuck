namespace TweetDck.Core {
    sealed partial class FormBrowser {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBrowser));
            this.SuspendLayout();
            // 
            // FormBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Icon = TweetDck.Properties.Resources.icon;
            this.Location = new System.Drawing.Point(-32000, -32000);
            this.Name = "FormBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ResizeEnd += new System.EventHandler(this.FormBrowser_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

