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
        public double ArrayArea { get; set; }
        public double ArrayLitArea { get; set; }
        public double WattsInsolation { get; set; }
        public double WattsOutputByCell { get; set; }
        public double WattsOutput { get; set; }
        // TODO: WattsInsolationNoShadow, WattsOutputNoShadow
    }

}
