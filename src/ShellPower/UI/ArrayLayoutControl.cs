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

        private const int JUNCTION_RADIUS = 4; //px
        private const int JUNCTION_RADIUS_CLICK = 15; //px

        // array model, currently selected string
        private ArraySpec _array;
        private ArraySpec.CellString _cellStr;

        // array layout and cached properties computed from the layout
        private Bitmap tex, texSmall; // layout and downsampled version
        private Color[,] pixels;
        private PointF[] cellPoints, junctionPoints;

        // selection rendering
        private Bitmap texSelected; // overlay
        private List<int> bypassJunctions = new List<int>();

        // mouseover hints
        private int mouseoverJunction;
        private ArraySpec.Cell mouseoverCell;

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
            timer.Tick += new EventHandler(OnAnimationTick);
            timer.Enabled = true;
        }

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

        protected override void OnPaint(PaintEventArgs e) {
            Graphics g = e.Graphics;
            DateTime start = DateTime.Now;

            RecomputeArrayViewModel(); // compute junction points, downsampled array image, etc
            //Debug.WriteLine("compute: " + (DateTime.Now - start).TotalMilliseconds);

            DrawBackground(g);
            //Debug.WriteLine("clear: " + (DateTime.Now - start).TotalMilliseconds);

            if (texSmall != null) {
                g.DrawImage(texSmall, new Point(0, 0));
                //Debug.WriteLine("layout: " + (DateTime.Now - start).TotalMilliseconds);
            }

            if (CellString != null) {
                RecomputeTexSelected();
                DrawSelectedString(g);
                //Debug.WriteLine("highlight: " + (DateTime.Now - start).TotalMilliseconds);
            }

            if (mouseoverJunction >= 0 && mouseoverJunction < junctionPoints.Length) {
                DrawJunction(g, junctionPoints[mouseoverJunction], Brushes.White);
                //Debug.WriteLine("mouse: " + (DateTime.Now - start).TotalMilliseconds);
            }
            //Debug.WriteLine("done: " + (DateTime.Now - start).TotalMilliseconds);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            // are we even listening for clicks?
            if (!Editable || CellString == null) {
                return;
            }

            // find out which cell or cell-cell junction was clicked on
            RecomputeArrayViewModel();
            if (EditBypassDiodes) {
                int junction = GetJunctionIxAtPixel(e.X, e.Y);
                if (junction < 0) {
                    return;
                }
                ClickBypassJunction(junction);
            } else {
                ArraySpec.Cell newCell = GetCellAtPixel(e.X, e.Y);
                if (newCell == null) {
                    return;
                }
                ClickCell(newCell);
            }

            // the string has changed. tell our listeners and redraw the UI
            if (CellStringChanged != null) {
                CellStringChanged(this, null);
            }
            Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e) {
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            RecomputeArrayViewModel();

            if (EditBypassDiodes) {
                mouseoverJunction = GetJunctionIxAtPixel(e.X, e.Y);
                mouseoverCell = null;
            } else {
                mouseoverCell = GetCellAtPixel(e.X, e.Y);
                mouseoverJunction = -1;
            }
            if (mouseoverJunction == -1 && mouseoverCell == null) {
                this.Cursor = Cursors.Arrow; // not clickable
            } else {
                this.Cursor = Cursors.Hand;
            }
        }

        private void OnAnimationTick(object sender, EventArgs e) {
            if (AnimatedSelection) {
                Refresh();
            }
        }


        private void RecomputeArrayViewModel() {
            if (Array == null || Array.LayoutTexture == null) return;
            
            // read the array texture
            DateTime start = DateTime.Now;
            if (tex != Array.LayoutTexture){
                tex = Array.LayoutTexture;
                pixels = GetPixels(tex);
                //Debug.WriteLine("... read pix " + (DateTime.Now - start).TotalMilliseconds);
            }

            // scale the array texture to the current viewport size
            SizeF size = GetScaledArraySize();
            if (texSmall == null || texSmall.Size != size.ToSize()) {
                texSmall = new Bitmap((int)size.Width, (int)size.Height);
                Graphics g = Graphics.FromImage(texSmall);
                g.DrawImage(Array.LayoutTexture, new Rectangle(new Point(0, 0), texSmall.Size));
                //Debug.WriteLine("... read pix " + (DateTime.Now - start).TotalMilliseconds);
            }

            // compute properties of the individual cells
            cellPoints = ComputeCellCenterpoints(CellString);
            junctionPoints = ComputeJunctions(cellPoints);
            //Debug.WriteLine("... c+j " + (DateTime.Now - start).TotalMilliseconds);
        }

        private void RecomputeTexSelected() {
            SizeF size = GetScaledArraySize();
            int w = Array.LayoutTexture.Width, h = Array.LayoutTexture.Height;
            float scale = size.Width / w;
            int selW = (int)size.Width, selH = (int)size.Height;
            if (texSelected == null || texSelected.Width != selW || texSelected.Height != selH) {
                texSelected = new Bitmap(selW, selH);
            }
            BitmapData texSelData = texSelected.LockBits(
                new Rectangle(0, 0, selW, selH),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppArgb);
            int animation = (int)(DateTime.Now.Ticks / 1000000 % 16);
            unsafe {
                uint* ptr = (uint*)texSelData.Scan0.ToPointer();
                for (int i = 0; i < selW * selH; i++) {
                    ptr[i] = 0;
                }
                foreach (ArraySpec.Cell cell in CellString.Cells) {
                    foreach (Pair<int> pixel in cell.Pixels) {
                        int i = (int)(pixel.First * scale);
                        int j = (int)(pixel.Second * scale);
                        if (i < 0 || i >= selW || j < 0 || j >= selH)
                        {
                            continue;
                        }
                        bool mask = (j + i + animation) % 16 < 8;
                        uint alpha = (uint)(mask ? 0x80 : 0x00);
                        uint color = 0xffffff | (alpha << 24); // white highlight
                        ptr[j * selW + i] = color;
                    }
                }
            }
            texSelected.UnlockBits(texSelData);
        }

        private void DrawBackground(Graphics g) {
            g.Clear(Color.Black);
            if (Array == null || Array.LayoutTexture == null) {
                return;
            }
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void DrawSelectedString(Graphics g) {
            // highlight the selected cells
            g.DrawImage(texSelected, new Point(0,0));

            // draw the wiring
            if (cellPoints.Length > 1) {
                g.DrawLines(new Pen(Color.FromArgb(80, Color.Black), 3.0f), cellPoints);
                g.DrawLines(new Pen(Color.LightYellow, 1.0f), cellPoints);
            }

            // draw the bypass diode arcs
            int ndiodes = CellString.BypassDiodes.Count;
            for (int i = 0; i < ndiodes; i++) {
                ArraySpec.BypassDiode diode = CellString.BypassDiodes[i];
                PointF pA = junctionPoints[diode.CellIxs.First];
                PointF pB = junctionPoints[diode.CellIxs.Second + 1];
                float perpX = (pB.Y - pA.Y) * 0.2f;
                float perpY = (pA.X - pB.X) * 0.2f;
                PointF pMidA = new PointF(
                    pA.X * 0.7f + pB.X * 0.3f + perpX,
                    pA.Y * 0.7f + pB.Y * 0.3f + perpY);
                PointF pMidB = new PointF(
                    pA.X * 0.3f + pB.X * 0.7f + perpX,
                    pA.Y * 0.3f + pB.Y * 0.7f + perpY);
                g.DrawBezier(new Pen(Color.FromArgb(200, Color.Black), 5f), pA, pMidA, pMidB, pB);
                g.DrawBezier(new Pen(Color.Red, 3f), pA, pMidA, pMidB, pB);
            }

            // draw the bypass diode endpoints
            foreach(int i in bypassJunctions){
                DrawJunction(g, junctionPoints[i], Brushes.Red);
            }
        }

        private void DrawJunction(Graphics g, PointF point, Brush brush) {
            g.FillEllipse(brush,
                point.X - JUNCTION_RADIUS, point.Y - JUNCTION_RADIUS,
                JUNCTION_RADIUS * 2, JUNCTION_RADIUS * 2);
        }

        private void ClickBypassJunction(int junction) {
            if (!bypassJunctions.Remove(junction)) {
                bypassJunctions.Add(junction);
            }
            if (bypassJunctions.Count == 2) {
                // ix0 and ix1 can be the same, for single-cell bypass diodes
                int ix0 = Math.Min(bypassJunctions[0], bypassJunctions[1]);
                int ix1 = Math.Max(bypassJunctions[0], bypassJunctions[1]) - 1;
                ArraySpec.BypassDiode newDiode = new ArraySpec.BypassDiode();
                newDiode.CellIxs = new Pair<int>(ix0, ix1);
                if (!CellString.BypassDiodes.Remove(newDiode)) {
                    CellString.BypassDiodes.Add(newDiode);
                }
                bypassJunctions.Clear();
            }
        }

        private void ClickCell(ArraySpec.Cell cell) {
            // either add it to the current string, or remove it if it's already there
            if (!CellString.Cells.Remove(cell)) {
                CellString.Cells.Add(cell);
            } else {
                // prune bypass diodes
                CellString.BypassDiodes.RemoveAll((diode) => {
                    return diode.CellIxs.First >= CellString.Cells.Count ||
                        diode.CellIxs.Second >= CellString.Cells.Count;
                });
            }
        }

        /// <summary>
        /// Gets the centerpoint of each cell in the texture 
        /// that has been wired in the current string, in order, 
        /// in screen (layout control pixel, not texel) coordinates.
        /// </summary>
        private PointF[] ComputeCellCenterpoints(ArraySpec.CellString cellString) {
            if (cellString == null) {
                return new PointF[0];
            }

            SizeF arraySize = GetScaledArraySize();
            float scale = arraySize.Width / Array.LayoutTexture.Width;

            int n = cellString.Cells.Count;
            PointF[] points = new PointF[n];
            for (int i = 0; i < n; i++) {
                ArraySpec.Cell cell = cellString.Cells[i];
                int sx = 0, sy = 0;
                foreach (Pair<int> xy in cell.Pixels) {
                    sx += xy.First;
                    sy += xy.Second;
                }
                int m = cell.Pixels.Count;
                points[i] = new PointF(
                    (float)sx / m * scale,
                    (float)sy / m * scale);
            }
            return points;
        }

        private PointF[] ComputeJunctions(PointF[] cells) {
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

        private SizeF GetScaledArraySize() {
            double texW = Array.LayoutTexture.Width;
            double texH = Array.LayoutTexture.Height;
            double scale = Math.Min(Width / texW, Height / texH);
            return new SizeF((float)(scale * texW), (float)(scale * texH));
        }

        private Color[,] GetPixels(Bitmap bmp) {
            int w = bmp.Width, h = bmp.Height;
            Color[,] pixels = new Color[w, h];

            // jump thru some hoops to read Bitmap data efficiently
            BitmapData texData = bmp.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            unsafe {
                int* ptr = (int*)texData.Scan0.ToPointer();
                for (int i = 0; i < w; i++) {
                    for (int j = 0; j < h; j++) {
                        Color pixelColor = Color.FromArgb(ptr[j * w + i] | unchecked((int)0xff000000));
                        Debug.Assert(pixelColor.A == 255);
                        pixels[i, j] = pixelColor;
                    }
                }
            }
            bmp.UnlockBits(texData);

            return pixels;
        }

        private ArraySpec.Cell GetCellAtPixel(int pixelX, int pixelY) {
            // find click texture coords
            int x, y;
            if (!GetTexCoord(pixelX, pixelY, out x, out y)) return null;
            Color color = pixels[x, y];
            if (ColorUtils.IsGrayscale(color)) return null;

            // flood fill
            int w = Array.LayoutTexture.Width, h = Array.LayoutTexture.Height;
            HashSet<Pair<int>> ffS = new HashSet<Pair<int>>();
            Queue<Pair<int>> ffQ = new Queue<Pair<int>>();
            ffQ.Enqueue(new Pair<int>(x, y));
            while (ffQ.Count > 0) {
                Pair<int> xy = ffQ.Dequeue();

                // skip if redundant
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
            int minIx = -1, minDD = JUNCTION_RADIUS_CLICK * JUNCTION_RADIUS_CLICK;
            for (var i = 0; i < junctionPoints.Length; i++) {
                int dx = pixelX - (int)junctionPoints[i].X;
                int dy = pixelY - (int)junctionPoints[i].Y;
                int dd = dx * dx + dy * dy;
                if (dd < minDD) {
                    minDD = dd;
                    minIx = i;
                }
            }
            return minIx;
        }

        private bool GetTexCoord(int pixX, int pixY, out int x, out int y) {
            int w = Array.LayoutTexture.Width, h = Array.LayoutTexture.Height;
            SizeF size = GetScaledArraySize();
            x = (int)(pixX * w / size.Width);
            y = (int)(pixY * h / size.Height);
            return !(x < 0 || x >= w || y < 0 || y >= h);
        }
    }
}
