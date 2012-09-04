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
        Shadow shadow;

        public ShadowMeshSprite(Shadow shadow) : base(shadow.Mesh) {
            this.shadow = shadow;
        }

        private void ComputeShadowByVertex() {
            VertexColors = new Vector4[Mesh.points.Length];
            for (int i = 0; i < Mesh.points.Length; i++) {
                float light = 1.0f;
                if (shadow.IsInShadow(Mesh.points[i])) {
                    light = 0.1f;
                }
                VertexColors[i] = new Vector4(light, light, light, 1.0f);
            }
        }

        public override void Render() {
            /* render normally */
            GL.Color3(1f, 1, 1);
            GL.Begin(BeginMode.Triangles);
            for (int i = 0; i < Mesh.triangles.Length; i++) {
                var triangle = Mesh.triangles[i];
                Vector4 color = new Vector4(1, 1, 1, 1);
                if (FaceColors != null) {
                    color = FaceColors[i];
                }
                float att = 0.3f;
                Vector4 shadowColor = new Vector4(color.X * att, color.Y*att, color.Z*att, color.W);

                //draw triangle
                GL.Color4(shadow.VertShadows[triangle.vertexA] ? shadowColor : color);
                GL.Normal3(Mesh.normals[triangle.vertexA]);
                GL.Vertex4(new Vector4(Mesh.points[triangle.vertexA], 1));

                GL.Color4(shadow.VertShadows[triangle.vertexB] ? shadowColor : color);
                GL.Normal3(Mesh.normals[triangle.vertexB]);
                GL.Vertex4(new Vector4(Mesh.points[triangle.vertexB], 1));

                GL.Color4(shadow.VertShadows[triangle.vertexC] ? shadowColor : color);
                GL.Normal3(Mesh.normals[triangle.vertexC]);
                GL.Vertex4(new Vector4(Mesh.points[triangle.vertexC], 1));
            }
            GL.End();

            DebugRenderShadowVolume();
            DebugRenderEdges();
        }

        private void DebugRenderShadowVolume() {
            /* no upward shadows */
            if (shadow.Light.Y <= 0) {
                return;
            }
            var minY = Mesh.points.Select(point => point.Y).Min() + Position.Y;

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
            var light = shadow.Light;
            GL.Color4(0f, 0f, 1f, 0.4f);
            foreach (var volume in shadow.ShadowVolumes) {
                GL.Begin(BeginMode.QuadStrip);
                foreach (var point in volume.SilhouettePoints) {
                    var p = Mesh.points[point];
                    GL.Vertex3(p);
                    GL.Vertex3(p - new Vector3(light.X, light.Y, light.Z) * ((p.Y - minY) / light.Y));
                }
                GL.End();
            }

            GL.Enable(EnableCap.Lighting);
        }

        private void DebugRenderEdges() {
            GL.Disable(EnableCap.Lighting);
            GL.Color3(1.0f, 0, 0);
            GL.Begin(BeginMode.Lines);
            foreach (var edge in shadow.SilhouetteEdges) {
                GL.Vertex3(Mesh.points[edge.First]);
                GL.Vertex3(Mesh.points[edge.Second]);
            }
            GL.End();
            GL.Enable(EnableCap.Lighting);
        }
    }
}
