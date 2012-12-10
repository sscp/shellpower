using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    public partial class IVTraceForm : Form {
        private IVTrace _trace;

        public string Label {
            get { return labelName.Text; }
            set { labelName.Text = value; }
        }
        public IVTrace IVTrace {
            get { return _trace; }
            set { _trace = value; UpdateView(); }
        }

        public IVTraceForm() {
            InitializeComponent();
        }
        private void UpdateView() {
            if (IVTrace == null) {
                labelMaxPower.Text = "";
            } else {
                // update text
                labelMaxPower.Text = string.Format("Maximum power: "+
                    "{0:0.000}A * {1:0.000}V = {2:0.000}W",
                    IVTrace.Imp, IVTrace.Vmp, IVTrace.Pmp);
                labelFillFactor.Text = string.Format(
                   "Isc={0:0.000}A, Voc={1:0.000}V, Fill factor={2:0.0}%",
                   IVTrace.Isc, IVTrace.Voc, IVTrace.FillFactor * 100);
                // update graph
                simpleGraphIV.X = IVTrace.V;
                simpleGraphIV.Y = IVTrace.I;
            }
        }
    }
}
