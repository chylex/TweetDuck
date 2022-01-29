namespace TweetDuck.Browser {
    partial class TrayIcon {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            // 
            // notifyIcon
            // 
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);
            //
            // TrayIcon
            //
        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
    }
}
