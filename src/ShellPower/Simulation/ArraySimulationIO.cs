using System;

namespace SSCP.ShellPower {
    public class ArraySimulationStepInput {
        // when and where
        public DateTime Utc { get; set; }
        /// <summary>
        /// Time zone offset from UTC. This has no effect on the simulation,
        /// but is useful for working with local time.
        /// </summary>
        public double TimezoneOffsetHours { get; set; }
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
        /// Tilt, in radians, + = tilt right, - = tilt left.
        /// </summary>
        public double Tilt { get; set; }

        /// <summary>
        /// Temperature in deg C.
        /// </summary>
        public double Temperature { get; set; }
        /// <summary>
        /// Flat-plate (facing the sun) direct irradiance in watts per square meter. 
        /// Does NOT include indirect irradiance from the rest of the sky, only from the sun.
        /// </summary>
        public double Irradiance { get; set; }
        /// <summary>
        /// Flat-plate (facing straight up) indirect irradiance in watts per square meter. 
        /// Does NOT include irradiance directly from the sun, but only from the rest of the sky.
        /// </summary>
        public double IndirectIrradiance { get; set; }

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
