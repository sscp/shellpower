using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public class StringSimulator {
        /// <summary>
        /// Digraph of bypass diodes for calculation.
        /// </summary>
        /*private class BypassNode {
            public int minCell, maxCell;
            public List<BypassNode> childs = new List<BypassNode>();
        }
        private static BypassNode GetRoot(ArraySpec.CellString cellStr) {
            BypassNode root = new BypassNode();
            root.minCell = 0;
            root.maxCell = cellStr.Cells.Count;
            foreach (ArraySpec.BypassDiode diode in cellStr.BypassDiodes) {
                BypassNode node = root;
                while(true){
                    bool found = false;
                    foreach (BypassNode child in node.childs) {
                        if (diode.CellIxs.First >= node.minCell &&
                            diode.CellIxs.Second <= node.maxCell) {
                            node = child;
                            found = true;
                            break;
                        }
                    }
                    if (!found) break;
                }

                BypassNode newNode = new BypassNode();
                newNode.minCell = diode.CellIxs.First;
                newNode.maxCell = diode.CellIxs.Second;
                node.childs.Add(newNode);
            }
            return root;
        }*/
        public static IVTrace CalcStringIV(ArraySpec.CellString cellStr, IVTrace[] cellTraces, DiodeSpec bypassSpec) {
            Debug.Assert(cellTraces.Length != 0);
            Debug.Assert(cellTraces.Length == cellStr.Cells.Count);
            int ncells = cellTraces.Length;
            double strIsc = cellTraces[0].Isc;
            for (int i = 0; i < cellTraces.Length; i++) {
                strIsc = Math.Max(strIsc, cellTraces[i].Isc);
            }

            // which nodes connect to which others via bypass diode?
            // there's one node between each cell (subtotal ncells-1), 
            // plus one at each end of the string, total ncells+1
            List<int>[] links = new List<int>[ncells+1];
            for(int i = 0; i <= ncells; i++){
                links[i] = new List<int>();
            }
            foreach (ArraySpec.BypassDiode diode in cellStr.BypassDiodes) {
                links[diode.CellIxs.Second + 1].Add(diode.CellIxs.First);
            }

            // sweep current this time, compute voltage
            int nsamples = 200;
            int ngoodsamples = nsamples;
            double[] veci = new double[nsamples];
            double[] vecv = new double[nsamples];
            for (int i = 0; i < nsamples; i++) {
                double current = i * strIsc / (nsamples-1);

                // what cells are in bypass?
                double[] nodevs = new double[ncells+1];
                nodevs[0] = 0; // string starts at ground, zero volts
                for (int j = 1; j <= ncells; j++) {
                    // first, voltage assuming no bypass
                    double nodev = nodevs[j - 1];
                    if (current < cellTraces[j - 1].Isc) {
                        nodev += cellTraces[j - 1].InterpV(current);
                    } else {
                        nodev = Double.NegativeInfinity;
                    }
                    // then, can we do better with bypass?
                    foreach (int linkIx in links[j]) {
                        nodev = Math.Max(nodev, nodevs[linkIx] - bypassSpec.VoltageDrop);
                    }
                    nodevs[j] = nodev;
                }
                veci[i] = current;
                vecv[i] = Math.Max(nodevs[ncells], 0);

                // cut off the part of the trace that's invalid (unachievable current)
                if (nodevs[ncells] >= 0) {
                    Debug.Assert(ngoodsamples == nsamples); // should not "bounce"
                } else if(ngoodsamples == nsamples){
                    ngoodsamples = i;
                }
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
            ngoodsamples = Math.Min(ngoodsamples + 1, nsamples);
            Debug.Assert(ngoodsamples > 0);
            trace.I = new double[ngoodsamples];
            trace.V = new double[ngoodsamples];
            Array.Copy(veci, trace.I, ngoodsamples);
            Array.Copy(vecv, trace.V, ngoodsamples);
            trace.Isc = veci[ngoodsamples - 1];
            trace.Voc = vecv[0];
            trace.Imp = imp;
            trace.Vmp = vmp;
            return trace;
        }
    }
}
