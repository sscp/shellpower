using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSCP.ShellPower {
    public class IVTrace {
        public double[] I { get; set; }
        public double[] V { get; set; }
        public double I0 { get; set; }
        public double Voc { get; set; }
        public double Isc { get; set; }
        public double FillFactor { get; set; }
        public double Vmp { get; set; }
        public double Imp { get; set; }
        public double Pmp { get { return Vmp * Imp; } }
    }
}
