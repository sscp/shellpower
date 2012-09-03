using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace SSCP.ShellPower {
    public class IgesNurbMesher {
        public static Mesh mesh(IEnumerable<IgesRationalBSplineSurfaceEntity> surfaces) {
            int[] offsets = new int[surfaces.Count() + 1];
            int n = surfaces.Sum(surf => surf.ControlPoints.Length);
            var points = new Vector3[n];
            var norms = new Vector3[n];

            int[] triOffsets = new int[surfaces.Count() + 1];
            int nTris = surfaces.Sum(surf => (surf.UControlPoints - 1) * (surf.VControlPoints - 1) * 2);
            var tris = new Mesh.Triangle[nTris];

            int i = 0;
            foreach (var surf in surfaces) {
                for (int j = 0; j < surf.UControlPoints; j++) {
                    for (int k = 0; k < surf.VControlPoints; k++) {
                        int ix = offsets[i] + surf.VControlPoints * j + k;
                        points[ix] = (Vector3)surf.ControlPoints[j, k];
                        norms[ix] =
                            (Vector3)Vector3d.Cross(
                            surf.ControlPoints[Math.Min(j + 1, surf.UControlPoints - 1), k] - surf.ControlPoints[Math.Max(j - 1, 0), k],
                            surf.ControlPoints[j, Math.Min(k + 1, surf.VControlPoints - 1)] - surf.ControlPoints[j, Math.Max(k - 1, 0)]);
                        norms[ix].Normalize();

                        if (j < surf.UControlPoints - 1 && k < surf.VControlPoints - 1) {
                            tris[triOffsets[i] + (j + (surf.UControlPoints - 1) * k) * 2] = new Mesh.Triangle() {
                                vertexA = offsets[i] + surf.VControlPoints * j + k,
                                vertexB = offsets[i] + surf.VControlPoints * (j + 1) + k,
                                vertexC = offsets[i] + surf.VControlPoints * j + (k + 1)
                            };
                            tris[triOffsets[i] + (j + (surf.UControlPoints - 1) * k) * 2 + 1] = new Mesh.Triangle() {
                                vertexA = offsets[i] + surf.VControlPoints * (j + 1) + (k + 1),
                                vertexC = offsets[i] + surf.VControlPoints * j + (k + 1),
                                vertexB = offsets[i] + surf.VControlPoints * (j + 1) + k
                            };
                        }
                    }
                }
                triOffsets[i + 1] = triOffsets[i] + (surf.UControlPoints - 1) * (surf.VControlPoints - 1) * 2;
                offsets[i + 1] = offsets[i] + surf.ControlPoints.Length;
                i++;
            }

            return new Mesh() {
                points = points,
                normals = norms,
                triangles = tris
            };
        }
    }
}
