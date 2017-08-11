namespace TweetDuck.Video {
    partial class FormPlayer {
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
            this.components = new System.ComponentModel.Container();
            this.timerSync = new System.Windows.Forms.Timer(this.components);
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            this.tablePanel = new System.Windows.Forms.TableLayoutPanel();
            this.progressSeek = new TweetDuck.Video.FlatProgressBar();
            this.labelTime = new System.Windows.Forms.Label();
            this.timerData = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.tablePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerSync
            // 
            this.timerSync.Interval = 10;
            this.timerSync.Tick += new System.EventHandler(this.timerSync_Tick);
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.AutoSize = false;
            this.trackBarVolume.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarVolume.Location = new System.Drawing.Point(158, 5);
            this.trackBarVolume.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(94, 26);
            this.trackBarVolume.SmallChange = 5;
            this.trackBarVolume.TabIndex = 2;
            this.trackBarVolume.TickFrequency = 10;
            this.trackBarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarVolume.Value = 50;
            this.trackBarVolume.ValueChanged += new System.EventHandler(this.trackBarVolume_ValueChanged);
            this.trackBarVolume.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarVolume_MouseUp);
            // 
            // tablePanel
            // 
            this.tablePanel.BackColor = System.Drawing.SystemColors.Control;
            this.tablePanel.ColumnCount = 3;
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tablePanel.Controls.Add(this.trackBarVolume, 2, 0);
            this.tablePanel.Controls.Add(this.progressSeek, 0, 0);
            this.tablePanel.Controls.Add(this.labelTime, 1, 0);
            this.tablePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tablePanel.Location = new System.Drawing.Point(0, 86);
            this.tablePanel.Name = "tablePanel";
            this.tablePanel.RowCount = 1;
            this.tablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanel.Size = new System.Drawing.Size(255, 34);
            this.tablePanel.TabIndex = 1;
            // 
            // progressSeek
            // 
            this.progressSeek.BackColor = System.Drawing.Color.White;
            this.progressSeek.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressSeek.ForeColor = System.Drawing.Color.LimeGreen;
            this.progressSeek.Location = new System.Drawing.Point(9, 10);
            this.progressSeek.Margin = new System.Windows.Forms.Padding(9, 10, 9, 11);
            this.progressSeek.Maximum = 5000;
            this.progressSeek.Name = "progressSeek";
            this.progressSeek.Size = new System.Drawing.Size(62, 13);
            this.progressSeek.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressSeek.TabIndex = 0;
            this.progressSeek.Click += new System.EventHandler(this.progressSeek_Click);
            // 
            // labelTime
            // 
            this.labelTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTime.Location = new System.Drawing.Point(80, 2);
            this.labelTime.Margin = new System.Windows.Forms.Padding(0, 2, 0, 5);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(75, 27);
            this.labelTime.TabIndex = 1;
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timerData
            // 
            this.timerData.Interval = 500;
            this.timerData.Tick += new System.EventHandler(this.timerData_Tick);
            // 
            // FormPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(255, 120);
            this.ControlBox = false;
            this.Controls.Add(this.tablePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(-32000, -32000);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormPlayer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TweetDuck Video";
            this.Load += new System.EventHandler(this.FormPlayer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).EndInit();
            this.tablePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerSync;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.TableLayoutPanel tablePanel;
        private FlatProgressBar progressSeek;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Timer timerData;
    }
}

