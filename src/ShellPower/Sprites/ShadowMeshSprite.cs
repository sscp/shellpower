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

        public ShadowMeshSprite(Shadow shadow) {
            this.shadow = shadow;
            mesh = shadow.Mesh;
        }

        private void ComputeShadowByVertex() {
            vertexColors = new Vector4[mesh.points.Length];
            for (int i = 0; i < mesh.points.Length; i++) {
                float light = 1.0f;
                if (shadow.IsInShadow(mesh.points[i])) {
                    light = 0.1f;
                }
                vertexColors[i] = new Vector4(light, light, light, 1.0f);
            }
        }

        public override void Render() {
            /* render normally */
            base.Render();
            DebugRenderShadowVolume();
            DebugRenderEdges();
        }

        private void DebugRenderShadowVolume() {
            /* no upward shadows */
            if (shadow.Light.Y <= 0) {
                return;
            }
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
            var light = shadow.Light;
            GL.Color4(0f, 0f, 1f, 0.4f);
            foreach (var volume in shadow.ShadowVolumes) {
                GL.Begin(BeginMode.QuadStrip);
                foreach (var point in volume.SilhouettePoints) {
                    var p = mesh.points[point];
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
                GL.Vertex3(mesh.points[edge.First]);
                GL.Vertex3(mesh.points[edge.Second]);
            }
            GL.End();
            GL.Enable(EnableCap.Lighting);
        }
    }
}
