namespace CF_TimeSpanSlider
{
    partial class CF_TimeSpanSlider
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_1h = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_30min = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_15min = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_10min = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_5min = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_1min = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(145, 28);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_1h,
            this.TSMI_30min,
            this.TSMI_15min,
            this.TSMI_10min,
            this.TSMI_5min,
            this.TSMI_1min});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(144, 24);
            this.toolStripMenuItem1.Text = "Timescale";
            // 
            // TSMI_1h
            // 
            this.TSMI_1h.Name = "TSMI_1h";
            this.TSMI_1h.Size = new System.Drawing.Size(156, 26);
            this.TSMI_1h.Text = "1 hour";
            this.TSMI_1h.Click += new System.EventHandler(this.TSMI_1h_Click);
            // 
            // TSMI_30min
            // 
            this.TSMI_30min.Name = "TSMI_30min";
            this.TSMI_30min.Size = new System.Drawing.Size(156, 26);
            this.TSMI_30min.Text = "30 minutes";
            this.TSMI_30min.Click += new System.EventHandler(this.TSMI_30min_Click);
            // 
            // TSMI_15min
            // 
            this.TSMI_15min.Name = "TSMI_15min";
            this.TSMI_15min.Size = new System.Drawing.Size(156, 26);
            this.TSMI_15min.Text = "15 minutes";
            this.TSMI_15min.Click += new System.EventHandler(this.TSMI_15min_Click);
            // 
            // TSMI_10min
            // 
            this.TSMI_10min.Name = "TSMI_10min";
            this.TSMI_10min.Size = new System.Drawing.Size(156, 26);
            this.TSMI_10min.Text = "10 minutes";
            this.TSMI_10min.Click += new System.EventHandler(this.TSMI_10min_Click);
            // 
            // TSMI_5min
            // 
            this.TSMI_5min.Name = "TSMI_5min";
            this.TSMI_5min.Size = new System.Drawing.Size(156, 26);
            this.TSMI_5min.Text = "5 minutes";
            this.TSMI_5min.Click += new System.EventHandler(this.TSMI_5min_Click);
            // 
            // TSMI_1min
            // 
            this.TSMI_1min.Name = "TSMI_1min";
            this.TSMI_1min.Size = new System.Drawing.Size(156, 26);
            this.TSMI_1min.Text = "1 minute";
            this.TSMI_1min.Click += new System.EventHandler(this.TSMI_1min_Click);
            // 
            // CF_TimeSpanSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CausesValidation = false;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CF_TimeSpanSlider";
            this.Size = new System.Drawing.Size(964, 96);
            this.Load += new System.EventHandler(this.CF_TimeSpanSlider_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem TSMI_1h;
        private System.Windows.Forms.ToolStripMenuItem TSMI_30min;
        private System.Windows.Forms.ToolStripMenuItem TSMI_15min;
        private System.Windows.Forms.ToolStripMenuItem TSMI_10min;
        private System.Windows.Forms.ToolStripMenuItem TSMI_5min;
        private System.Windows.Forms.ToolStripMenuItem TSMI_1min;


    }
}
