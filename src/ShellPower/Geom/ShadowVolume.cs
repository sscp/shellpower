using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    /// <summary>
    /// Represents a shadow cast by a single mesh and a single lightsource.
    /// Lets you query the shadow volume (to see if a vector is in shadow or not).
    /// </summary>
    public class ShadowVolume {
        public Mesh mesh { get; private set; }
        public Vector4 lightSource { get; set; }
        public List<int> silhouettePoints { get; private set; }
        public float[,] quadMatrices { get; private set; }

        public ShadowVolume(Mesh mesh, Vector4 lightSource) {
            this.mesh = mesh;
            this.lightSource = lightSource;
            silhouettePoints = new List<int>();
        }

        public void CalculateQuadMatrices() {
            var light = new Vector3(lightSource.X, lightSource.Y, lightSource.Z) / (lightSource.W + 0.000001f);
            quadMatrices = new float[silhouettePoints.Count, 6];
            for (int i = 0; i < silhouettePoints.Count; i++) {
                Vector3 pointA = mesh.points[i];
                Vector3 pointB = mesh.points[(i + 1) % silhouettePoints.Count];
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
                quadMatrices[i, 0] = mult * pointB.Y;
                quadMatrices[i, 1] = mult * -pointB.X;
                quadMatrices[i, 2] = mult * -pointA.Y;
                quadMatrices[i, 3] = mult * pointA.X;
                quadMatrices[i, 4] = pointA.Z * quadMatrices[i, 0] + pointB.Z * quadMatrices[i, 2];
                quadMatrices[i, 5] = pointA.Z * quadMatrices[i, 1] + pointB.Z * quadMatrices[i, 3];
            }
        }
        public bool contains(Vector3 point) {
            int intersectionsBelow = 0;
            for (int i = 0; i < silhouettePoints.Count; i++) {
                float u = quadMatrices[i, 0] * point.X + quadMatrices[i, 1] * point.Y;
                float v = quadMatrices[i, 2] * point.X + quadMatrices[i, 3] * point.Y;
                float z = quadMatrices[i, 4] * point.X + quadMatrices[i, 5] * point.Y;
                if (z < point.Z)
                    intersectionsBelow++;
            }
            return intersectionsBelow % 2 > 0;
        }
    }
}
