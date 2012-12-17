using System;

namespace SSCP.ShellPower {
    public class ArraySimulationStepInput {
        // when and where
        public DateTime Utc { get; set; }
        public DateTime LocalTime { get; set; }
        public float Timezone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }

        // conditions
        public double Temperature { get; set; }
        public Double Insolation { get; set; }

        // shape and layout of the solar array
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
