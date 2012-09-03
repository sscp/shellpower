using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenTK;

namespace SSCP.ShellPower {
    struct MeshTriangle : IBoundingBox {
        public Mesh Mesh { get; set; }
        public int Triangle { get; set; }
        public Quad3 BoundingBox {
            get {
                Debug.Assert(Mesh.triangles[Triangle].vertexA >= 0);
                Vector3
                    a = Mesh.points[Mesh.triangles[Triangle].vertexA],
                    b = Mesh.points[Mesh.triangles[Triangle].vertexB],
                    c = Mesh.points[Mesh.triangles[Triangle].vertexC];
                Quad3 quad = new Quad3() {
                    Min = new Vector3(
                        a.X < b.X ? (a.X < c.X ? a.X : c.X) : (b.X < c.X ? b.X : c.X),
                        a.Y < b.Y ? (a.Y < c.Y ? a.Y : c.Y) : (b.Y < c.Y ? b.Y : c.Y),
                        a.Z < b.Z ? (a.Z < c.Z ? a.Z : c.Z) : (b.Z < c.Z ? b.Z : c.Z)),
                    Max = new Vector3(
                        a.X > b.X ? (a.X > c.X ? a.X : c.X) : (b.X > c.X ? b.X : c.X),
                        a.Y > b.Y ? (a.Y > c.Y ? a.Y : c.Y) : (b.Y > c.Y ? b.Y : c.Y),
                        a.Z > b.Z ? (a.Z > c.Z ? a.Z : c.Z) : (b.Z > c.Z ? b.Z : c.Z))
                };
                return quad;
            }
        }
    }
}
