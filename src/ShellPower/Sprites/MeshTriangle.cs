using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenTK;

namespace SSCP.ShellPower {
    struct MeshTriangle : IBoundingBox {
        public MeshSprite Sprite { get; set; }
        public int Triangle { get; set; }
        public Quad3 BoundingBox {
            get {
                Debug.Assert(Sprite.Triangles[Triangle].VertexA >= 0);
                Vector3
                    a = Sprite.Points[Sprite.Triangles[Triangle].VertexA],
                    b = Sprite.Points[Sprite.Triangles[Triangle].VertexB],
                    c = Sprite.Points[Sprite.Triangles[Triangle].VertexC];
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
