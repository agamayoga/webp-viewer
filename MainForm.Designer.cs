namespace Agama
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.imgView = new System.Windows.Forms.PictureBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.itemSaveAsJpeg = new System.Windows.Forms.ToolStripMenuItem();
            this.itemSaveAsPng = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.imgView)).BeginInit();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgView
            // 
            this.imgView.ContextMenuStrip = this.contextMenu;
            this.imgView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgView.Location = new System.Drawing.Point(0, 0);
            this.imgView.Name = "imgView";
            this.imgView.Size = new System.Drawing.Size(784, 562);
            this.imgView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgView.TabIndex = 0;
            this.imgView.TabStop = false;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemSaveAs});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(181, 48);
            // 
            // itemSaveAs
            // 
            this.itemSaveAs.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemSaveAsJpeg,
            this.itemSaveAsPng});
            this.itemSaveAs.Name = "itemSaveAs";
            this.itemSaveAs.Size = new System.Drawing.Size(180, 22);
            this.itemSaveAs.Text = "Save as...";
            // 
            // itemSaveAsJpeg
            // 
            this.itemSaveAsJpeg.Name = "itemSaveAsJpeg";
            this.itemSaveAsJpeg.Size = new System.Drawing.Size(180, 22);
            this.itemSaveAsJpeg.Text = "JPEG";
            this.itemSaveAsJpeg.Click += new System.EventHandler(this.itemSaveAsJpeg_Click);
            // 
            // itemSaveAsPng
            // 
            this.itemSaveAsPng.Name = "itemSaveAsPng";
            this.itemSaveAsPng.Size = new System.Drawing.Size(180, 22);
            this.itemSaveAsPng.Text = "PNG";
            this.itemSaveAsPng.Click += new System.EventHandler(this.itemSaveAsPng_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.imgView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "WebP Viewer";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.imgView)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgView;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem itemSaveAs;
        private System.Windows.Forms.ToolStripMenuItem itemSaveAsJpeg;
        private System.Windows.Forms.ToolStripMenuItem itemSaveAsPng;
    }
}

