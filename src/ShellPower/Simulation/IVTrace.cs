using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public class IVTrace {
        public double[] I { get; set; }
        public double[] V { get; set; }
        public double Voc { get; set; }
        public double Isc { get; set; }
        public double Vmp { get; set; }
        public double Imp { get; set; }
        public double Pmp { get { return Vmp * Imp; } }
        public double FillFactor { get { return Pmp / (Isc * Voc); } }

        public double InterpV(double i) {  
            Debug.Assert(I.Length == V.Length && I.Length >= 2);
            return LinInterp(I, V, i, I[0]<I[I.Length-1]);
        }
        public double InterpI(double v) {
            Debug.Assert(I.Length == V.Length && I.Length >= 2);
            return LinInterp(V, I, v, V[0]<V[V.Length-1]);
        }
        private double LinInterp(double[] xs, double[] ys, double x0, bool asc) {
            // check preconditions
            Debug.Assert(xs.Length == ys.Length);
            int n = xs.Length;
            for(int i = 0; i < n-1; i++){
                if (asc) {
                    Debug.Assert(xs[i + 1] >= xs[i]);
                } else {
                    Debug.Assert(xs[i + 1] <= xs[i]);
                }
            }
            //Debug.Assert(asc ? x0 >= xs[0] : x0 <= xs[0]);
            //Debug.Assert(asc ? x0 <= xs[n - 1] : x0 >= xs[n - 1]);

            // binary search
            int ix0 = 0, ix1 = n - 1;
            while (ix0 < ix1-1) {
                int ixmid = (ix1 + ix0) / 2;
                if ((x0 > xs[ixmid]) == asc) {
                    ix0 = ixmid;
                } else {
                    ix1 = ixmid;
                }
            }
            Debug.Assert(ix0 == ix1 - 1);
            double t = (x0 - xs[ix0]) / (xs[ix1] - xs[ix0]);
            return t * ys[ix1] + (1 - t) * ys[ix0];
        }
    }
}
