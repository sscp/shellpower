using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using System.IO;

namespace SSCP.ShellPower {
    class Program {
        static int Main(string[] args) {
            String meshFile, textureFile;
            Mesh mesh;
            Bitmap texture;
            DateTime startTime, endTime;
            if (args.Length == 4) {
                try {
                    meshFile = args[0];
                    mesh = MeshIO.LoadMesh(meshFile);
                } catch (Exception e) {
                    Console.Error.WriteLine("Invalid mesh file {0}: {1}", args[0], e.Message);
                    return 1;
                }
                try {
                    textureFile = args[1];
                    FileStream stream = new FileStream(textureFile, FileMode.Open);
                    texture = new Bitmap(stream);
                } catch (Exception e) {
                    Console.Error.WriteLine("Invalid texture file {0}: {1}", args[1], e.Message);
                    return 1;
                }
                try {
                    startTime = DateTime.Parse(args[2]).ToUniversalTime();
                    endTime = DateTime.Parse(args[3]).ToUniversalTime();
                } catch (Exception e) {
                    Console.Error.WriteLine("Invalid time range {0} {1}: {2}", args[2], args[3], e.Message);
                    return 1;
                }
            } else {
                Console.Error.WriteLine("Usage: ShellBatch.exe <mesh file> <texture file> <utc start time> <utc end time>\n"
                    + " ... example: ShellBatch.exe luminos.stl luminos-6-string.png 2013-10-08T22:30:00Z 2013-10-09T07:30:00Z\n"
                    + " ... see https://github.com/dcposch/shellpower for documentation");
                return 1;
            }

            // Load simulation inputs
            ArraySimulationStepInput input = ArraySimDefaults.CreateDefaultInput();
            input.Array.Mesh = mesh;
            input.Array.LayoutTexture = texture;

            // Log simulation inputs 
            var numTriangles = input.Array.Mesh.triangles.Length;
            var meshSize = input.Array.Mesh.BoundingBox.Max - input.Array.Mesh.BoundingBox.Min;
            var textureSize = input.Array.LayoutTexture.Size;
            var numStrings = input.Array.Strings.Count;
            var numCells = input.Array.Strings.Sum(str => str.Cells.Count);
            var textureLayout = input.Array.LayoutBoundsXZ;
            var cellSpec = input.Array.CellSpec;
            var bypassSpec = input.Array.BypassDiodeSpec;
            Console.Error.WriteLine("Simulation input\n" +
                " ... mesh {0} ({1:0.000}x{2:0.000}x{3:0.000}, {4} triangles)\n" +
                " ... texture {5} ({6}x{7}, {8} strings, {9} cells)\n" +
                " ... texture layout x {10:0.000} y {11:0.000} width {12:0.000} height {13:0.000}\n" +
                " ... location {14:0.000000} {15:0.000000}\n" +
                " ... time {16} to {17}\n" +
                " ... conditions {18:0.0} direct, {19:0.0} indirect, {20:0.0}% encap loss\n" +
                " ... cells {21} {22}/degC isc at stc, {23} {24}/degC voc at stc, {25} series r, {26} ideality\n" +
                " ... bypass diodes {27}V drop",
                meshFile, meshSize.X, meshSize.Y, meshSize.Z, numTriangles,
                textureFile, textureSize.Width, textureSize.Height, numStrings, numCells,
                textureLayout.X, textureLayout.Y, textureLayout.Width, textureLayout.Height,
                input.Longitude, input.Latitude,
                startTime.ToString("yyyy-MM-ddTHH:mm:ssZ"), endTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                input.Irradiance, input.IndirectIrradiance, input.EncapuslationLoss * 100,
                cellSpec.IscStc, cellSpec.DIscDT, cellSpec.VocStc, cellSpec.DVocDT, cellSpec.SeriesR, cellSpec.NIdeal,
                bypassSpec.VoltageDrop);


            // Simulate
            new GameWindow(); // force OpenTK to initialize
            var simulator = new ArraySimulator();
            Console.WriteLine("time_utc,lit_area_m2,insolation_w,output_perfect_mppt_w,output_w");
            for (DateTime time = startTime; time <= endTime; time = time.AddMinutes(10)) {
                input.Utc = time;
                var output = simulator.Simulate(input);
                Console.WriteLine("{0},{1},{2},{3},{4},{5}",
                    time.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    output.ArrayArea,
                    output.ArrayLitArea,
                    output.WattsInsolation,
                    output.WattsOutputByCell,
                    output.WattsOutput);
            }

            return 0;
        }
    }
}