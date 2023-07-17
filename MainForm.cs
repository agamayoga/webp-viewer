using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Manipura
{
    public partial class MainForm : Form
    {
        public string WorkingDirectory = null;
        public string CurrentFileName = null;

        public MainForm()
        {
            InitializeComponent();

            //Get command line arguments
            string[] args = Environment.GetCommandLineArgs();

#if DEBUG
            //Debug only - simulate command line arguments
            //args = new string[] { "webpviewer.exe", @"..\..\Samples\5-seashell.webp" };
#endif

            //Get initial path - first argument after assembly name
            string initialPath = args != null && args.Length > 1 ? args[1] : null;

            //In case file argument not set - browse for a file
            if (string.IsNullOrEmpty(initialPath))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "WebP format (*.webp)|*.webp";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    initialPath = dialog.FileName;
                }

                if (string.IsNullOrEmpty(initialPath))
                {
                    Environment.Exit(0);
                }
            }

            //File not found message
            if (!File.Exists(initialPath))
            {
                MessageBox.Show("File not found: " + initialPath, "File not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Environment.Exit(1);
            }

            //Set working directory
            this.WorkingDirectory = Path.GetDirectoryName(initialPath) ?? ".";

            //Show image
            LoadImage(initialPath);
        }
        
        protected void LoadImage(string path)
        {
            try
            {
                //Try to read the file as WebP format
                var decoder = new WebPDecoder();
                using (Stream inputStream = File.Open(path, System.IO.FileMode.Open))
                {
                    var bytes = ReadToEnd(inputStream);
                    var outBitmap = decoder.DecodeFromBytes(bytes, bytes.LongLength);
                    imgView.Image = outBitmap;
                }
            }
            catch (Exception)
            {
                try
                {
                    //Try to read the file as any other Windows supported format
                    using (Stream inputStream = File.Open(path, System.IO.FileMode.Open))
                    {
                        var bitmap = Image.FromStream(inputStream);
                        imgView.Image = bitmap;
                    }
                }
                catch (Exception)
                {
                    //No image
                    imgView.Image = null;
                }
            }
            
            this.CurrentFileName = Path.GetFileName(path);
            this.Text = "WebP Viewer - " + this.CurrentFileName;
        }

        protected void FindNeighbourImages(out string prev, out string next)
        {
            prev = null;
            next = null;

            string current = Path.GetFileName(this.CurrentFileName).ToLowerInvariant();

            var allfiles = Directory.GetFiles(this.WorkingDirectory);
            if (allfiles.Length > 1)
            {
                var files = new List<string>();
                var allowed = "*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tif;*.tiff;*.webp".Replace("*", "").Split(';');
                foreach (string file in allfiles)
                {
                    string ext = Path.GetExtension(file).ToLower();
                    for (int i = 0; i < allowed.Length; i++)
                    {
                        if (allowed[i] == ext)
                        {
                            files.Add(file);
                        }
                    }
                }
                for (int i = 0; i < files.Count; i++)
                {
                    string file = Path.GetFileName(files[i]).ToLowerInvariant();
                    if (file == current)
                    {
                        prev = (i == 0 ? files[files.Count - 1] : files[i - 1]);
                        next = (i == files.Count - 1 ? files[0] : files[i + 1]);
                        break;
                    }
                }
            }
        }

        protected byte[] ReadToEnd(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            string prev = null;
            string next = null;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    FindNeighbourImages(out prev, out next);
                    if (prev != null)
                    {
                        LoadImage(prev);
                    }
                    break;

                case Keys.Right:
                    FindNeighbourImages(out prev, out next);
                    if (next != null)
                    {
                        LoadImage(next);
                    }
                    break;
            }
        }
    }
}
