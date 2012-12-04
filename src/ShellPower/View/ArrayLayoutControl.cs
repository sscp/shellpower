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

        public int[,] SelectionMask { get; private set; }

        public event EventHandler SelectionChanged;

        private int frame = 0;
        private int nextId = 1;
        private Bitmap tex; // array layout
        private Bitmap texSelected; // overlay
        private Color[,] pixels;
        private int w, h;

        public ArrayLayoutControl() {
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


            System.Windows.Forms.Timer timer = new Timer();
            timer.Interval = 40;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
        }

        private RectangleF GetArrayLayoutRect() {
            double texW = Array.LayoutTexture.Width;
            double texH = Array.LayoutTexture.Height;
            double scale = Math.Min(Width / texW, Height / texH);
            return new RectangleF(0, 0, (float)(scale * texW), (float)(scale * texH));
        }

        /*private Rectangle? GetSelectionRect() {
            if(dragPointA == null || dragPointB == null){
                return null;
            }
            int minX = Math.Min(dragPointA.Value.X, dragPointB.Value.X);
            int maxX = Math.Max(dragPointA.Value.X, dragPointB.Value.X);
            int minY = Math.Min(dragPointA.Value.Y, dragPointB.Value.Y);
            int maxY = Math.Max(dragPointA.Value.Y, dragPointB.Value.Y);
            return new Rectangle(minX, minY, maxX-minX, maxY-minY);
        }*/

        /// <summary>
        /// Gets the array layout texture.
        /// </summary>
        private void CreateTextureIfNeeded() {
            Debug.Assert(Array != null && Array.LayoutTexture != null);
            Debug.Assert(tex == null || tex == Array.LayoutTexture);
            if (tex == Array.LayoutTexture) return; // already init'd

            // init texture:
            tex = Array.LayoutTexture;
            w = tex.Width;
            h = tex.Height;
            pixels = new Color[w,h];
            Debug.WriteLine("copying out array layout texture pixels");

            // jump thru some hoops to read Bitmap data efficiently
            BitmapData texData = tex.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe {
                int* ptr = (int*)texData.Scan0.ToPointer();
                for (int i = 0; i < w; i++){
                    for(int j = 0; j < h; j++){
                        Color pixelColor = Color.FromArgb(ptr[j*w + i] | unchecked((int)0xff000000));
                        Debug.Assert(pixelColor.A == 255);
                        pixels[i, j] = pixelColor;
                    }
                }
            }
            tex.UnlockBits(texData);

            // init select mask
            SelectionMask = new int[w, h];
        }

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
            if (SelectionMask == null) return;
            if (texSelected == null || texSelected.Width != w || texSelected.Height != h) {
                texSelected = new Bitmap(w, h);
            }
            BitmapData texSelData = texSelected.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            var selMask = SelectionMask;
            unsafe {
                uint* pixelSel = (uint*)texSelData.Scan0.ToPointer();
                for (int i = 0; i < w; i++) {
                    for (int j = 0; j < h; j++) {
                        bool sel = selMask[i, j] > 0;
                        bool mask = (j + i + frame) % 16 < 8;
                        uint alpha = (uint)((sel && mask) ? 0x80 : 0x00);
                        uint color = 0xffffff | (alpha << 24); // white highlight
                        pixelSel[j * w + i] = color;
                    }
                }
            }
            texSelected.UnlockBits(texSelData);
            g.DrawImage(texSelected, arrayLayoutRect);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            CreateTextureIfNeeded();
            RectangleF drawRect = GetArrayLayoutRect();
            int x = (int)((e.X - drawRect.X) * w / drawRect.Width);
            int y = (int)((e.Y - drawRect.Y) * h / drawRect.Height);
            if (x < 0 || x >= w || y < 0 || y >= h) return;

            // flood fill
            int selId = nextId++;
            HashSet<Pair<int>> ffS = new HashSet<Pair<int>>();
            Queue<Pair<int>> ffQ = new Queue<Pair<int>>();
            ffQ.Enqueue(new Pair<int>(x, y));
            Color color = pixels[x,y];
            if (color.R == color.G && color.G == color.B) {
                return; // grayscale, not selectable
            }
            while (ffQ.Count > 0) {
                Pair<int> xy = ffQ.Dequeue();

                // redundant?
                if (ffS.Contains(xy)) continue;
                ffS.Add(xy);

                // select the pixel
                SelectionMask[xy.First, xy.Second] = selId;

                // enqueue the neighbors
                for(int x2 = xy.First-1; x2 <= xy.First+1; x2++){
                    for(int y2 = xy.Second-1; y2 <= xy.Second+1; y2++){
                        if(x2 <= 0 || x2 >= w || y2 <= 0 || y2 >= h) continue;
                        if(pixels[xy.First, xy.Second] != color) continue;
                        ffQ.Enqueue(new Pair<int>(x2,y2));
                    }
                }
            }
            Refresh();
        }
        protected override void OnMouseMove(MouseEventArgs e) {
        }

        private void timer_Tick(object sender, EventArgs e) {
            frame++;
            Refresh();
        }
    }
}
