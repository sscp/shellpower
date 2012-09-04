using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    public class Shadow {
        private class Edge {
            public List<int> triangles = new List<int>();
            public Vector3 normalA, normalB;
            public int pointA, pointB;
            public void addNormal(Vector3 normal) {
                if (normalA == Vector3.Zero) {
                    normalA = normal;
                } else {
                    normalB = normal;
                }
            }
        }

        public Vector4 Light { get; set; }
        public Mesh Mesh { get; private set; }
        public List<ShadowVolume> ShadowVolumes { get; private set; }
        public List<Pair<int>> SilhouetteEdges { get; private set; }
        public bool[] VertShadows { get; private set; }
        List<Edge> edges = new List<Edge>();


        public Shadow(Mesh mesh) {
            this.Mesh = mesh;
            this.ShadowVolumes = new List<ShadowVolume>();
            this.SilhouetteEdges = new List<Pair<int>>();
            this.VertShadows = new bool[mesh.points.Length];
        }

        public void Initialize() {
            MeshUtils.JoinVertices(Mesh);
            ComputeEdges();
        }

        private void ComputeEdges() {
            var start = DateTime.Now;

            /* compute map of edges -> adjacent triangles */
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

            /* delete triangles that aren't part of the mesh */
            int[] adjacencies = new int[Mesh.triangles.Length];
            foreach (var edge in edgeMap.Values) {
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

            /* keep only the valid edges */
            ICollection<Edge> edgeColl = edgeMap.Values;
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

            /* compute normals */
            foreach (var edge in edges) {
                foreach (var triIx in edge.triangles) {
                    var triangle = Mesh.triangles[triIx];
                    if (triangle.vertexA < 0) {
                        continue;
                    }
                    var normal = Vector3.Cross(
                        Mesh.points[triangle.vertexB] - Mesh.points[triangle.vertexA], 
                        Mesh.points[triangle.vertexC] - Mesh.points[triangle.vertexA]);
                    if (normal.Y < 0) {
                        normal = -normal;
                    }
                    edge.addNormal(normal);
                }
            }
            Logger.info("calculated edges in {0:0.0}ms", (DateTime.Now - start).TotalSeconds * 1000);
        }

        public void ComputeShadows() {
            ComputeShadowVolumes();
            ComputeVertShadows();
        }

        public void ComputeVertShadows() {
            int np = Mesh.points.Length;
            this.VertShadows = new bool[np];
            for (int i = 0; i < np; i++) {
                VertShadows[i] = IsInShadow(Mesh.points[i]);
            }
        }

        private void ComputeShadowVolumes() {
            /* find each edge in the mesh, and find out whether its part of a silhouette
             * (specifically, if one of the adjacent faces is facing the light and the other is not)
             * (more specifically, if dot(face1.normal, light direction) * dot(face2.normal, light direction) < 0)
             */
            SilhouetteEdges.Clear();

            /* create silhouettes and keep joining them until each one is a closed path */
            var volumes = new HashSet<ShadowVolume>();
            var volumeTable = new Dictionary<int, ShadowVolume>();
            foreach (var edge in edges
                .Where((edge) => {
                    float xy =
                        Vector3.Dot(edge.normalA, new Vector3(Light.X, Light.Y, Light.Z)) *
                        Vector3.Dot(edge.normalB, new Vector3(Light.X, Light.Y, Light.Z));
                    return xy < 0;
                })) {
                Pair<int> edgeIxs = PairUtils.Order(edge.pointA, edge.pointB);
                SilhouetteEdges.Add(edgeIxs);
                int v1 = edgeIxs.First, v2 = edgeIxs.Second;
                if (volumeTable.ContainsKey(v1)) {
                    if (volumeTable.ContainsKey(v2)) {
                        var vol1 = volumeTable[v1];
                        var vol2 = volumeTable[v2];
                        //Debug.Assert(vol1 != vol2);
                        if (vol1 == vol2)
                            continue;
                        if (vol1.SilhouettePoints.IndexOf(v1) > vol1.SilhouettePoints.Count / 2) {
                            vol1.SilhouettePoints.Reverse();
                            //Debug.Assert(vol1.silhouettePoints[0] == v1);
                            while (vol1.SilhouettePoints[0] != v1) {
                                volumeTable.Remove(vol1.SilhouettePoints[0]);
                                vol1.SilhouettePoints.RemoveAt(0);
                            }
                        }
                        if (vol2.SilhouettePoints.IndexOf(v2) <= vol2.SilhouettePoints.Count / 2) {
                            //Debug.Assert(vol2.silhouettePoints[0] == v2);
                            while (vol2.SilhouettePoints[0] != v2) {
                                volumeTable.Remove(vol2.SilhouettePoints[0]);
                                vol2.SilhouettePoints.RemoveAt(0);
                            }
                            vol2.SilhouettePoints.Reverse();
                        }
                        /* vol2 now ends next to the current edge,
                         * and vol1 begins next to the current edge,
                         * so we can join them */
                        vol2.SilhouettePoints.AddRange(vol1.SilhouettePoints);
                        foreach (var pointIx in vol1.SilhouettePoints)
                            volumeTable[pointIx] = vol2;
                        volumes.Add(vol2);
                    } else {
                        var pointIx = v1;
                        var vol = volumeTable[pointIx];
                        if (vol.SilhouettePoints[0] == pointIx ||
                            vol.SilhouettePoints[vol.SilhouettePoints.Count - 1] == pointIx) {
                            if (vol.SilhouettePoints[0] == pointIx)
                                vol.SilhouettePoints.Reverse();
                            vol.SilhouettePoints.Add(v2);
                            volumeTable.Add(v2, vol);
                        }
                    }
                } else if (volumeTable.ContainsKey(v2)) {
                    var pointIx = v2;
                    var vol = volumeTable[pointIx];
                    if (vol.SilhouettePoints[0] == pointIx ||
                        vol.SilhouettePoints[vol.SilhouettePoints.Count - 1] == pointIx) {
                        if (vol.SilhouettePoints[0] == pointIx)
                            vol.SilhouettePoints.Reverse();
                        vol.SilhouettePoints.Add(v1);
                        volumeTable.Add(v1, vol);
                    }
                } else {
                    var vol = new ShadowVolume(Mesh, Light);
                    vol.SilhouettePoints.Add(v1);
                    vol.SilhouettePoints.Add(v2);
                    volumeTable.Add(v1, vol);
                    volumeTable.Add(v2, vol);
                }
            }

            /* finally, now that we have all the shadow volumes, copy to list
             * and process so that we can quickly calc whether a point is in one of them */
            ShadowVolumes = new List<ShadowVolume>();
            foreach (var volume in volumes) {
                volume.CalculateQuadMatrices();
                ShadowVolumes.Add(volume);
            }
        }

        public bool IsInShadow(Vector3 point) {
            /* check if it's in the shadow cones */
            bool inVolume = false;
            foreach (var vol in ShadowVolumes) {
                if (vol.Contains(point)) {
                    inVolume = true;
                    break;
                }
            }
            return inVolume;
        }
    }
}
