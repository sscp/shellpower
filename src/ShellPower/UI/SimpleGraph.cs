using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    public partial class SimpleGraph : UserControl {
        public SimpleGraph() {
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

        private int[] margins = { 10, 10, 20, 20 };
        private double[] x, y;
        public double[] X {
            get {
                return x;
            }
            set {
                x = value;
                Refresh();
            }
        }
        public double[] Y {
            get {
                return y;
            }
            set {
                y = value;
                Refresh();
            }
        }
        public int[] Margins {
            get {
                return margins;
            }
            set {
                margins = value;
                Refresh();
            }
        }

        double[] CalcTicks(double min, double max) {
            Debug.Assert(max > min);
            double tick = Math.Pow(10, (int)Math.Log10(max - min) - 1);
            while ((max - min) / tick > 10) tick *= 10;
            if ((max - min) / tick < 2) tick /= 5;
            if ((max - min) / tick < 5) tick /= 2;
            long i1 = (int)(min / tick) + 1;
            long i2 = (int)(max / tick);
            double[] ticks = new double[(i2 - i1) + 1]; 
            for (long i = i1; i <= i2; i++) {
                ticks[i - i1] = i * tick;
            }
            return ticks;
        }

        protected override void OnPaint(PaintEventArgs e) {
            // style
            Color colorBg = Color.Black;
            Font fontLabels = new Font("Verdana", 10);
            Brush brushLabels = Brushes.LightGray;
            Pen penGrid = Pens.Gray;
            Pen penData = Pens.Yellow;
            Graphics g = e.Graphics;
            g.Clear(colorBg);

            // get the data
            if (x == null || y == null) return;
            Debug.Assert(x.Length == y.Length);
            Debug.Assert(x.Length > 0.0);
            int n = x.Length;
            double xmin = x[0], xmax = x[0];
            double ymin = y[0], ymax = y[0];
            for (int i = 1; i < n; i++) {
                xmin = Math.Min(x[i], xmin);
                xmax = Math.Max(x[i], xmax);
                ymin = Math.Min(y[i], ymin);
                ymax = Math.Max(y[i], ymax);
            }
            if (xmax <= xmin || ymax <= ymin) return;

            // plot the axes
            int[] margins = Margins;
            int w = Width - margins[1] - margins[3];
            int h = Height - margins[0] - margins[2];
            double[] xticks = CalcTicks(xmin, xmax);
            double[] yticks = CalcTicks(ymin, ymax);
            for (int i = 0; i < xticks.Length; i++) {
                int xt = (int)((xticks[i] - xmin) / (xmax - xmin) * w) + margins[3];
                g.DrawLine(penGrid, xt, margins[0], xt, margins[0] + h);
                g.DrawString(string.Format("{0:0.000}", xticks[i]), 
                    fontLabels, brushLabels, xt, margins[0] + h); 
            }
            for (int i = 0; i < yticks.Length; i++) {
                int yt = h-(int)((yticks[i] - ymin) / (ymax - ymin) * h) + margins[0];
                g.DrawLine(penGrid, margins[3], yt, margins[3] + w, yt);
                g.DrawString(string.Format("{0:0.000}", yticks[i]),
                    fontLabels, brushLabels, margins[3], yt); 
            }

            // plot the data
            Point[] points = new Point[n];
            for (int i = 0; i < n; i++) {
                points[i].X = (int)((x[i] - xmin) / (xmax - xmin) * w) + margins[3];
                points[i].Y = h - (int)((y[i] - ymin) / (ymax - ymin) * h) + margins[0];                
            }
            g.DrawLines(penData, points);
        }
    }
}
