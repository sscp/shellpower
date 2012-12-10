using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public class DiodeSpec {
        private double _vdrop;
        public double VoltageDrop {
            get { return _vdrop; }
            set { Debug.Assert(value >= 0); _vdrop = value; }
        }
    }
}
