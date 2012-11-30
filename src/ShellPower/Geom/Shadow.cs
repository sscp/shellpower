using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenTK;

namespace SSCP.ShellPower {
    public class Shadow {

        public Vector4 Light { get; set; }
        public Mesh Mesh { get; private set; }
        public List<Pair<int>> SilhouetteEdges { get; private set; }

        private class Edge {
            public List<int> triangles = new List<int>();
            public int pointA, pointB;
        }
        private List<Edge>[] edgeBuckets;
        private List<Edge> edges = new List<Edge>();

        public Shadow(Mesh mesh) {
            this.Mesh = mesh;
            this.SilhouetteEdges = new List<Pair<int>>();
        }

        public void Initialize() {
            ComputeEdges();
        }

        private void ComputeEdges() {
            var start = DateTime.Now;

            Logger.info("compute map of edges -> adjacent triangles"); 
            Dictionary<Pair<int>, Edge> edgeMap = new Dictionary<Pair<int>, Edge>();
            for (int i = 0; i < Mesh.triangles.Length; i++) {
                var triangle = Mesh.triangles[i];
                var ab = PairUtils.Order(triangle.vertexA, triangle.vertexB);
                var ac = PairUtils.Order(triangle.vertexA, triangle.vertexC);
                var bc = PairUtils.Order(triangle.vertexB, triangle.vertexC);
                if (!edgeMap.ContainsKey(ab)) {
                    edgeMap.Add(ab, new Edge() { pointA = triangle.vertexA, pointB = triangle.vertexB });
                } 
                if (!edgeMap.ContainsKey(ac)){
                    edgeMap.Add(ac, new Edge() { pointA = triangle.vertexA, pointB = triangle.vertexC });
                }
                if (!edgeMap.ContainsKey(bc)){
                    edgeMap.Add(bc, new Edge() { pointA = triangle.vertexB, pointB = triangle.vertexC });
                }
                edgeMap[ab].triangles.Add(i);
                edgeMap[ac].triangles.Add(i);
                edgeMap[bc].triangles.Add(i);
            }
            edges = edgeMap.Values.ToList();

            /* sanity check--output # of regular edges (w/ 2 adj triangles) */
            int numRegular = 0;
            foreach (var edge in edges) {
                if (edge.triangles.Count == 2) {
                    numRegular++;
                } else if (edge.triangles.Count > 2) {
                    throw new Exception("wtf");
                }
            }
            Logger.info("calculated {0} edges ({1} regular) in {2:0.0}ms", 
                edges.Count, numRegular, (DateTime.Now - start).TotalSeconds * 1000);
        }

        /// <summary>
        /// Does a stupid mesh transformation. Probably obsolete.
        /// </summary>
        private void DeleteLoneEdges(ICollection<Edge> edgeColl) {
            Logger.info("deleting triangles that aren't part of the mesh");
            int[] adjacencies = new int[Mesh.triangles.Length];
            foreach (var edge in edgeColl) {
                if (edge.triangles.Count >= 2) {
                    foreach (var triIx in edge.triangles) {
                        adjacencies[triIx]++;
                    }
                }
            }
            List<Mesh.Triangle> validTris = new List<Mesh.Triangle>();
            int[] newIndices = new int[Mesh.triangles.Length];
            for (int i = 0; i < Mesh.triangles.Length; i++) {
                if (adjacencies[i] >= 3) {
                    validTris.Add(Mesh.triangles[i]);
                    newIndices[i] = validTris.Count - 1;
                } else {
                    newIndices[i] = -1;
                }
            }
            Mesh.triangles = validTris.ToArray();

            Logger.info("deleting corresponding edges...");
            edges.Clear();
            foreach (var edge in edgeColl) {
                bool good = true;
                for (int i = 0; i < edge.triangles.Count; i++) {
                    int ix = newIndices[edge.triangles[i]];
                    // negative ix means bad triangle -> bad edge.
                    good &= (ix >= 0);
                    edge.triangles[i] = ix;
                }
                if (good) {
                    edges.Add(edge);
                }
            }
        }

        public void ComputeShadows() {
            LightXYZ = Light.Xyz / (Light.W + 0.01f);
            ComputeSilhouette();
        }
        
        /// <summary>
        /// find each edge in the mesh, and find out whether its part of a silhouette
        /// (specifically, if one of the adjacent faces is facing the light and the other is not)
        /// (more specifically, if dot(face1.normal, light direction) * dot(face2.normal, light direction) < 0)
        /// </summary>
        private void ComputeSilhouette() {
            /* organize edges by angle relative to the 
             * point light source for fast lookup */
            int nbuckets = 20000;
            edgeBuckets = new List<Edge>[nbuckets];
            for (int i = 0; i < nbuckets; i++) {
                edgeBuckets[i] = new List<Edge>();
            }
            var vL = LightXYZ;

            /* create silhouettes */
            SilhouetteEdges.Clear();
            foreach (var edge in edges
                .Where((edge) => {
                    if (edge.triangles.Count < 2) {
                        return false;
                    } else if (edge.triangles.Count > 2) {
                        throw new Exception("wtf");
                    }
                    var norm1 = Mesh.triangles[edge.triangles[0]].normal;
                    var norm2 = Mesh.triangles[edge.triangles[1]].normal;

                    float xy =
                        Vector3.Dot(norm1, new Vector3(Light.X, Light.Y, Light.Z)) *
                        Vector3.Dot(norm2, new Vector3(Light.X, Light.Y, Light.Z));
                    // xy will be exactly zero for edges on the mesh boundary, with only one adj face
                    return xy <= 0;
                })) {
                var pA = Mesh.points[edge.pointA];
                var pB = Mesh.points[edge.pointB];

                // add to internal lookup table
                int bucketA = GetBucket(pA);
                int bucketB = GetBucket(pB);
                for (int i = Math.Min(bucketA, bucketB); i <= Math.Max(bucketA, bucketB); i++) {
                    edgeBuckets[i].Add(edge);
                }

                // add to public silhouette
                Pair<int> edgeIxs = PairUtils.Order(edge.pointA, edge.pointB);
                SilhouetteEdges.Add(edgeIxs);
            }
        }

        private Vector3 ComputeNormal(Mesh.Triangle triangle) {
            var normal = Vector3.Cross(
                Mesh.points[triangle.vertexB] - Mesh.points[triangle.vertexA],
                Mesh.points[triangle.vertexC] - Mesh.points[triangle.vertexA]);
            normal.Normalize();
            return normal;
        }

        /// <summary>
        /// The math is easier assuming a point light source
        /// that has XYZ coordinates (instead of a light source at infinity,
        /// XYZW with W=0). 
        /// 
        /// So we just create a source that's far away. A bit jank, but effective.
        /// </summary>
        private Vector3 LightXYZ { get; set; }

        private int GetBucket(Vector3 v) {
            var vL = LightXYZ;
            int ix = (int)(Math.Atan2(v.X - vL.X, v.Z - vL.Z) * edgeBuckets.Length / (2 * Math.PI));
            return Math.Max(0, ix);
        }

        public bool IsInShadow(Vector3 v) {
            int nInt = 0;
            var vL = LightXYZ;
            int bucket = GetBucket(v);
            foreach (Edge edge in edgeBuckets[bucket]) {
                var vA = Mesh.points[edge.pointA];
                var vB = Mesh.points[edge.pointB];
                // first, see if <vL.x, vL.z> overlaps the shadow in the XZ plane
                var zInterceptA = (v.X - vL.X) / (vA.X - vL.X) * (vA.Z - vL.Z) + vL.Z;
                var zInterceptB = (v.X - vL.X) / (vB.X - vL.X) * (vB.Z - vL.Z) + vL.Z;
                int nIntXZ = 0;
                if (v.Z < 0) {
                    //Logger.info("wtf");
                }
                if (zInterceptA > v.Z) {
                    nIntXZ++;
                }
                if (zInterceptB > v.Z) {
                    nIntXZ++;
                }
                bool inSector = (zInterceptA > v.Z) ^ (zInterceptB > v.Z);
                bool inShadowXZ = ((v.Z - vA.Z) > 0 == (vA.Z - vL.Z) > 0) && 
                                  ((v.Z - vB.Z) > 0 == (vB.Z - vL.Z) > 0);
                if (!inSector || !inShadowXZ) {
                    continue;
                }

                //ok, now check if a ray from the point upwars (+Y dir) intersects the shadow vol
                // make vL the origin. then, solve a system of equations:
                // ya = A*xa + B*za
                // yb = A*xb + B*zb
                // so...
                // ya/za = A*xa/za + B
                // yb/zb = A*xb/zb + B
                // a = (ya / za - yb / zb) / (xa / za - xb / zb)
                //   = (ya * zb - yb * za) / (xa * zb - xb * za)
                // similar for b

                float xa = vA.X - vL.X;
                float xb = vB.X - vL.X;
                float ya = vA.Y - vL.Y;
                float yb = vB.Y - vL.Y;
                float za = vA.Z - vL.Z;
                float zb = vB.Z - vL.Z;
                /* not numerically stable... 
                float a = (ya / za - yb / zb) / (xa / za - xb / zb);
                float b = (ya / xa - yb / xb) / (za / xa - zb / xb);
                float b1 = (ya - a * xa) / za;
                float b2 = (yb - a * xb) / zb;
                if(Math.Abs(b1-b2) > 1e-5) Debug.Fail("math error. "+b1+" vs "+b2);*/
                float a = (ya * zb - yb * za) / (xa * zb - xb * za);
                float b = (ya * xb - yb * xa) / (za * xb - zb * xa);

                /* double check */
                float yAdebug = a * xa + b * za + vL.Y;
                float yBdebug = a * xb + b * zb + vL.Y;
                if (Math.Abs(vA.Y - yAdebug) + Math.Abs(vB.Y - yBdebug) > 1e-4) {
                    //Logger.warn("numeric instability");
                } 
                float yint = a * (v.X - vL.X) + b * (v.Z - vL.Z) + vL.Y;
                if (yint > v.Y) {
                    nInt++;
                }
            }
            // if a ray traced in any dir (in this case, +Y axis) intersects
            // the volume boundary an odd number of times, then we're in the volume
            return (nInt % 2) == 1;

            //Logger.info("overlap " + nInt);
            //return nInt > 0;
        }
    }
}
