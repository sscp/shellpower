using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    /// <summary>
    /// Represents a shadow cast by a single mesh and a singlje lightsource.
    /// Lets you query the shadow volume (to see if a vector is in shadow or not).
    /// </summary>
    public class ShadowVolume : IVolume {
        public Mesh Mesh { get; private set; }
        public Vector4 LightSource { get; set; }
        public List<int> SilhouettePoints { get; private set; }
        public float[,] QuadMatrices { get; private set; }

        public ShadowVolume(Mesh mesh, Vector4 lightSource) {
            this.Mesh = mesh;
            this.LightSource = lightSource;
            SilhouettePoints = new List<int>();
        }

        public void CalculateQuadMatrices() {
            var light = new Vector3(LightSource.X, LightSource.Y, LightSource.Z) / (LightSource.W + 0.000001f);
            QuadMatrices = new float[SilhouettePoints.Count, 6];
            for (int i = 0; i < SilhouettePoints.Count; i++) {
                Vector3 pointA = Mesh.points[i];
                Vector3 pointB = Mesh.points[(i + 1) % SilhouettePoints.Count];
                //qm0 * a.X + qm1 * a.Y = 1
                //qm0 * b.X + qm1 * b.Y = 0
                //qm2 * a.X + qm3 * a.Y = 0
                //qm2 * b.X + qm3 * b.Y = 1
                //[a, b] * [qm0, qm1] = [1, 0]
                //[a, b] * [qm2, qm3] = [0, 1]
                //[a, b] * [qm4, qm5] = [a.Z, b.Z]
                //[qm0, qm1] = [a,b]^-1 * [1, 0] = 1/(a0b1-b0a1) [b1 -a1, -b0 a0] * [1, 0] = 1/(a0b1-b0a1) [b1 -a1]
                //[qm0, qm1] = [a,b]^-1 * [0, 1] 
                //[qm0, qm1] = [a,b]^-1 * [a.Z, b.Z]

                var mult = 1f / (pointA.X * pointB.Y - pointB.X * pointA.Y);
                QuadMatrices[i, 0] = mult * pointB.Y;
                QuadMatrices[i, 1] = mult * -pointB.X;
                QuadMatrices[i, 2] = mult * -pointA.Y;
                QuadMatrices[i, 3] = mult * pointA.X;
                QuadMatrices[i, 4] = pointA.Z * QuadMatrices[i, 0] + pointB.Z * QuadMatrices[i, 2];
                QuadMatrices[i, 5] = pointA.Z * QuadMatrices[i, 1] + pointB.Z * QuadMatrices[i, 3];
            }
        }

        public bool Contains(Vector3 point) {
            int intersectionsBelow = 0;
            for (int i = 0; i < SilhouettePoints.Count; i++) {
                float u = QuadMatrices[i, 0] * point.X + QuadMatrices[i, 1] * point.Y;
                float v = QuadMatrices[i, 2] * point.X + QuadMatrices[i, 3] * point.Y;
                float z = QuadMatrices[i, 4] * point.X + QuadMatrices[i, 5] * point.Y;
                if (z < point.Z)
                    intersectionsBelow++;
            }
            return intersectionsBelow % 2 > 0;
        }
    }
}
