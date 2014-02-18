using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public class CellSimulator {
        /// <summary>
        /// Calculates current flow for a given voltage.
        /// </summary>
        public static double CalcI(CellSpec cell, double v, double insolationW, double tempC) {
            double[] veci = CalcIV(cell, new double[] { v }, insolationW, tempC);
            return veci[0];
        }
        /// <summary>
        /// Computes an IV curve.
        /// </summary>
        public static double[] CalcIV(CellSpec cell, double[] vecv, double insolationW, double tempC) {
            double[] veci = new double[vecv.Length];
            double i0 = cell.CalcI0(insolationW, tempC);
            double isc = cell.CalcIsc(insolationW, tempC);
            double t = tempC + Constants.C_IN_KELVIN;
            double k = Constants.BOLTZMANN_K;
            double q = Constants.ELECTRON_CHARGE_Q;
            double ni = cell.NIdeal;
            double rs = cell.SeriesR;
            bool invalid = false;
            for (int i = 0; i < vecv.Length; i++) {
                double v = vecv[i];
                // iterate to convergence
                double iprev = 0.0, icurr = 0.0;
                for (int j = 0; j < 2000; j++) {
                    double vdrop = iprev * rs;
                    double idark = i0 * (Math.Exp((q * (v + vdrop)) / (ni * k * t)) - 1.0);
                    icurr = isc - idark;
                    if (Math.Abs(icurr - iprev) < 1e-6) {
                        //Debug.WriteLine("converged in " + j);
                        break;
                    }
                    iprev = 0.95 * iprev + 0.05 * icurr;
                }
                // check for invalid results (the simulation didn't converge)
                if (icurr < 0 || (i > 0 && icurr > veci[i - 1])){
                    invalid = true;
                }
                veci[i] = icurr;
            }
            if (invalid)
            {
                Debug.WriteLine("Warning: the IV trace didn't converge, returning zero currents");
                return new double[vecv.Length];
            }
            return veci;
        }
        public static IVTrace CalcSweep(CellSpec cell, double insolationW, double tempC) {
            double voc = cell.CalcVoc(insolationW, tempC);
            double isc = cell.CalcIsc(insolationW, tempC);
            int n = 100;
            double[] vecv = new double[n + 1];
            for (int i = 0; i <= n; i++) {
                vecv[i] = voc * (double)i / n;
            }
            double[] veci = CalcIV(cell, vecv, insolationW, tempC);
            //Debug.Assert(Math.Abs(veci[0] - isc) < 0.001);
            //Debug.Assert(Math.Abs(veci[n]) < 0.001);
            double pmp = 0.0;
            double imp = 0.0, vmp = 0.0;
            for (int i = 0; i < n; i++) {
                double p = veci[i] * vecv[i];
                if (p > pmp) {
                    pmp = p;
                    vmp = vecv[i];
                    imp = veci[i];
                }
            }

            IVTrace trace = new IVTrace();
            trace.I = veci;
            trace.V = vecv;
            trace.Isc = isc;
            trace.Voc = voc;
            trace.Imp = imp;
            trace.Vmp = vmp;
            return trace;
        }
    }
}
