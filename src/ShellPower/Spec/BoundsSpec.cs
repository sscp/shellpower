using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSCP.ShellPower {
    /// <summary>
    /// Array layout texture bounds, in the X and Z dimension, in meters.
    /// (Note that the model coordinates are required to be mm or m.)
    /// </summary>
    public class BoundsSpec {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinZ { get; set; }
        public double MaxZ { get; set; }
    }
}
