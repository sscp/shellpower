using OpenTK;
using OpenTK.Graphics;

namespace SSCP.ShellPower {
    public class MeshSprite : Sprite {
        public struct Triangle {
            public int VertexA, VertexB, VertexC;
        }
        public Vector3[] Points;
        public Vector3[] Normals;
        public Triangle[] Triangles;

        public Vector2[] TextureCoordinates;
        public Vector4[] VertexColors;
        public Vector4[] FaceColors;
        public bool IsWireframe { get; set; }

        public MeshSprite() {
        }

        public override void Render() {
            GL.Color3(1f, 1, 1);
            if (!IsWireframe)
                GL.Begin(BeginMode.Triangles);
            for (int i = 0; i < Triangles.Length; i++) {
                var triangle = Triangles[i];
                if (triangle.VertexA < 0)
                    continue;
                if (IsWireframe)
                    GL.Begin(BeginMode.LineStrip);
                //TODO: texture coords
                if (FaceColors != null) GL.Color4(FaceColors[i]);

                //draw triangle
                if (VertexColors != null) GL.Color4(VertexColors[triangle.VertexA]);
                GL.Normal3(Normals[triangle.VertexA]);
                GL.Vertex4(new Vector4(Points[triangle.VertexA], 1));

                if (VertexColors != null) GL.Color4(VertexColors[triangle.VertexB]);
                GL.Normal3(Normals[triangle.VertexB]);
                GL.Vertex4(new Vector4(Points[triangle.VertexB], 1));

                if (VertexColors != null) GL.Color4(VertexColors[triangle.VertexC]);
                GL.Normal3(Normals[triangle.VertexC]);
                GL.Vertex4(new Vector4(Points[triangle.VertexC], 1));

                if (IsWireframe)
                    GL.End();
            }
            if (!IsWireframe)
                GL.End();
        }
        public override void Dispose() {
            base.Dispose();
            Points = null;
            Normals = null;
            Triangles = null;
        }

        public Quad3 BoundingBox {
            get {
                var box = new Quad3();
                box.Min = box.Max = Points[0];
                foreach (var point in Points) {
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

        /// <summary>
        /// Returns a depth t such that position + direction*t = a point in triangle. Note that this may be a positive or negative number.
        /// Returns NaN if the ray does not intersect the triangle or if direction is the zero vector.
        /// </summary>
        public float Intersect(Triangle triangle, Vector3 position, Vector3 direction) {
            //transform coords so that position=0, triangle = (v1, v2, v3)
            // find a b c such that
            // a*v1x + b*v1y + c*v1z = 1
            // a*v2x + b*v2y + c*v2z = 1
            // a*v3x + b*v3y + c*v3z = 1

            // (v1 v2 v3)T(a b c)T = (1 1 1)
            // (a b c) = (v1 v2 v3)T^-1 (1 1 1)
            var m = new Matrix3(
                Points[triangle.VertexA],
                Points[triangle.VertexB],
                Points[triangle.VertexC]);
            m.Transpose();
            var inv = m.Inverse;
            var abc = inv * new Vector3(1, 1, 1);

            //(p + t*d) dot (a b c) = 1
            //p dot (a b c) + t*d dot (a b c) = 1
            //t = (1 - p dot (a b c)) / (d dot (a b c))
            var t = (1f - Vector3.Dot(position, abc)) / Vector3.Dot(direction, abc);
            var intersection = position + direction * t;

            //next, find the intersection
            //i = p + t*d
            //ap*v1 + bp*v2 + cp*v3 = i
            //(ap bp cp) = (v1 v2 v3)T^-1 (1 1 1)
            var abcPrime = inv * intersection;

            //if any of the components (ap bp cp) is outside of [0, 1], 
            //then the ray does not intersect the triangle
            if (abcPrime.X < 0 || abcPrime.X > 1)
                return float.NaN;
            if (abcPrime.Y < 0 || abcPrime.Y > 1)
                return float.NaN;
            if (abcPrime.Z < 0 || abcPrime.Z > 1)
                return float.NaN;
            return t;
        }
    }
}
