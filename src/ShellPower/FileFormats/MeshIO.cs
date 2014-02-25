using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    public class MeshIO {
        public static Mesh LoadMesh(String filename) {
            String extension = filename.Split('.').Last().ToLower();
            IMeshParser parser;
            if (extension.Equals("3dxml")) {
                parser = new MeshParser3DXml();
            } else if (extension.Equals("stl")) {
                parser = new MeshParserStl();
            } else {
                throw new ArgumentException("Unsupported file type: " + extension);
            }
            parser.Parse(filename);
            Mesh mesh = parser.GetMesh();

            // convert mm to m if necessary
            Vector3 size = mesh.BoundingBox.Max - mesh.BoundingBox.Min;
            if (size.Length > 1000) {
                mesh = MeshUtils.Scale(mesh, 0.001f);
                size *= 0.001f;
            }
            return mesh;
        }
    }
}
