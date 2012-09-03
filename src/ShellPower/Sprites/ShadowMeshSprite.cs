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
        Dictionary<Pair<int>, Edge> edges = new Dictionary<Pair<int>, Edge>();
        List<Pair<int>> debugEdges = new List<Pair<int>>();

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
            edges.Clear();
            var e = new Edge();
            for (int i = 0; i < mesh.triangles.Length; i++) {
                var triangle = mesh.triangles[i];
                var ab = new Pair<int>(Math.Min(triangle.vertexA, triangle.vertexB), 
                    Math.Max(triangle.vertexA, triangle.vertexB));
                var ac = new Pair<int>(Math.Min(triangle.vertexA, triangle.vertexC), 
                    Math.Max(triangle.vertexA, triangle.vertexC));
                var bc = new Pair<int>(Math.Min(triangle.vertexB, triangle.vertexC), 
                    Math.Max(triangle.vertexB, triangle.vertexC));
                if (!edges.ContainsKey(ab)){
                    edges.Add(ab, new Edge() { pointA = triangle.vertexA, pointB = triangle.vertexB });
                }
                if (!edges.ContainsKey(ac)){
                    edges.Add(ac, new Edge() { pointA = triangle.vertexA, pointB = triangle.vertexC });
                }
                if (!edges.ContainsKey(bc)){
                    edges.Add(bc, new Edge() { pointA = triangle.vertexB, pointB = triangle.vertexC });
                }
                edges[ab].triangles.Add(i);
                edges[ac].triangles.Add(i);
                edges[bc].triangles.Add(i);
            }

            /* delete triangles that aren't part of the mesh */
            int[] adjacencies = new int[mesh.triangles.Length];
            foreach (var edge in edges.Values) {
                if (edge.triangles.Count >= 2) {
                    foreach (var triIx in edge.triangles) {
                        adjacencies[triIx]++;
                    }
                }
            }
            List<Mesh.Triangle> validTris = new List<Mesh.Triangle>();
            for (int i = 0; i < mesh.triangles.Length; i++) {
                if (adjacencies[i] >= 3) {
                    validTris.Add(mesh.triangles[i]);
                }
            }
            mesh.triangles = validTris.ToArray();

            /* compute normals */
            foreach (var edge in edges.Values) {
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
            debugEdges.Clear();

            /* create silhouettes and keep joining them until each one is a closed path */
            var volumes = new HashSet<ShadowVolume>();
            var volumeTable = new Dictionary<int, ShadowVolume>();
            foreach (var edge in edges
                .Where((pair) => {
                    float xy =
                        Vector3.Dot(pair.Value.normalA, new Vector3(Light.X, Light.Y, Light.Z)) *
                        Vector3.Dot(pair.Value.normalB, new Vector3(Light.X, Light.Y, Light.Z));
                    return xy < 0;
                }).Select(pair => pair.Key)) {
                debugEdges.Add(edge);
                if (volumeTable.ContainsKey(edge.First)) {
                    if (volumeTable.ContainsKey(edge.Second)) {
                        var vol1 = volumeTable[edge.First];
                        var vol2 = volumeTable[edge.Second];
                        //Debug.Assert(vol1 != vol2);
                        if (vol1 == vol2)
                            continue;
                        if (vol1.silhouettePoints.IndexOf(edge.First) > vol1.silhouettePoints.Count / 2) {
                            vol1.silhouettePoints.Reverse();
                            //Debug.Assert(vol1.silhouettePoints[0] == edge.First);
                            while (vol1.silhouettePoints[0] != edge.First) {
                                volumeTable.Remove(vol1.silhouettePoints[0]);
                                vol1.silhouettePoints.RemoveAt(0);
                            }
                        }
                        if (vol2.silhouettePoints.IndexOf(edge.Second) <= vol2.silhouettePoints.Count / 2) {
                            //Debug.Assert(vol2.silhouettePoints[0] == edge.Second);
                            while (vol2.silhouettePoints[0] != edge.Second) {
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
                        var pointIx = edge.First;
                        var vol = volumeTable[pointIx];
                        if (vol.silhouettePoints[0] == pointIx ||
                            vol.silhouettePoints[vol.silhouettePoints.Count - 1] == pointIx) {
                            if (vol.silhouettePoints[0] == pointIx)
                                vol.silhouettePoints.Reverse();
                            vol.silhouettePoints.Add(edge.Second);
                            volumeTable.Add(edge.Second, vol);
                        }
                    }
                } else if (volumeTable.ContainsKey(edge.Second)) {
                    var pointIx = edge.Second;
                    var vol = volumeTable[pointIx];
                    if (vol.silhouettePoints[0] == pointIx ||
                        vol.silhouettePoints[vol.silhouettePoints.Count - 1] == pointIx) {
                        if (vol.silhouettePoints[0] == pointIx)
                            vol.silhouettePoints.Reverse();
                        vol.silhouettePoints.Add(edge.First);
                        volumeTable.Add(edge.First, vol);
                    }
                } else {
                    var vol = new ShadowVolume(mesh, Light);
                    vol.silhouettePoints.Add(edge.First);
                    vol.silhouettePoints.Add(edge.Second);
                    volumeTable.Add(edge.First, vol);
                    volumeTable.Add(edge.Second, vol);
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
            foreach (var edge in debugEdges) {
                GL.Vertex3(mesh.points[edge.First]);
                GL.Vertex3(mesh.points[edge.Second]);
            }
            GL.End();
            GL.Enable(EnableCap.Lighting);
        }
    }
}
