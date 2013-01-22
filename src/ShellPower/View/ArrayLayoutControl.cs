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
            set {
                _array = value;
                if (_array != null && !_array.Strings.Contains(_cellStr)) {
                    _cellStr = null;
                }
                Refresh(); 
            }
        }
        public ArraySpec.CellString CellString {
            get { return _cellStr; }
            set { _cellStr = value; Refresh(); }
        }
        public bool Editable { get; set; }
        public bool EditBypassDiodes { get; set; }
        public bool AnimatedSelection { get; set; }
        public event EventHandler CellStringChanged;

        private const int JUNCTION_RADIUS = 4; //px
        private const int JUNCTION_RADIUS_CLICK = 15; //px

        private int frame = 0;
        private int nextId = 1;
        private Bitmap tex; // array layout
        private Bitmap texSelected; // overlay
        private Color[,] pixels;
        private int w, h;

        private int mouseoverJunction;
        private ArraySpec.Cell mouseoverCell;
        // bypassCells count = [0,1,2] dep on ui state
        private List<int> bypassJunctions = new List<int>(); 

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
            //Debug.Assert(tex == null || tex == Array.LayoutTexture);
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
                }
            }
            texSelected.UnlockBits(texSelData);
        }
        private ArraySpec.Cell GetCellAtPixel(int pixelX, int pixelY) {
            // find click texture coords
            int x, y;
            if (!GetTexCoord(pixelX,pixelY, out x, out y)) return null;
            Color color = pixels[x, y];
            if (ColorUtils.IsGrayscale(color)) return null;

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
                for (int x2 = xy.First - 1; x2 <= xy.First + 1; x2++) {
                    for (int y2 = xy.Second - 1; y2 <= xy.Second + 1; y2++) {
                        if (x2 <= 0 || x2 >= w || y2 <= 0 || y2 >= h) continue;
                        if (pixels[x2, y2] != color) continue;
                        ffQ.Enqueue(new Pair<int>(x2, y2));
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
            return newCell;
        }
        private int GetJunctionIxAtPixel(int pixelX, int pixelY) {
            PointF[] junctions = GetJunctions(GetCellCenterpoints());
            int minIx = -1, minDD = JUNCTION_RADIUS_CLICK*JUNCTION_RADIUS_CLICK;
            for (var i = 0; i < junctions.Length; i++) {
                int dx = pixelX - (int)junctions[i].X;
                int dy = pixelY - (int)junctions[i].Y;
                int dd = dx * dx + dy * dy;
                if (dd < minDD) {
                    minDD = dd;
                    minIx = i;
                }
            }
            return minIx;
        }
        private PointF[] GetJunctions(PointF[] cells) {
            if (cells.Length == 0) {
                return new PointF[0];
            } else if (cells.Length == 1) {
                return new PointF[] {
                    new PointF(cells[0].X - 10, cells[0].Y),
                    new PointF(cells[0].X + 10, cells[0].Y)
                };
            } else {
                int n = cells.Length + 1;
                PointF[] junctions = new PointF[n];
                junctions[0] = new PointF(
                    cells[0].X * 1.5f - cells[1].X * 0.5f,
                    cells[0].Y * 1.5f - cells[1].Y * 0.5f);
                junctions[n - 1] = new PointF(
                    cells[n - 2].X * 1.5f - cells[n - 3].X * 0.5f,
                    cells[n - 2].Y * 1.5f - cells[n - 3].Y * 0.5f);
                for (int i = 1; i < n - 1; i++) {
                    junctions[i] = new PointF(
                        cells[i - 1].X * 0.5f + cells[i].X * 0.5f,
                        cells[i - 1].Y * 0.5f + cells[i].Y * 0.5f);
                }
                return junctions;
            }
        }
        /// <summary>
        /// Gets the centerpoint of each cell in the texture 
        /// that has been wired in the current string, in order, 
        /// in screen (layout control pixel, not texel) coordinates.
        /// </summary>
        private PointF[] GetCellCenterpoints() {
            RectangleF arrayLayoutRect = GetArrayLayoutRect();
            int n = CellString.Cells.Count;
            PointF[] points = new PointF[n];
            float scale = arrayLayoutRect.Width / Array.LayoutTexture.Width;
            for (int i = 0; i < n; i++) {
                ArraySpec.Cell cell = CellString.Cells[i];
                int sx = 0, sy = 0;
                foreach (Pair<int> xy in cell.Pixels) {
                    sx += xy.First;
                    sy += xy.Second;
                }
                int m = cell.Pixels.Count;
                points[i] = new PointF(
                    (float)sx / m * scale + arrayLayoutRect.X,
                    (float)sy / m * scale + arrayLayoutRect.Y);
            }
            return points;
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
            PointF[] cellPoints = GetCellCenterpoints();
            if (cellPoints.Length > 1) {
                g.DrawLines(new Pen(Color.FromArgb(80, Color.Black), 3.0f), cellPoints);
                g.DrawLines(new Pen(Color.LightYellow, 1.0f), cellPoints);
            }

            // draw the bypass diodes
            int ndiodes = CellString.BypassDiodes.Count;
            PointF[] junctionPoints = GetJunctions(cellPoints);
            for (int i = 0; i < ndiodes; i++) {
                ArraySpec.BypassDiode diode = CellString.BypassDiodes[i];
                PointF pA = junctionPoints[diode.CellIxs.First];
                PointF pB = junctionPoints[diode.CellIxs.Second+1];
                float perpX = (pB.Y - pA.Y)*0.2f;
                float perpY = (pA.X - pB.X)*0.2f;
                PointF pMidA = new PointF(
                    pA.X*0.7f+pB.X*0.3f+perpX,
                    pA.Y*0.7f+pB.Y*0.3f+perpY);
                PointF pMidB = new PointF(
                    pA.X*0.3f+pB.X*0.7f+perpX,
                    pA.Y*0.3f+pB.Y*0.7f+perpY);
                g.DrawBezier(new Pen(Color.FromArgb(200, Color.Black), 5f), pA, pMidA, pMidB, pB);
                g.DrawBezier(new Pen(Color.Red, 3f), pA, pMidA, pMidB, pB);
            }

            // draw junction selection
            int nj = bypassJunctions.Count + (mouseoverJunction < 0 ? 0 : 1);
            for(int i = 0; i < nj; i++){
                PointF pJ;
                Brush bJ;
                if (i < bypassJunctions.Count) {
                    pJ = junctionPoints[bypassJunctions[i]];
                    bJ = Brushes.Red;
                } else {
                    pJ = junctionPoints[mouseoverJunction];
                    bJ = Brushes.White;
                }
                g.FillEllipse(bJ,
                    pJ.X - JUNCTION_RADIUS, pJ.Y - JUNCTION_RADIUS,
                    JUNCTION_RADIUS * 2, JUNCTION_RADIUS * 2);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e) {
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            if (!Editable || CellString==null) return;

            if (EditBypassDiodes) {
                int junction = GetJunctionIxAtPixel(e.X, e.Y);
                if (junction < 0) return;
                if (!bypassJunctions.Remove(junction)) {
                    bypassJunctions.Add(junction);
                }
                if (bypassJunctions.Count == 2) {
                    // ix0 and ix1 can be the same, for single-cell bypass diodes
                    int ix0 = Math.Min(bypassJunctions[0], bypassJunctions[1]);
                    int ix1 = Math.Max(bypassJunctions[0], bypassJunctions[1]) - 1;
                    ArraySpec.BypassDiode newDiode = new ArraySpec.BypassDiode();
                    newDiode.CellIxs = new Pair<int>(ix0,ix1);
                    if (!CellString.BypassDiodes.Remove(newDiode)) {
                        CellString.BypassDiodes.Add(newDiode);
                    }
                    bypassJunctions.Clear();
                }
            } else {
                ArraySpec.Cell newCell = GetCellAtPixel(e.X, e.Y);
                if (newCell == null) return;

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
            if (EditBypassDiodes) {
                mouseoverJunction = GetJunctionIxAtPixel(e.X, e.Y);
                mouseoverCell = null;
            } else {
                mouseoverCell = GetCellAtPixel(e.X, e.Y);
                mouseoverJunction = -1;
            }
            if(mouseoverJunction==-1 && mouseoverCell==null) {
                this.Cursor = Cursors.Arrow; // not clickable
            } else {
                this.Cursor = Cursors.Hand;
            } 
        }
        private bool GetTexCoord(int pixX, int pixY, out int x, out int y) {
            CreateTextureIfNeeded();
            RectangleF drawRect = GetArrayLayoutRect();
            x = (int)((pixX - drawRect.X) * w / drawRect.Width);
            y = (int)((pixY - drawRect.Y) * h / drawRect.Height);
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
