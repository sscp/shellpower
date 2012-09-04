using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenTK;

namespace SSCP.ShellPower {
    public static class MeshUtils {
        public static Mesh MirrorAndCombine(Mesh mesh, Vector3 axis) {
            /* if the current mesh has n points, we'll have up to 2n-1 points in the mirrored and combined one */
            var points = new List<Vector3>();
            points.AddRange(mesh.points);
            /* find the point or points we're going to mirror around--the ones closest in the 'axis' direction
             * (in other words, find the mirror plane) */
            var minDot = mesh.points.Min(point => Vector3.Dot(point, axis));
            /* if points are this close or closer to the mirror plane, they will not be duplicated--they will simply be shared by additional triangles */
            var epsilon = 0.002f;
            /* create a dictionary mapping point indices in the original mesh to the corresponding mirrored points */
            var pointIndexMap = new Dictionary<int, int>();

            for (int i = 0; i < mesh.points.Length; i++) {
                var distance = Vector3.Dot(mesh.points[i], axis) - minDot;
                if (distance < epsilon) {
                    /* point lies on mirror plane */
                    pointIndexMap.Add(i, i);
                } else {
                    /* point is not on mirror plane; create its mirror image as a new point */
                    pointIndexMap.Add(i, points.Count);
                    var mirrorPoint = mesh.points[i] - 2 * distance * axis;
                    points.Add(mirrorPoint);
                }
            }

            /* normals correspond to points, but may need to be mirrored.
             * if a point is on the mirror plane, make sure the corresponding normal is in the mirror plane
             */
            var normals = new Vector3[points.Count];
            for (int i = 0; i < mesh.points.Length; i++) {
                if (pointIndexMap[i] == i) {
                    normals[i] = mesh.normals[i] - Vector3.Dot(mesh.normals[i], axis) * axis;
                    normals[i].Normalize();
                } else {
                    normals[i] = mesh.normals[i];
                    normals[pointIndexMap[i]] = mesh.normals[i] - 2 * Vector3.Dot(mesh.normals[i], axis) * axis;
                }
            }

            /* triangles are simply duplicated--each triangle gets a mirror image */
            var triangles = new Mesh.Triangle[mesh.triangles.Length * 2];
            for (int i = 0; i < mesh.triangles.Length; i++) {
                triangles[i] = mesh.triangles[i];
            }
            for (int i = 0; i < mesh.triangles.Length; i++) {
                var triangle = mesh.triangles[i];
                triangles[mesh.triangles.Length + i] = new Mesh.Triangle() {
                    vertexA = pointIndexMap[triangle.vertexA],
                    vertexB = pointIndexMap[triangle.vertexB],
                    vertexC = pointIndexMap[triangle.vertexC]
                };
            }

            return new Mesh() {
                points = points.ToArray(),
                normals = normals,
                triangles = triangles
            };
        }

        public static void Split(Mesh input, IVolume volume, 
            out Mesh output, out int[] ixTrisInside) {

            /**
             *
             *  for triangle in input
             *      if triangle is entirely inside or outside
             *         add to corresponding mesh
             *      else split triangle into 4 pieces.
             *         three go in one mesh, one goes into the other
             *      
             * Splitting a triangle, Before:
             * 
             *                *    vA
             *               / \
             * boundary --- /---\ ---
             *             /     \
             *       vB1  *-------*  vB2
             * 
             * 
             * After:
             * 
             *                * vA
             *         vMid1 / \ vMid2
             * boundary --- *---* ---
             *             / \ / \
             *       vB1  *---*---*  vB2
             *              vMidB
             * 
             **/

            int np = input.points.Length, nt = input.triangles.Length;
            List<Vector3> points = input.points.ToList();
            List<Vector3> norms = input.normals.ToList();
            List<Mesh.Triangle> tris = new List<Mesh.Triangle>();
            List<int> trisInside = new List<int>();
            for(int i = 0; i < nt; i++){
                var tri = input.triangles[i];
                Vector3 vA = input.points[tri.vertexA];
                Vector3 vB = input.points[tri.vertexB];
                Vector3 vC = input.points[tri.vertexC];
                int ncontains = 
                    (volume.Contains(vA) ? 1 : 0) + 
                    (volume.Contains(vB) ? 1 : 0) + 
                    (volume.Contains(vC) ? 1 : 0);
                if (ncontains==3) {
                    tris.Add(tri);
                    trisInside.Add(tris.Count-1);
                } else if (ncontains==0){
                    tris.Add(tri);
                } else {
                    // see comment above for explanation
                    Debug.Assert(ncontains == 1 || ncontains == 2);
                    bool containsA = (ncontains==1);
                    int ixA, ixB1, ixB2;
                    if (volume.Contains(vA) == containsA) {
                        ixA = tri.vertexA;
                        ixB1 = tri.vertexB;
                        ixB2 = tri.vertexC;
                    } else if (volume.Contains(vB) == containsA) {
                        ixA = tri.vertexB;
                        ixB1 = tri.vertexA;
                        ixB2 = tri.vertexC;
                    } else {
                        Debug.Assert(volume.Contains(vC) == containsA);
                        ixA = tri.vertexC;
                        ixB1 = tri.vertexA;
                        ixB2 = tri.vertexB;
                    }
                    Vector3 vAO = input.points[ixA];
                    Vector3 vB1 = input.points[ixB1];
                    Vector3 vB2 = input.points[ixB2];
                    Vector3 vMid1 = FindBoundary(vAO, vB1, volume);
                    Vector3 vMid2 = FindBoundary(vAO, vB2, volume);
                    Vector3 vMidB = input.points[ixB1]*0.5f + input.points[ixB2]*0.5f;
                    points.Add(vMid1); points.Add(vMid2); points.Add(vMidB);
                    float b1 = (vMid1 - vAO).Length / ((vB1 - vAO).Length + float.Epsilon);
                    float b2 = (vMid2 - vAO).Length / ((vB2 - vAO).Length + float.Epsilon);
                    Debug.Assert(0 <= b1 && b1 <= 1 && 0 <= b2 && b2 <= 1);
                    Vector3 nMid1 = input.normals[ixA] * (1 - b1) + input.normals[ixB1] * b1;
                    Vector3 nMid2 = input.normals[ixA] * (1 - b2) + input.normals[ixB2] * b2;
                    Vector3 nMidB = input.normals[ixB1] * 0.5f + input.normals[ixB2] * 0.5f;
                    norms.Add(nMid1); norms.Add(nMid2); norms.Add(nMidB);
                    tris.Add(new Mesh.Triangle(ixA, points.Count - 3, points.Count - 2));
                    tris.Add(new Mesh.Triangle(ixB1, points.Count - 3, points.Count - 1));
                    tris.Add(new Mesh.Triangle(points.Count - 3, points.Count - 2, points.Count - 1));
                    tris.Add(new Mesh.Triangle(ixB2, points.Count - 2, points.Count - 1));
                    if (containsA) {
                        trisInside.Add(tris.Count - 4);
                    } else {
                        trisInside.Add(tris.Count - 3);
                        trisInside.Add(tris.Count - 2);
                        trisInside.Add(tris.Count - 1);
                    }
                }
            }

            // done
            output = new Mesh() {
                points = points.ToArray(),
                normals = norms.ToArray(),
                triangles = tris.ToArray()
            };
            ixTrisInside = trisInside.ToArray();
        }

        /// <summary>
        /// Takes two points, one of which is in the volume and one of which isn't.
        /// 
        /// Finds the distance along the segment a...b that intersects the boundary of the volume.
        /// 
        /// Note: this assumes that there is only one such intersection! If this is not the case, use a finer mesh.
        /// </summary>
        public static Vector3 FindBoundary(Vector3 a, Vector3 b, IVolume volume) {
            Debug.Assert(volume.Contains(a) != volume.Contains(b));
            bool contains = volume.Contains(a);
            Vector3 v1 = a, v2 = b;
            for (int i = 0; i < 10; i++) {
                Vector3 mid = (v1+v2)/2;
                if (contains == volume.Contains(mid)) {
                    v1 = mid;
                } else {
                    v2 = mid;
                }
            }
            return (v1 + v2) / 2;
        }

        /// <summary>
        /// If two mesh points have the same (X, Y, Z) coords, then they get combined into one.
        /// </summary>
        public static void JoinVertices(Mesh mesh) {
            Dictionary<Vector3, int> pointIxMap = new Dictionary<Vector3, int>();
            List<Vector3> points = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            for (int i = 0; i < mesh.points.Length; i++) {
                if (!pointIxMap.ContainsKey(mesh.points[i])) {
                    points.Add(mesh.points[i]);
                    norms.Add(mesh.normals[i]);
                    pointIxMap.Add(mesh.points[i], points.Count-1);
                }
            }
            Logger.info("mesh has " + mesh.points.Length + " verts, "+
                "joined " + (mesh.points.Length - pointIxMap.Count) + " dupes");
            for (int i = 0; i < mesh.triangles.Length; i++) {
                Mesh.Triangle tri = mesh.triangles[i];
                mesh.triangles[i].vertexA = pointIxMap[mesh.points[tri.vertexA]];
                mesh.triangles[i].vertexB = pointIxMap[mesh.points[tri.vertexB]];
                mesh.triangles[i].vertexC = pointIxMap[mesh.points[tri.vertexC]];
            }
            mesh.points = points.ToArray();
            mesh.normals = norms.ToArray();
        }
    }
}
