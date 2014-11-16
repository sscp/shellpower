using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SSCP.ShellPower {
    public static class JsonSpecConverter {
        public static JsonSpec Read(string filename) {
            String json = File.ReadAllText(filename, Encoding.UTF8);
            return (JsonSpec)JsonConvert.DeserializeObject(json, typeof(JsonSpec));
        }

        public static void Write(JsonSpec spec, string filename) {
            String json = JsonConvert.SerializeObject(spec, Formatting.Indented);
            File.WriteAllText(filename, json, Encoding.UTF8);
        }

        public static JsonSpec ToJson(ArraySimulationStepInput input, string relativeDir) {
            return new JsonSpec() {
                Array = ToJson(input.Array, relativeDir),
                Environment = new EnvironmentJsonSpec() {
                    HeadingDeg = RadToDeg(input.Heading),
                    IndirectIrradianceWM2 = input.IndirectIrradiance,
                    IrradianceWM2 = input.Irradiance,
                    LatitudeDeg = input.Latitude,
                    LongitudeDeg = input.Longitude,
                    TemperatureC = input.Temperature,
                    TiltDeg = RadToDeg(input.Tilt),
                    TimezoneOffsetHours = input.TimezoneOffsetHours,
                    Utc = input.Utc
                }
            };
        }

        private static double RadToDeg(double r) {
            return r * 180.0 / Math.PI;
        }

        private static double DegToRad(double d) {
            return d * Math.PI / 180.0;
        }

        private static ArrayJsonSpec ToJson(ArraySpec arraySpec, string relativeDir) {
            return new ArrayJsonSpec() {
                BypassDiode = ToJson(arraySpec.BypassDiodeSpec),
                Cell = ToJson(arraySpec.CellSpec),
                EncapuslationLoss = arraySpec.EncapuslationLoss,
                LayoutBounds = ToJson(arraySpec.LayoutBounds),
                LayoutFilename = GetRelative(arraySpec.LayoutFilename, relativeDir),
                MeshFilename = GetRelative(arraySpec.MeshFilename, relativeDir)
            };
        }

        private static string GetRelative(string p, string relativeDir) {
            throw new NotImplementedException();
        }

        private static BoundsJsonSpec ToJson(BoundsSpec rect) {
            return new BoundsJsonSpec() {
                MinX = rect.MinX,
                MinZ = rect.MinZ,
                MaxX = rect.MaxX,
                MaxZ = rect.MaxZ
            };
        }

        private static CellJsonSpec ToJson(CellSpec cellSpec) {
            return new CellJsonSpec() {
                AreaM2 = cellSpec.Area,
                DIscDT = cellSpec.DIscDT,
                DVocDT = cellSpec.DVocDT,
                IscStc = cellSpec.IscStc,
                NIdeal = cellSpec.NIdeal,
                SeriesR = cellSpec.SeriesR,
                VocStc = cellSpec.VocStc
            };
        }

        private static DiodeJsonSpec ToJson(DiodeSpec diodeSpec) {
            return new DiodeJsonSpec() {
                VoltageDrop = diodeSpec.VoltageDrop
            };
        }
    }
}
