using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace SSCP.ShellPower {
    /// <summary>
    /// Renders shadow volumes.
    /// </summary>
    public class ShadowMeshSprite : MeshSprite {
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

        List<ShadowVolume> shadowVolumes = new List<ShadowVolume>();
        List<Edge> edges = new List<Edge>();
        List<Pair<int>> silhouetteEdges = new List<Pair<int>>();

        public ShadowMeshSprite(MeshSprite other) {
            mesh = other.mesh;
            Position = other.Position;
            Transform = other.Transform;
        }

        public override void Initialize() {
            base.Initialize();
            MeshUtils.JoinVertices(mesh);
            ComputeEdges();
        }


        private void ComputeEdges() {
            var start = DateTime.Now;

            /* compute map of edges -> adjacent triangles */
            Dictionary<Pair<int>, Edge> edgeMap = new Dictionary<Pair<int>, Edge>();
            for (int i = 0; i < mesh.triangles.Length; i++) {
                var triangle = mesh.triangles[i];
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
            int[] adjacencies = new int[mesh.triangles.Length];
            foreach (var edge in edgeMap.Values) {
                if (edge.triangles.Count >= 2) {
                    foreach (var triIx in edge.triangles) {
                        adjacencies[triIx]++;
                    }
                }
            }
            List<Mesh.Triangle> validTris = new List<Mesh.Triangle>();
            int[] newIndices = new int[mesh.triangles.Length];
            for (int i = 0; i < mesh.triangles.Length; i++) {
                if (adjacencies[i] >= 3) {
                    validTris.Add(mesh.triangles[i]);
                    newIndices[i] = validTris.Count - 1;
                } else {
                    newIndices[i] = -1;
                }
            }
            mesh.triangles = validTris.ToArray();

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
                    var triangle = mesh.triangles[triIx];
                    if (triangle.vertexA < 0) {
                        continue;
                    }
                    var normal = Vector3.Cross(
                        mesh.points[triangle.vertexB] - mesh.points[triangle.vertexA], 
                        mesh.points[triangle.vertexC] - mesh.points[triangle.vertexA]);
                    if (normal.Y < 0) {
                        normal = -normal;
                    }
                    edge.addNormal(normal);
                }
            }
            Logger.info("calculated edges in {0:0.0}ms", (DateTime.Now - start).TotalSeconds * 1000);
        }

        public void ComputeShadowVolumes() {
            /* find each edge in the mesh, and find out whether its part of a silhouette
             * (specifically, if one of the adjacent faces is facing the light and the other is not)
             * (more specifically, if dot(face1.normal, light direction) * dot(face2.normal, light direction) < 0)
             */
            silhouetteEdges.Clear();

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
                silhouetteEdges.Add(edgeIxs);
                int v1 = edgeIxs.First, v2 = edgeIxs.Second;
                if (volumeTable.ContainsKey(v1)) {
                    if (volumeTable.ContainsKey(v2)) {
                        var vol1 = volumeTable[v1];
                        var vol2 = volumeTable[v2];
                        //Debug.Assert(vol1 != vol2);
                        if (vol1 == vol2)
                            continue;
                        if (vol1.silhouettePoints.IndexOf(v1) > vol1.silhouettePoints.Count / 2) {
                            vol1.silhouettePoints.Reverse();
                            //Debug.Assert(vol1.silhouettePoints[0] == v1);
                            while (vol1.silhouettePoints[0] != v1) {
                                volumeTable.Remove(vol1.silhouettePoints[0]);
                                vol1.silhouettePoints.RemoveAt(0);
                            }
                        }
                        if (vol2.silhouettePoints.IndexOf(v2) <= vol2.silhouettePoints.Count / 2) {
                            //Debug.Assert(vol2.silhouettePoints[0] == v2);
                            while (vol2.silhouettePoints[0] != v2) {
                                volumeTable.Remove(vol2.silhouettePoints[0]);
                                vol2.silhouettePoints.RemoveAt(0);
                            }
                            vol2.silhouettePoints.Reverse();
                        }
                        /* vol2 now ends next to the current edge,
                         * and vol1 begins next to the current edge,
                         * so we can join them */
                        vol2.silhouettePoints.AddRange(vol1.silhouettePoints);
                        foreach (var pointIx in vol1.silhouettePoints)
                            volumeTable[pointIx] = vol2;
                        volumes.Add(vol2);
                    } else {
                        var pointIx = v1;
                        var vol = volumeTable[pointIx];
                        if (vol.silhouettePoints[0] == pointIx ||
                            vol.silhouettePoints[vol.silhouettePoints.Count - 1] == pointIx) {
                            if (vol.silhouettePoints[0] == pointIx)
                                vol.silhouettePoints.Reverse();
                            vol.silhouettePoints.Add(v2);
                            volumeTable.Add(v2, vol);
                        }
                    }
                } else if (volumeTable.ContainsKey(v2)) {
                    var pointIx = v2;
                    var vol = volumeTable[pointIx];
                    if (vol.silhouettePoints[0] == pointIx ||
                        vol.silhouettePoints[vol.silhouettePoints.Count - 1] == pointIx) {
                        if (vol.silhouettePoints[0] == pointIx)
                            vol.silhouettePoints.Reverse();
                        vol.silhouettePoints.Add(v1);
                        volumeTable.Add(v1, vol);
                    }
                } else {
                    var vol = new ShadowVolume(mesh, Light);
                    vol.silhouettePoints.Add(v1);
                    vol.silhouettePoints.Add(v2);
                    volumeTable.Add(v1, vol);
                    volumeTable.Add(v2, vol);
                }
            }

            /* finally, now that we have all the shadow volumes, copy to list
             * and process so that we can quickly calc whether a point is in one of them */
            shadowVolumes = new List<ShadowVolume>();
            foreach (var volume in volumes) {
                volume.CalculateQuadMatrices();
                shadowVolumes.Add(volume);
            }
        }

        private void ComputeShadowByvertex() {
            vertexColors = new Vector4[mesh.points.Length];
            for (int i = 0; i < mesh.points.Length; i++) {
                float light = 1.0f;
                if (IsInShadow(mesh.points[i]))
                    light = 0.1f;
                vertexColors[i] = new Vector4(light, light, light, 1.0f);
            }
        }

        public bool IsInShadow(Vector3 point) {
            /* check if it's in the shadow cones */
            bool inVolume = false;
            foreach (var vol in shadowVolumes) {
                if (vol.contains(point)) {
                    inVolume = true;
                    break;
                }
            }
            return inVolume;
        }

        public override void Render() {
            /* render normally */
            base.Render();

            DebugRenderShadowVolume();
            DebugRenderEdges();
        }

        private void DebugRenderShadowVolume() {
            /* no upward shadows */
            if (Light.Y <= 0)
                return;
            var minY = mesh.points.Select(point => point.Y).Min() + Position.Y;

            /* no shading, just a translucent shadow volume */
            GL.Disable(EnableCap.Lighting);
            /* draw ground plane */
            //GL.Color4(0f, 0f, 0.2f, 1f);
            //GL.Begin(BeginMode.Quads);
            //var r = 100f;
            //GL.Vertex3(r,  y, r);
            //GL.Vertex3(-r, y, r);
            //GL.Vertex3(-r, y, -r);
            //GL.Vertex3(r,  y, -r);
            //GL.End();

            /* draw shadow volume */
            GL.Color4(0f, 0f, 1f, 0.4f);
            foreach (var volume in shadowVolumes) {
                GL.Begin(BeginMode.QuadStrip);
                foreach (var point in volume.silhouettePoints) {
                    var p = mesh.points[point];
                    GL.Vertex3(p);
                    GL.Vertex3(p - new Vector3(Light.X, Light.Y, Light.Z) * ((p.Y - minY) / Light.Y));
                }
                GL.End();
            }

            GL.Enable(EnableCap.Lighting);
        }

        private void DebugRenderEdges() {
            GL.Disable(EnableCap.Lighting);
            GL.Color3(1.0f, 0, 0);
            GL.Begin(BeginMode.Lines);
            foreach (var edge in silhouetteEdges) {
                GL.Vertex3(mesh.points[edge.First]);
                GL.Vertex3(mesh.points[edge.Second]);
            }
            GL.End();
            GL.Enable(EnableCap.Lighting);
        }
    }
}
