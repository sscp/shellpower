using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            out Mesh inside, out Mesh outside) {
            inside = new Mesh();
            outside = new Mesh();
            
            /**
             *  for triangle in input
             *      if triangle is entirely inside or outside
             *         add to corresponding mesh
             *      else split triangle into either 2 or 4 pieces
             **/
            //TODO: complete
        }

        /// <summary>
        /// If two mesh points have the same (X, Y, Z) coords, then they get combined into one.
        /// </summary>
        public static void JoinVertices(Mesh mesh) {
            Dictionary<Vector3, int> pointIxMap = new Dictionary<Vector3, int>();
            for (int i = 0; i < mesh.points.Length; i++) {
                if (!pointIxMap.ContainsKey(mesh.points[i])) {
                    pointIxMap.Add(mesh.points[i], i);
                }
            }
            Logger.info("mesh has " + mesh.points.Length + " verts, "+
                "joined " + (mesh.points.Length - pointIxMap.Count) + " dupes");
            for (int i = 0; i < mesh.triangles.Length; i++) {
                Mesh.Triangle tri = mesh.triangles[i];
                tri.vertexA = pointIxMap[mesh.points[tri.vertexA]];
                tri.vertexB = pointIxMap[mesh.points[tri.vertexB]];
                tri.vertexC = pointIxMap[mesh.points[tri.vertexC]];
            }
        }
    }
}
