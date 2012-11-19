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
            SelectedColors.Clear();
            BitmapData texData = tex.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe {
                int* pixels = (int*)texData.Scan0.ToPointer();
                for (int j = minY; j <= maxY; j++) {
                    for (int i = minX; i <= maxX; i++) {
                        if (i < 0 || j < 0 || i >= w || j >= h) {
                            continue;
                        }
                        Color pixelColor = Color.FromArgb(pixels[j*w + i] | unchecked((int)0xff000000));
                        Debug.Assert(pixelColor.A == 255);
                        // ignore grayscale
                        if (pixelColor.B == pixelColor.G && pixelColor.G == pixelColor.R) {
                            continue;
                        }
                        SelectedColors.Add(pixelColor);
                    }
                }
            }
            tex.UnlockBits(texData);
        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            if(Array == null || Array.LayoutTexture == null){
                return;
            }
            g.DrawImage(Array.LayoutTexture, GetArrayLayoutRect());
            
            // draw the selection
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
            Debug.WriteLine(SelectedColors.Count + " colors selected");
            Refresh();
        }
    }
}
