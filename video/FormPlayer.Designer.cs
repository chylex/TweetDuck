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
            this.tablePanelFull = new System.Windows.Forms.TableLayoutPanel();
            this.progressSeek = new TweetDuck.Video.Controls.SeekBar();
            this.labelTime = new System.Windows.Forms.Label();
            this.imageResize = new System.Windows.Forms.PictureBox();
            this.imageDownload = new System.Windows.Forms.PictureBox();
            this.imageClose = new System.Windows.Forms.PictureBox();
            this.timerData = new System.Windows.Forms.Timer(this.components);
            this.tablePanelCompactBottom = new System.Windows.Forms.TableLayoutPanel();
            this.tablePanelCompactTop = new System.Windows.Forms.TableLayoutPanel();
            this.labelTooltip = new TweetDuck.Video.Controls.LabelTooltip();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume)).BeginInit();
            this.tablePanelFull.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageResize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageDownload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageClose)).BeginInit();
            this.SuspendLayout();
            // 
            // timerSync
            // 
            this.timerSync.Interval = 15;
            this.timerSync.Tick += new System.EventHandler(this.timerSync_Tick);
            // 
            // trackBarVolume
            // 
            this.trackBarVolume.AutoSize = false;
            this.trackBarVolume.BackColor = System.Drawing.SystemColors.Control;
            this.trackBarVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarVolume.Location = new System.Drawing.Point(212, 5);
            this.trackBarVolume.Margin = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(130, 26);
            this.trackBarVolume.TabIndex = 2;
            this.trackBarVolume.TickFrequency = 10;
            this.trackBarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarVolume.Value = 50;
            this.trackBarVolume.ValueChanged += new System.EventHandler(this.trackBarVolume_ValueChanged);
            this.trackBarVolume.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarVolume_MouseDown);
            this.trackBarVolume.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBarVolume_MouseUp);
            // 
            // tablePanelFull
            // 
            this.tablePanelFull.BackColor = System.Drawing.SystemColors.Control;
            this.tablePanelFull.ColumnCount = 6;
            this.tablePanelFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tablePanelFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tablePanelFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tablePanelFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tablePanelFull.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tablePanelFull.Controls.Add(this.trackBarVolume, 3, 0);
            this.tablePanelFull.Controls.Add(this.progressSeek, 1, 0);
            this.tablePanelFull.Controls.Add(this.labelTime, 2, 0);
            this.tablePanelFull.Controls.Add(this.imageResize, 5, 0);
            this.tablePanelFull.Controls.Add(this.imageDownload, 4, 0);
            this.tablePanelFull.Controls.Add(this.imageClose, 0, 0);
            this.tablePanelFull.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tablePanelFull.Enabled = false;
            this.tablePanelFull.Location = new System.Drawing.Point(0, 86);
            this.tablePanelFull.Name = "tablePanelFull";
            this.tablePanelFull.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tablePanelFull.RowCount = 1;
            this.tablePanelFull.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelFull.Size = new System.Drawing.Size(400, 34);
            this.tablePanelFull.TabIndex = 0;
            this.tablePanelFull.Visible = false;
            // 
            // progressSeek
            // 
            this.progressSeek.BackColor = System.Drawing.Color.White;
            this.progressSeek.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressSeek.ForeColor = System.Drawing.Color.LimeGreen;
            this.progressSeek.Location = new System.Drawing.Point(39, 10);
            this.progressSeek.Margin = new System.Windows.Forms.Padding(9, 10, 8, 10);
            this.progressSeek.Maximum = 5000;
            this.progressSeek.Name = "progressSeek";
            this.progressSeek.Size = new System.Drawing.Size(85, 14);
            this.progressSeek.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressSeek.TabIndex = 0;
            this.progressSeek.MouseDown += new System.Windows.Forms.MouseEventHandler(this.progressSeek_MouseDown);
            // 
            // labelTime
            // 
            this.labelTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTime.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelTime.Location = new System.Drawing.Point(132, 3);
            this.labelTime.Margin = new System.Windows.Forms.Padding(0, 3, 0, 5);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(80, 26);
            this.labelTime.TabIndex = 1;
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // imageResize
            // 
            this.imageResize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imageResize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageResize.Image = global::TweetDuck.Video.Properties.Resources.btnResize;
            this.imageResize.Location = new System.Drawing.Point(373, 5);
            this.imageResize.Margin = new System.Windows.Forms.Padding(3, 5, 3, 7);
            this.imageResize.Name = "imageResize";
            this.imageResize.Size = new System.Drawing.Size(22, 22);
            this.imageResize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imageResize.TabIndex = 3;
            this.imageResize.TabStop = false;
            this.imageResize.WaitOnLoad = true;
            this.imageResize.Click += new System.EventHandler(this.imageResize_Click);
            // 
            // imageDownload
            // 
            this.imageDownload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imageDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageDownload.Image = global::TweetDuck.Video.Properties.Resources.btnDownload;
            this.imageDownload.Location = new System.Drawing.Point(345, 5);
            this.imageDownload.Margin = new System.Windows.Forms.Padding(3, 5, 3, 7);
            this.imageDownload.Name = "imageDownload";
            this.imageDownload.Size = new System.Drawing.Size(22, 22);
            this.imageDownload.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imageDownload.TabIndex = 4;
            this.imageDownload.TabStop = false;
            this.imageDownload.WaitOnLoad = true;
            this.imageDownload.Click += new System.EventHandler(this.imageDownload_Click);
            // 
            // imageClose
            // 
            this.imageClose.BackColor = System.Drawing.SystemColors.Control;
            this.imageClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imageClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageClose.Image = global::TweetDuck.Video.Properties.Resources.btnClose;
            this.imageClose.Location = new System.Drawing.Point(5, 5);
            this.imageClose.Margin = new System.Windows.Forms.Padding(3, 5, 3, 7);
            this.imageClose.Name = "imageClose";
            this.imageClose.Size = new System.Drawing.Size(22, 22);
            this.imageClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.imageClose.TabIndex = 5;
            this.imageClose.TabStop = false;
            this.imageClose.WaitOnLoad = true;
            this.imageClose.Click += new System.EventHandler(this.imageClose_Click);
            // 
            // timerData
            // 
            this.timerData.Interval = 500;
            this.timerData.Tick += new System.EventHandler(this.timerData_Tick);
            // 
            // tablePanelCompactBottom
            // 
            this.tablePanelCompactBottom.BackColor = System.Drawing.SystemColors.Control;
            this.tablePanelCompactBottom.ColumnCount = 5;
            this.tablePanelCompactBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tablePanelCompactBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelCompactBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tablePanelCompactBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tablePanelCompactBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tablePanelCompactBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tablePanelCompactBottom.Enabled = false;
            this.tablePanelCompactBottom.Location = new System.Drawing.Point(0, 52);
            this.tablePanelCompactBottom.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tablePanelCompactBottom.Name = "tablePanelCompactBottom";
            this.tablePanelCompactBottom.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.tablePanelCompactBottom.RowCount = 1;
            this.tablePanelCompactBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelCompactBottom.Size = new System.Drawing.Size(400, 34);
            this.tablePanelCompactBottom.TabIndex = 1;
            // 
            // tablePanelCompactTop
            // 
            this.tablePanelCompactTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tablePanelCompactTop.BackColor = System.Drawing.SystemColors.Control;
            this.tablePanelCompactTop.ColumnCount = 2;
            this.tablePanelCompactTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelCompactTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tablePanelCompactTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tablePanelCompactTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tablePanelCompactTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tablePanelCompactTop.Enabled = false;
            this.tablePanelCompactTop.Location = new System.Drawing.Point(0, 60);
            this.tablePanelCompactTop.Margin = new System.Windows.Forms.Padding(0);
            this.tablePanelCompactTop.Name = "tablePanelCompactTop";
            this.tablePanelCompactTop.Padding = new System.Windows.Forms.Padding(2, 0, 4, 0);
            this.tablePanelCompactTop.RowCount = 1;
            this.tablePanelCompactTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelCompactTop.Size = new System.Drawing.Size(400, 34);
            this.tablePanelCompactTop.TabIndex = 2;
            this.tablePanelCompactTop.Visible = false;
            // 
            // labelTooltip
            // 
            this.labelTooltip.AutoSize = true;
            this.labelTooltip.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.labelTooltip.ForeColor = System.Drawing.Color.White;
            this.labelTooltip.Location = new System.Drawing.Point(0, 0);
            this.labelTooltip.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.labelTooltip.Name = "labelTooltip";
            this.labelTooltip.Padding = new System.Windows.Forms.Padding(4, 2, 2, 2);
            this.labelTooltip.Size = new System.Drawing.Size(6, 19);
            this.labelTooltip.TabIndex = 3;
            this.labelTooltip.Visible = false;
            // 
            // FormPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(400, 120);
            this.ControlBox = false;
            this.Controls.Add(this.labelTooltip);
            this.Controls.Add(this.tablePanelCompactBottom);
            this.Controls.Add(this.tablePanelFull);
            this.Controls.Add(this.tablePanelCompactTop);
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
            this.tablePanelFull.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageResize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageDownload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerSync;
        private System.Windows.Forms.TrackBar trackBarVolume;
        private System.Windows.Forms.TableLayoutPanel tablePanelFull;
        private Controls.SeekBar progressSeek;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Timer timerData;
        private Controls.LabelTooltip labelTooltip;
        private System.Windows.Forms.PictureBox imageResize;
        private System.Windows.Forms.PictureBox imageDownload;
        private System.Windows.Forms.PictureBox imageClose;
        private System.Windows.Forms.TableLayoutPanel tablePanelCompactBottom;
        private System.Windows.Forms.TableLayoutPanel tablePanelCompactTop;
    }
}

