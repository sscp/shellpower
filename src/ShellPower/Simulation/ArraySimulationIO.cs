using System;

namespace SSCP.ShellPower {
    public class ArraySimulationStepInput {
        // when and where
        public DateTime Utc { get; set; }
        public DateTime LocalTime {
            get {
                return Utc.Add(Timezone.GetUtcOffset(Utc));
            }
        }
        /// <summary>
        /// Time zone. This has no effect on the simulation,
        /// but is useful for working with local time.
        /// </summary>
        public TimeZoneInfo Timezone { get; set; }
        /// <summary>
        /// Latitude, in degrees.
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Longitude, in degrees.
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Heading, in radians, 0 = due north.
        /// </summary>
        public double Heading { get; set; }

        /// <summary>
        /// Temperature in deg C.
        /// </summary>
        public double Temperature { get; set; }
        /// <summary>
        /// Flat-plate insolation in watts per square meter. 
        /// </summary>
        public Double Insolation { get; set; }

        /// <summary>
        /// Shape and layout of the solar array
        /// </summary>
        public ArraySpec Array { get; set; }
    }

    public class ArraySimulationStepOutput {
        public ArraySimulationStepOutput() {
            Strings = new ArraySimStringOutput[0];
        }
        public double ArrayArea { get; set; }
        public double ArrayLitArea { get; set; }
        public double WattsInsolation { get; set; }
        public double WattsOutputByCell { get; set; }
        public double WattsOutput { get; set; }

        public ArraySimStringOutput[] Strings { get; set; }
    }

    public class ArraySimStringOutput {
        public double WattsIn { get; set; }
        public double WattsOutput { get; set; }
        public double WattsOutputByCell { get; set; }
        public double WattsOutputIdeal { get; set; }
        public IVTrace IVTrace { get; set; }
        public double Area { get; set; }
        public double AreaShaded { get; set; }
        public ArraySpec.CellString String { get; set; }

        public override string ToString() {
            return string.Format("{0}: {1:0.0} W", String, WattsOutput);
        }
    }
}
