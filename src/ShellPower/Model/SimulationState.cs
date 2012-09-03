﻿using System;

namespace SSCP.ShellPower {
    public class ArraySimulationStepInput {
        public DateTime Utc { get; set; }
        public DateTime LocalTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public float Timezone { get; set; }
        public double Heading { get; set; }
    }

    public class ArraySimulationStepOutput {
        public double SunAzimuth { get; set; }
        public double SunElevation { get; set; }
    }
}
