using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

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

        public static ArraySimulationStepInput FromJson(JsonSpec spec, Mesh mesh, Bitmap texture) {
            ArraySimulationStepInput input = new ArraySimulationStepInput();
            input.Heading = DegToRad(spec.Environment.HeadingDeg);
            input.IndirectIrradiance = spec.Environment.IndirectIrradianceWM2;
            input.Irradiance = spec.Environment.IrradianceWM2;
            input.Latitude = spec.Environment.LatitudeDeg;
            input.Longitude = spec.Environment.LongitudeDeg;
            input.Temperature = spec.Environment.TemperatureC;
            input.Tilt = DegToRad(spec.Environment.TiltDeg);
            input.TimezoneOffsetHours = spec.Environment.TimezoneOffsetHours;
            input.Utc = spec.Environment.Utc;
            input.Array = new ArraySpec();
            input.Array.BypassDiodeSpec.VoltageDrop = spec.Array.BypassDiode.VoltageDrop;
            input.Array.CellSpec.Area = spec.Array.Cell.AreaM2;
            input.Array.CellSpec.DIscDT = spec.Array.Cell.DIscDT;
            input.Array.CellSpec.DVocDT = spec.Array.Cell.DVocDT;
            input.Array.CellSpec.IscStc = spec.Array.Cell.IscStc;
            input.Array.CellSpec.NIdeal = spec.Array.Cell.NIdeal;
            input.Array.CellSpec.SeriesR = spec.Array.Cell.SeriesR;
            input.Array.CellSpec.VocStc = spec.Array.Cell.VocStc;
            input.Array.EncapsulationLoss = spec.Array.EncapsulationLoss;
            input.Array.LayoutBounds = FromJson(spec.Array.LayoutBounds);
            input.Array.Mesh = mesh;
            input.Array.LayoutTexture = texture;
            input.Array.ReadStringsFromColors();
            return input;
        }

        private static BoundsSpec FromJson(BoundsJsonSpec boundsJsonSpec) {
            BoundsSpec spec = new BoundsSpec();
            spec.MaxX = boundsJsonSpec.MaxX;
            spec.MinX = boundsJsonSpec.MinX;
            spec.MaxZ = boundsJsonSpec.MaxZ;
            spec.MinZ = boundsJsonSpec.MinZ;
            return spec;
        }

        public static JsonSpec ToJson(ArraySimulationStepInput input, string layoutFile, string meshFile, string relativeDir) {
            return new JsonSpec() {
                Array = ToJson(input.Array, layoutFile, meshFile, relativeDir),
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

        private static ArrayJsonSpec ToJson(ArraySpec arraySpec, string layoutFile, string meshFile, string relativeDir) {
            return new ArrayJsonSpec() {
                BypassDiode = ToJson(arraySpec.BypassDiodeSpec),
                Cell = ToJson(arraySpec.CellSpec),
                EncapsulationLoss = arraySpec.EncapsulationLoss,
                LayoutBounds = ToJson(arraySpec.LayoutBounds),
                LayoutFilename = GetRelative(relativeDir, layoutFile),
                MeshFilename = GetRelative(relativeDir, meshFile)
            };
        }

        private static string GetRelative(string fromPath, string toPath) {
            Uri fromUri = new Uri(fromPath + "/");
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) {
                throw new ArgumentException();
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath;
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
