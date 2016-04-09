namespace TweetDick.Core {
    partial class RichTextLabel {
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
            this.SuspendLayout();
            // 
            // RichTextLabel
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ReadOnly = true;
            this.TabStop = false;
            this.MouseEnter += new System.EventHandler(this.RichTextLabel_MouseEnter);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
