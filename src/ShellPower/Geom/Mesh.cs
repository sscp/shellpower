using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    /// <summary>
    /// Represents an arbitrary 3D shape.
    /// 
    /// Triangles only, no quads or higher-order shapes. Per-vertex normals. All coordinates are in meters.
    /// </summary>
    public class Mesh : IBoundingBox {
        public struct Triangle {
            public Triangle(int vA, int vB, int vC) {
                vertexA = vA;
                vertexB = vB;
                vertexC = vC;
                normal = new Vector3(0, 0, 0);
            }
            public readonly int vertexA, vertexB, vertexC;
            public Vector3 normal;
        }
        public Mesh(Vector3[] points, Vector3[] normals, Triangle[] triangles) {
            this.points = points;
            this.normals = normals;
            this.triangles = triangles;
        }
        public readonly Vector3[] points;
        public readonly Vector3[] normals;
        public readonly Triangle[] triangles;

        private Quad3? boundingBox;
        public Quad3 BoundingBox {
            get {
                if (boundingBox == null) {
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
                    boundingBox = box;
                }

                return boundingBox.Value;
            }
        }
    }
}
