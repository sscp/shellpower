using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public class StringSimulator {
        public static IVTrace CalcStringIV(ArraySpec.CellString cellStr, IVTrace[] cellTraces) {
            Debug.Assert(cellTraces.Length != 0);
            Debug.Assert(cellTraces.Length == cellStr.Cells.Count);
            int ncells = cellTraces.Length;
            double strIsc = cellTraces[0].Isc;
            for (int i = 0; i < cellTraces.Length; i++) {
                strIsc = Math.Min(strIsc, cellTraces[i].Isc);
            }

            // sweep current this time, compute voltage
            int nsamples = 200;
            double[] veci = new double[nsamples];
            double[] vecv = new double[nsamples];
            for (int i = 0; i < nsamples; i++) {
                veci[i] = i * strIsc / (nsamples-1);
                double sumv = 0.0;
                for (int j = 0; j < ncells; j++) {
                    sumv += cellTraces[j].InterpV(veci[i]);
                }
                vecv[i] = sumv;
            }

            // calculate summary info such as mppt power
            double vmp = 0, imp = 0;
            for (int i = 0; i < nsamples; i++) {
                if (veci[i] * vecv[i] > vmp * imp) {
                    vmp = vecv[i];
                    imp = veci[i];
                }
            }
            
            // return a trace. supports lin interp, etc
            IVTrace trace = new IVTrace();
            trace.I = veci;
            trace.V = vecv;
            trace.Isc = strIsc;
            trace.Voc = vecv[0];
            trace.Imp = imp;
            trace.Vmp = vmp;
            return trace;
        }
    }
}
