namespace TweetDck.Plugins.Controls {
    partial class PluginListControl {
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
            this.flowLayoutPlugins = new TweetDck.Plugins.Controls.PluginListFlowLayout();
            this.btnTabCustom = new TweetDck.Plugins.Controls.TabButton();
            this.btnTabOfficial = new TweetDck.Plugins.Controls.TabButton();
            this.SuspendLayout();
            // 
            // flowLayoutPlugins
            // 
            this.flowLayoutPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPlugins.AutoScroll = true;
            this.flowLayoutPlugins.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPlugins.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPlugins.Location = new System.Drawing.Point(0, 29);
            this.flowLayoutPlugins.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPlugins.Name = "flowLayoutPlugins";
            this.flowLayoutPlugins.Size = new System.Drawing.Size(705, 321);
            this.flowLayoutPlugins.TabIndex = 6;
            this.flowLayoutPlugins.WrapContents = false;
            this.flowLayoutPlugins.Resize += new System.EventHandler(this.flowLayoutPlugins_Resize);
            // 
            // btnTabCustom
            // 
            this.btnTabCustom.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnTabCustom.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnTabCustom.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnTabCustom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabCustom.Location = new System.Drawing.Point(90, 0);
            this.btnTabCustom.Margin = new System.Windows.Forms.Padding(0);
            this.btnTabCustom.Name = "btnTabCustom";
            this.btnTabCustom.Size = new System.Drawing.Size(91, 30);
            this.btnTabCustom.TabIndex = 8;
            this.btnTabCustom.Text = "Custom";
            this.btnTabCustom.UseVisualStyleBackColor = true;
            this.btnTabCustom.Click += new System.EventHandler(this.btnTabCustom_Click);
            // 
            // btnTabOfficial
            // 
            this.btnTabOfficial.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnTabOfficial.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.btnTabOfficial.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.btnTabOfficial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTabOfficial.Location = new System.Drawing.Point(0, 0);
            this.btnTabOfficial.Margin = new System.Windows.Forms.Padding(0);
            this.btnTabOfficial.Name = "btnTabOfficial";
            this.btnTabOfficial.Size = new System.Drawing.Size(91, 30);
            this.btnTabOfficial.TabIndex = 7;
            this.btnTabOfficial.Text = "Official";
            this.btnTabOfficial.UseVisualStyleBackColor = true;
            this.btnTabOfficial.Click += new System.EventHandler(this.btnTabOfficial_Click);
            // 
            // PluginListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnTabCustom);
            this.Controls.Add(this.btnTabOfficial);
            this.Controls.Add(this.flowLayoutPlugins);
            this.Name = "PluginListControl";
            this.Size = new System.Drawing.Size(705, 350);
            this.ResumeLayout(false);

        }

        #endregion

        private TabButton btnTabCustom;
        private TabButton btnTabOfficial;
        private PluginListFlowLayout flowLayoutPlugins;
    }
}
