using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SSCP.ShellPower {
    /// <summary>
    /// Defines the JSON parameter file format.
    /// 
    /// Do NOT rename or remove any of these fields: you'll break existing config files.
    /// You can add new fields, but must handle the case where they're null.
    /// </summary>
    public class JsonSpec {
        public ArrayJsonSpec Array { get; set; }
        public EnvironmentJsonSpec Environment { get; set; }
    }
    public class ArrayJsonSpec {
        public String MeshFilename { get; set; }
        public String LayoutFilename { get; set; }
        public BoundsJsonSpec LayoutBounds { get; set; }
        public CellJsonSpec Cell { get; set; }
        public DiodeJsonSpec BypassDiode { get; set; }
        public double EncapuslationLoss { get; set; }
    }
    public class BoundsJsonSpec {
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinZ { get; set; }
        public double MaxZ { get; set; }
    }
    public class CellJsonSpec {
        public double VocStc { get; set; }
        public double IscStc { get; set; }
        public double DVocDT { get; set; }
        public double DIscDT { get; set; }
        public double AreaM2 { get; set; }
        public double NIdeal { get; set; }
        public double SeriesR { get; set; }
    }
    public class DiodeJsonSpec {
        public double VoltageDrop { get; set; }
    }
    public class EnvironmentJsonSpec {
        public DateTime Utc { get; set; }
        public double TimezoneOffsetHours { get; set; }
        public double LatitudeDeg { get; set; }
        public double LongitudeDeg { get; set; }
        public double HeadingDeg { get; set; }
        public double TiltDeg { get; set; }
        public double TemperatureC { get; set; }
        public double IrradianceWM2 { get; set; }
        public double IndirectIrradianceWM2 { get; set; }
    }
}
