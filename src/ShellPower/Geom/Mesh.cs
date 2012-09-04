using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    public class Mesh {
        public struct Triangle {
            public Triangle(int vA, int vB, int vC) {
                vertexA = vA;
                vertexB = vB;
                vertexC = vC;
            }
            public int vertexA, vertexB, vertexC;
        }
        public Vector3[] points;
        public Vector3[] normals;
        public Triangle[] triangles;

        public Quad3 BoundingBox {
            get {
                var box = new Quad3();
                box.Min = box.Max = points[0];
                foreach (var point in points) {
                    if (point.X < box.Min.X) box.Min.X = point.X;
                    if (point.Y < box.Min.Y) box.Min.Y = point.Y;
                    if (point.Z < box.Min.Z) box.Min.Z = point.Z;
                    if (point.X > box.Max.X) box.Max.X = point.X;
                    if (point.Y > box.Max.Y) box.Max.Y = point.Y;
                    if (point.Z > box.Max.Z) box.Max.Z = point.Z;
                }
                return box;
            }
        }
    }
}
