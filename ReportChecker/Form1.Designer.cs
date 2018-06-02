namespace ReportChecker
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tvResults = new System.Windows.Forms.TreeView();
            this.ilResults = new System.Windows.Forms.ImageList(this.components);
            this.mnuAlerts = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideAlertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAlertsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAlerts.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.button1.Location = new System.Drawing.Point(802, 271);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.SystemColors.Window;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.Location = new System.Drawing.Point(51, 42);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(393, 212);
            this.listBox1.TabIndex = 1;
            // 
            // tvResults
            // 
            this.tvResults.BackColor = System.Drawing.SystemColors.Window;
            this.tvResults.ImageIndex = 0;
            this.tvResults.ImageList = this.ilResults;
            this.tvResults.Location = new System.Drawing.Point(502, 42);
            this.tvResults.Name = "tvResults";
            this.tvResults.SelectedImageIndex = 0;
            this.tvResults.Size = new System.Drawing.Size(375, 212);
            this.tvResults.TabIndex = 2;
            // 
            // ilResults
            // 
            this.ilResults.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilResults.ImageStream")));
            this.ilResults.TransparentColor = System.Drawing.Color.Transparent;
            this.ilResults.Images.SetKeyName(0, "blank.bmp");
            this.ilResults.Images.SetKeyName(1, "alert.bmp");
            // 
            // mnuAlerts
            // 
            this.mnuAlerts.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideAlertsToolStripMenuItem,
            this.showAlertsToolStripMenuItem});
            this.mnuAlerts.Name = "mnuAlerts";
            this.mnuAlerts.Size = new System.Drawing.Size(137, 48);
            this.mnuAlerts.Text = " Eeek";
            // 
            // hideAlertsToolStripMenuItem
            // 
            this.hideAlertsToolStripMenuItem.Name = "hideAlertsToolStripMenuItem";
            this.hideAlertsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.hideAlertsToolStripMenuItem.Text = "Hide Alerts";
            this.hideAlertsToolStripMenuItem.Click += new System.EventHandler(this.hideAlertsToolStripMenuItem_Click);
            // 
            // showAlertsToolStripMenuItem
            // 
            this.showAlertsToolStripMenuItem.Enabled = false;
            this.showAlertsToolStripMenuItem.Name = "showAlertsToolStripMenuItem";
            this.showAlertsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.showAlertsToolStripMenuItem.Text = "Show Alerts";
            this.showAlertsToolStripMenuItem.Click += new System.EventHandler(this.hideAlertsToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(205)))), ((int)(((byte)(215)))));
            this.ClientSize = new System.Drawing.Size(925, 334);
            this.Controls.Add(this.tvResults);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Report Checker";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.mnuAlerts.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TreeView tvResults;
        private System.Windows.Forms.ImageList ilResults;
        private System.Windows.Forms.ContextMenuStrip mnuAlerts;
        private System.Windows.Forms.ToolStripMenuItem hideAlertsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAlertsToolStripMenuItem;
    }
}

