using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public partial class ArrayLayoutControl : UserControl {

        public ArraySpec Array { get; set; }

        public HashSet<Color> SelectedColors { get; private set; }

        public ArrayLayoutControl() {
            // init viewmodel
            SelectedColors = new HashSet<Color>();

            // init view
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.ContainerControl |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor, 
                          true);
        }

        public event EventHandler SelectionChanged;

        public HashSet<String> SelectedIDs {
            get {
                HashSet<String> ret = new HashSet<string>();
                foreach (Color col in SelectedColors) {
                    if (!Array.CellIDs.ContainsKey(col)) {
                        Array.CellIDs.Add(col, ColorTranslator.ToHtml(col));
                    }
                    ret.Add(Array.CellIDs[col]);
                }
                return ret;
            }
        }

        private RectangleF GetArrayLayoutRect() {
            double texW = Array.LayoutTexture.Width;
            double texH = Array.LayoutTexture.Height;
            double scale = Math.Min(Width / texW, Height / texH);
            return new RectangleF(0, 0, (float)(scale * texW), (float)(scale * texH));
        }

        private Rectangle? GetSelectionRect() {
            if(dragPointA == null || dragPointB == null){
                return null;
            }
            int minX = Math.Min(dragPointA.Value.X, dragPointB.Value.X);
            int maxX = Math.Max(dragPointA.Value.X, dragPointB.Value.X);
            int minY = Math.Min(dragPointA.Value.Y, dragPointB.Value.Y);
            int maxY = Math.Max(dragPointA.Value.Y, dragPointB.Value.Y);
            return new Rectangle(minX, minY, maxX-minX, maxY-minY);
        }

        /// <summary>
        /// Gets the array layout texture.
        /// </summary>
        private Bitmap cacheTex;
        private Color[,] cachePixels;
        private Color[,] GetPixels() {
            Debug.Assert(Array != null && Array.LayoutTexture != null);
            Bitmap tex = Array.LayoutTexture;
            if (tex == cacheTex) {
                return cachePixels;
            }
            int w = tex.Width, h = tex.Height;
            Color[,] ret = new Color[w,h];
            Debug.WriteLine("copying out array layout texture pixels");

            // jump thru some hoops to read Bitmap data efficiently
            BitmapData texData = tex.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe {
                int* pixels = (int*)texData.Scan0.ToPointer();
                for (int i = 0; i < w; i++){
                    for(int j = 0; j < h; j++){
                        Color pixelColor = Color.FromArgb(pixels[j*w + i] | unchecked((int)0xff000000));
                        Debug.Assert(pixelColor.A == 255);
                        ret[i,j] = pixelColor;
                    }
                }
            }
            tex.UnlockBits(texData);

            cacheTex = tex;
            cachePixels = ret;
            return ret;
        }

        private void RecomputeSelection() {
            Bitmap tex = Array.LayoutTexture;
            int w = tex.Width, h = tex.Height;

            // find which pixels we've selected
            RectangleF rectLayout = GetArrayLayoutRect();
            RectangleF rectSel = GetSelectionRect().Value;
            int minX = (int)(w * (rectSel.Left - rectLayout.X) / rectLayout.Width);
            int maxX = (int)(w * (rectSel.Right - rectLayout.X) / rectLayout.Width);
            int minY = (int)(h * (rectSel.Top - rectLayout.Y) / rectLayout.Height);
            int maxY = (int)(h * (rectSel.Bottom - rectLayout.Y) / rectLayout.Height);

            // find which solar cells we've selected
            Color[,] pixels = GetPixels();
            HashSet<Color> newSelection = new HashSet<Color>();
            for (int j = minY; j <= maxY; j++) {
                for (int i = minX; i <= maxX; i++) {
                    if (i < 0 || j < 0 || i >= w || j >= h) {
                        continue;
                    }
                    Color pixelColor = pixels[i,j];
                    // ignore grayscale
                    if (pixelColor.B == pixelColor.G && pixelColor.G == pixelColor.R) {
                        continue;
                    }
                    newSelection.Add(pixelColor);
                }
            }
            if (!newSelection.SetEquals(SelectedColors)) {
                SelectedColors = newSelection;
                if (SelectionChanged != null) {
                    SelectionChanged(this, null);
                }
            }
        }

        private Bitmap texSelected;
        protected override void OnPaint(PaintEventArgs e) {
            // draw the background
            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            if(Array == null || Array.LayoutTexture == null){
                return;
            }

            // draw the array layout
            RectangleF arrayLayoutRect = GetArrayLayoutRect();
            g.DrawImage(Array.LayoutTexture, arrayLayoutRect);

            // highlight the selected cells
            Color[,] pixels = GetPixels();
            int w = Array.LayoutTexture.Width, h = Array.LayoutTexture.Height;
            if (texSelected == null || texSelected.Width != w || texSelected.Height != h) {
                texSelected = new Bitmap(w, h);
            }
            BitmapData texSelData = texSelected.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            unsafe {
                uint* pixelSel = (uint*)texSelData.Scan0.ToPointer();
                for (int j = 0; j < h; j++) {
                    for (int i = 0; i < w; i++) {
                        bool sel = SelectedColors.Contains(pixels[i, j]);
                        bool mask = (j + i) % 20 < 10;
                        uint alpha = (uint)((sel && mask) ? 0x80 : 0x00);
                        uint color = 0xffffff | (alpha << 24); // white highlight
                        pixelSel[j * w + i] = color;
                    }
                }
            }
            texSelected.UnlockBits(texSelData);
            g.DrawImage(texSelected, arrayLayoutRect);
            
            // draw the selection box
            Rectangle? rect = GetSelectionRect();
            if (rect != null) {
                Pen stroke = Pens.White;
                Brush fill = new SolidBrush(Color.FromArgb(128, 255, 255, 255));
                g.FillRectangle(fill, rect.Value);
                g.DrawRectangle(stroke, rect.Value);
            }
        }

        Point? dragPointA, dragPointB;
        protected override void OnMouseDown(MouseEventArgs e) {
            dragPointA = new Point(e.X, e.Y);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            dragPointA = dragPointB = null;
            Refresh();
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            if (dragPointA == null) {
                return;
            }
            dragPointB = new Point(e.X, e.Y);
            RecomputeSelection();
            Refresh();
        }
    }
}
