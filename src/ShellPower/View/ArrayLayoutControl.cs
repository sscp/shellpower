using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SSCP.ShellPower {
    /// <summary>
    /// Allows user to define individual strings.
    /// </summary>
    public partial class ArrayLayoutControl : UserControl {

        public ArraySpec _array;
        public ArraySpec.CellString _cellStr;
        public ArraySpec Array {
            get { return _array; }
            set { _array = value; Refresh(); }
        }
        public ArraySpec.CellString CellString {
            get { return _cellStr; }
            set { _cellStr = value; Refresh(); }
        }
        public bool Editable { get; set; }
        public bool EditBypassDiodes { get; set; }
        public bool AnimatedSelection { get; set; }
        public event EventHandler CellStringChanged;

        private int frame = 0;
        private int nextId = 1;
        private Bitmap tex; // array layout
        private Bitmap texSelected; // overlay
        private Color[,] pixels;
        private int w, h;
        // bypassCells count = [0,1,2] dep on ui state
        private List<ArraySpec.Cell> bypassCells = new List<ArraySpec.Cell>(); 

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

            // init model
            Editable = true;
            AnimatedSelection = false;

            // animate selection
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
        }
        private void UpdateTexSelected() {
            if (texSelected == null || texSelected.Width != w || texSelected.Height != h) {
                texSelected = new Bitmap(w, h);
            }
            BitmapData texSelData = texSelected.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            unsafe {
                uint* ptr = (uint*)texSelData.Scan0.ToPointer();
                for (int i = 0; i < w * h; i++) {
                    ptr[i] = 0;
                }
                if (CellString != null) {
                    foreach (ArraySpec.Cell cell in CellString.Cells) {
                        foreach (Pair<int> pixel in cell.Pixels) {
                            int i = pixel.First, j = pixel.Second;
                            bool mask = (j + i + frame) % 16 < 8;
                            uint alpha = (uint)(mask ? 0x80 : 0x00);
                            uint color = 0xffffff | (alpha << 24); // white highlight
                            ptr[j * w + i] = color;
                        }
                    }
                    foreach (ArraySpec.Cell cell in bypassCells) {
                        foreach (Pair<int> pixel in cell.Pixels) {
                            int i = pixel.First, j = pixel.Second;
                            uint alpha = ptr[j * w + i] >> 24;
                            if (alpha > 0) continue;
                            ptr[j * w + i] = 0x80ff0000;
                        }
                    }
                }
            }
            texSelected.UnlockBits(texSelData);
        }

        protected override void OnPaint(PaintEventArgs e) {
            // draw the background
            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            if(Array == null || Array.LayoutTexture == null){
                return;
            }
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // draw the array layout
            RectangleF arrayLayoutRect = GetArrayLayoutRect();
            g.DrawImage(Array.LayoutTexture, arrayLayoutRect);

            // highlight the selected cells
            if (tex == null) return;
            UpdateTexSelected();
            g.DrawImage(texSelected, arrayLayoutRect);

            // draw the wiring
            if (CellString == null) return;
            int n = CellString.Cells.Count;
            PointF[] points = new PointF[n];
            float scale = arrayLayoutRect.Width / Array.LayoutTexture.Width;
            for(int i = 0; i < n; i++){
                ArraySpec.Cell cell = CellString.Cells[i];
                int sx=0,sy=0;
                foreach (Pair<int> xy in cell.Pixels) {
                    sx += xy.First;
                    sy += xy.Second;
                }
                int m = cell.Pixels.Count;
                points[i] = new PointF(
                    (float)sx / m * scale + arrayLayoutRect.X, 
                    (float)sy / m * scale + arrayLayoutRect.Y);
            }
            if (points.Length > 1) {
                g.DrawLines(new Pen(Color.FromArgb(80, Color.Black), 3.0f), points);
                g.DrawLines(new Pen(Color.LightYellow, 1.0f), points);
            }

            // draw the bypass diodes
            int ndiodes = CellString.BypassDiodes.Count;
            for (int i = 0; i < ndiodes; i++) {
                ArraySpec.BypassDiode diode = CellString.BypassDiodes[i];
                PointF pA = points[diode.CellIxs.First];
                PointF pB = points[diode.CellIxs.Second];
                
                g.DrawLine(new Pen(Color.FromArgb(80, Color.Black), 4.0f), pA, pB);
                g.DrawLine(new Pen(Color.Red, 2.5f), pA, pB);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e) {
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            if (!Editable || CellString==null) return;

            // find click texture coords
            int x, y;
            if (!GetTexCoord(e, out x, out y)) return;
            Color color = pixels[x, y];
            if (ColorUtils.IsGrayscale(color)) return;

            // flood fill
            int selId = nextId++;
            HashSet<Pair<int>> ffS = new HashSet<Pair<int>>();
            Queue<Pair<int>> ffQ = new Queue<Pair<int>>();
            ffQ.Enqueue(new Pair<int>(x, y));
            while (ffQ.Count > 0) {
                Pair<int> xy = ffQ.Dequeue();

                // redundant?
                if (ffS.Contains(xy)) continue;
                ffS.Add(xy);

                // enqueue the neighbors
                for(int x2 = xy.First-1; x2 <= xy.First+1; x2++){
                    for(int y2 = xy.Second-1; y2 <= xy.Second+1; y2++){
                        if(x2 <= 0 || x2 >= w || y2 <= 0 || y2 >= h) continue;
                        if(pixels[x2,y2] != color) continue;
                        ffQ.Enqueue(new Pair<int>(x2,y2));
                    }
                }
            }

            // create the new cell
            ArraySpec.Cell newCell = new ArraySpec.Cell();
            newCell.Color = color;
            newCell.Pixels.AddRange(ffS);
            newCell.Pixels.Sort(new Comparison<Pair<int>>((a, b) => {
                if (a.Second < b.Second) return -1; // scan line order
                if (a.Second > b.Second) return 1;
                if (a.First < b.First) return -1;
                if (a.First > b.First) return 1;
                return 0;
            }));

            if (EditBypassDiodes) {
                if (CellString.Cells.Contains(newCell)) {
                    if(!bypassCells.Remove(newCell)){
                        bypassCells.Add(newCell);
                    }
                }
                if (bypassCells.Count == 2) {
                    int ix0 = CellString.Cells.IndexOf(bypassCells[0]);
                    int ix1 = CellString.Cells.IndexOf(bypassCells[1]);
                    ArraySpec.BypassDiode newDiode = new ArraySpec.BypassDiode();
                    newDiode.CellIxs = new Pair<int>(Math.Min(ix0, ix1), Math.Max(ix0, ix1));
                    if (!CellString.BypassDiodes.Remove(newDiode)) {
                        CellString.BypassDiodes.Add(newDiode);
                    }
                    bypassCells.Clear();
                }
            } else {
                // either add it to the current string, or remove it if it's already there
                if (!CellString.Cells.Remove(newCell)) {
                    CellString.Cells.Add(newCell);
                } else {
                    // prune bypass diodes
                    CellString.BypassDiodes.RemoveAll((diode) => {
                        return diode.CellIxs.First >= CellString.Cells.Count ||
                            diode.CellIxs.Second >= CellString.Cells.Count;
                    });
                }
            }

            if (CellStringChanged!=null) CellStringChanged(this, null);
            Refresh();
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            int x, y;
            if (GetTexCoord(e, out x, out y)) {
                if (ColorUtils.IsGrayscale(pixels[x, y])) {
                    this.Cursor = Cursors.Arrow; // not clickable
                } else {
                    this.Cursor = Cursors.Hand;
                }
            }
        }
        private bool GetTexCoord(MouseEventArgs e, out int x, out int y) {
            CreateTextureIfNeeded();
            RectangleF drawRect = GetArrayLayoutRect();
            x = (int)((e.X - drawRect.X) * w / drawRect.Width);
            y = (int)((e.Y - drawRect.Y) * h / drawRect.Height);
            return !(x < 0 || x >= w || y < 0 || y >= h) ;
        }
        private void timer_Tick(object sender, EventArgs e) {
            if (AnimatedSelection) {
                frame++;
                Refresh();
            }
        }
    }
}
