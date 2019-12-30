namespace emulator_gui {
    partial class GBForm {
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.goToGBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sDLTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openGBRomDialog = new System.Windows.Forms.OpenFileDialog();
			this.gbPanel = new System.Windows.Forms.Panel();
			this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.goToGBToolStripMenuItem,
            this.sDLTestToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(281, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
			// 
			// openROMToolStripMenuItem
			// 
			this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
			this.openROMToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
			this.openROMToolStripMenuItem.Text = "Open ROM";
			this.openROMToolStripMenuItem.Click += new System.EventHandler(this.openROMToolStripMenuItem_Click);
			// 
			// goToGBToolStripMenuItem
			// 
			this.goToGBToolStripMenuItem.Name = "goToGBToolStripMenuItem";
			this.goToGBToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
			this.goToGBToolStripMenuItem.Text = "Go To GBA";
			this.goToGBToolStripMenuItem.Click += new System.EventHandler(this.goToGBToolStripMenuItem_Click);
			// 
			// sDLTestToolStripMenuItem
			// 
			this.sDLTestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.runToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.setColorToolStripMenuItem});
			this.sDLTestToolStripMenuItem.Name = "sDLTestToolStripMenuItem";
			this.sDLTestToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
			this.sDLTestToolStripMenuItem.Text = "SDL Test";
			// 
			// openGBRomDialog
			// 
			this.openGBRomDialog.Filter = "GB Roms|*.gb|All files|*.*";
			// 
			// gbPanel
			// 
			this.gbPanel.Location = new System.Drawing.Point(12, 27);
			this.gbPanel.Name = "gbPanel";
			this.gbPanel.Size = new System.Drawing.Size(194, 288);
			this.gbPanel.TabIndex = 2;
			this.gbPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gbPanel_Paint);
			// 
			// runToolStripMenuItem
			// 
			this.runToolStripMenuItem.Name = "runToolStripMenuItem";
			this.runToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.runToolStripMenuItem.Text = "Run";
			this.runToolStripMenuItem.Click += new System.EventHandler(this.RunToolStripMenuItem_Click);
			// 
			// stopToolStripMenuItem
			// 
			this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
			this.stopToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.stopToolStripMenuItem.Text = "Stop";
			this.stopToolStripMenuItem.Click += new System.EventHandler(this.StopToolStripMenuItem_Click);
			// 
			// startToolStripMenuItem
			// 
			this.startToolStripMenuItem.Name = "startToolStripMenuItem";
			this.startToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.startToolStripMenuItem.Text = "Start";
			this.startToolStripMenuItem.Click += new System.EventHandler(this.StartToolStripMenuItem_Click);
			// 
			// setColorToolStripMenuItem
			// 
			this.setColorToolStripMenuItem.Name = "setColorToolStripMenuItem";
			this.setColorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.setColorToolStripMenuItem.Text = "Set Color";
			this.setColorToolStripMenuItem.Click += new System.EventHandler(this.SetColorToolStripMenuItem_Click);
			// 
			// GBForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(281, 327);
			this.Controls.Add(this.gbPanel);
			this.Controls.Add(this.menuStrip1);
			this.Name = "GBForm";
			this.Text = "GBForm";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToGBToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openGBRomDialog;
        private System.Windows.Forms.Panel gbPanel;
		private System.Windows.Forms.ToolStripMenuItem sDLTestToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setColorToolStripMenuItem;
	}
}