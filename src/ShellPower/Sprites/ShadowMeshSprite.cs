using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;

namespace SSCP.ShellPower {
    /// <summary>
    /// Implements shadow volumes and Carmack's Reverse to add shadows to a mesh.
    /// Also lets you query the shadow volume (to see if the a point is in shadow or not).
    /// Currently only supports one light source.
    /// </summary>
    public class ShadowMeshSprite : MeshSprite {
        public class shadowVolume {
            public List<int> silhouettePoints = new List<int>();
            public float[,] quadMatrices;
            public void calculateQuadMatrices(ShadowMeshSprite sprite) {
                var light = new Vector3(sprite.Light.X, sprite.Light.Y, sprite.Light.Z) / (sprite.Light.W + 0.000001f);
                quadMatrices = new float[silhouettePoints.Count, 6];
                for (int i = 0; i < silhouettePoints.Count; i++) {
                    Vector3 pointA = sprite.Points[i];
                    Vector3 pointB = sprite.Points[(i + 1) % silhouettePoints.Count];
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

        class edge {
            public List<int> triangles = new List<int>();
            public Vector3 normalA, normalB;
            public int pointA, pointB;
            public void addNormal(Vector3 normal) {
                if (normalA == Vector3.Zero)
                    normalA = normal;
                else
                    normalB = normal;
            }
        }

        public Vector4 Light { get; set; }

        List<shadowVolume> shadowVolumes = new List<shadowVolume>();
        Dictionary<Pair<int>, edge> edges = new Dictionary<Pair<int>, edge>();
        List<Pair<int>> debugEdges = new List<Pair<int>>();

        public ShadowMeshSprite(MeshSprite other) {
            Points = other.Points;
            Triangles = other.Triangles;
            Normals = other.Normals;
            Position = other.Position;
            Transform = other.Transform;
        }

        public override void Initialize() {
            base.Initialize();
            JoinMesh();
            ComputeEdges();
            //ComputeShadowVolumes();
        }
        void JoinMesh() {
            Dictionary<Vector3, int> pointIxMap = new Dictionary<Vector3, int>();
            for (int i = 0; i < Points.Length; i++) {
                if (!pointIxMap.ContainsKey(Points[i]))
                    pointIxMap.Add(Points[i], i);
            }
            for (int i = 0; i < Triangles.Length; i++) {
                Triangles[i].VertexA = pointIxMap[Points[Triangles[i].VertexA]];
                Triangles[i].VertexB = pointIxMap[Points[Triangles[i].VertexB]];
                Triangles[i].VertexC = pointIxMap[Points[Triangles[i].VertexC]];
            }
        }

        void ComputeEdges() {
            var start = DateTime.Now;

            /* compute map of edges -> adjacent triangles */
            edges.Clear();
            var e = new edge();
            for (int i = 0; i < Triangles.Length; i++) {
                var triangle = Triangles[i];
                var ab = new Pair<int>(Math.Min(triangle.VertexA, triangle.VertexB), Math.Max(triangle.VertexA, triangle.VertexB));
                var ac = new Pair<int>(Math.Min(triangle.VertexA, triangle.VertexC), Math.Max(triangle.VertexA, triangle.VertexC));
                var bc = new Pair<int>(Math.Min(triangle.VertexB, triangle.VertexC), Math.Max(triangle.VertexB, triangle.VertexC));
                if (!edges.ContainsKey(ab))
                    edges.Add(ab, new edge() { pointA = triangle.VertexA, pointB = triangle.VertexB });
                if (!edges.ContainsKey(ac))
                    edges.Add(ac, new edge() { pointA = triangle.VertexA, pointB = triangle.VertexC });
                if (!edges.ContainsKey(bc))
                    edges.Add(bc, new edge() { pointA = triangle.VertexB, pointB = triangle.VertexC });
                edges[ab].triangles.Add(i);
                edges[ac].triangles.Add(i);
                edges[bc].triangles.Add(i);
            }

            /* delete triangles that aren't part of the mesh */
            int[] adjacencies = new int[Triangles.Length];
            foreach (var edge in edges.Values)
                if (edge.triangles.Count >= 2)
                    foreach (var triIx in edge.triangles)
                        adjacencies[triIx]++;
            for (int i = 0; i < Triangles.Length; i++)
                if (adjacencies[i] < 3)
                    Triangles[i].VertexA = -1;

            /* compute normals */
            foreach (var edge in edges.Values) {
                foreach (var triIx in edge.triangles) {
                    var triangle = Triangles[triIx];
                    if (triangle.VertexA < 0)
                        continue;
                    var normal = Vector3.Cross(Points[triangle.VertexB] - Points[triangle.VertexA], Points[triangle.VertexC] - Points[triangle.VertexA]);
                    if (normal.Y < 0)
                        normal = -normal;
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
            var volumes = new HashSet<shadowVolume>();
            var volumeTable = new Dictionary<int, shadowVolume>();
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
                    var vol = new shadowVolume();
                    vol.silhouettePoints.Add(edge.First);
                    vol.silhouettePoints.Add(edge.Second);
                    volumeTable.Add(edge.First, vol);
                    volumeTable.Add(edge.Second, vol);
                }
            }
            //debugEdges = edges.Select(pair => pair.Key).ToList();

            /* finally, now that we have all the shadow volumes, copy to list
             * and process so that we can quickly calc whether a point is in one of them */
            shadowVolumes = new List<shadowVolume>();
            foreach (var volume in volumes) {
                volume.calculateQuadMatrices(this);
                shadowVolumes.Add(volume);
            }

            //ComputeShadowByVertex();
        }

        void ComputeShadowByVertex() {
            VertexColors = new Vector4[Points.Length];
            for (int i = 0; i < Points.Length; i++) {
                float light = 1.0f;
                if (IsInShadow(Points[i]))
                    light = 0.1f;
                VertexColors[i] = new Vector4(light, light, light, 1.0f);
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

            /* TODO: shadow volume cap */
            //GEDANKENEXPERIMENT
            //
            //
        }

        public override void Render() {
            /* render normally */
            base.Render();

            DebugRenderShadowVolume();

            GL.Disable(EnableCap.Lighting);
            GL.Color3(1.0f, 0, 0);
            GL.Begin(BeginMode.Lines);
            foreach (var edge in debugEdges) {
                GL.Vertex3(Points[edge.First]);
                GL.Vertex3(Points[edge.Second]);
            }
            //foreach (var edge in edges.Values)
            //{
            //    var a = edge.normalA;
            //    var b = edge.normalB;
            //    a.Normalize(); a /= 10;
            //    b.Normalize(); b /= 10;
            //    //GL.Color3(255, 0, 0);
            //    GL.Vertex3(Position + Points[edge.pointA]);
            //    GL.Vertex3(Position + Points[edge.pointA] + a);

            //    //GL.Color3(0, 255, 0);
            //    GL.Vertex3(Position + Points[edge.pointB]);
            //    GL.Vertex3(Position + Points[edge.pointB] + b);
            //}
            GL.End();
            GL.Enable(EnableCap.Lighting);

            /* TODO: render carmack's reverse into the stencil buffer */
            /* render shadows using the stencil buffer */
            //GL.Light(LightName.Light0, LightParameter.Diffuse, new Vector4(0, 0, 0, 1));
            //GL.Light(LightName.Light0, LightParameter.Specular, new Vector4(0, 0, 0, 1));
        }
        void DebugRenderShadowVolume() {
            /* no upward shadows */
            if (Light.Y <= 0)
                return;
            var minY = Points.Select(point => point.Y).Min() + Position.Y;

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
                    var p = Points[point];
                    GL.Vertex3(p);
                    GL.Vertex3(p - new Vector3(Light.X, Light.Y, Light.Z) * ((p.Y - minY) / Light.Y));
                }
                GL.End();
            }

            GL.Enable(EnableCap.Lighting);
        }
    }
}
