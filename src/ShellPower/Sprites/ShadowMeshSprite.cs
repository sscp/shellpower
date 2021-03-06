﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    /// <summary>
    /// Renders shadow volumes.
    /// </summary>
    public class ShadowMeshSprite : MeshSprite {
        Shadow shadow;
        public Shadow Shadow {
            get { return shadow; }
        }

        public ShadowMeshSprite(Shadow shadow) : base(shadow.Mesh) {
            this.shadow = shadow;
        }

        public override void Render() {
            RenderMesh();
            RenderShadowVolume();
            RenderShadowOutline();
        }

        public void RenderMesh() {
            /* render normally */
            GL.Color3(1f, 1, 1);
            GL.Begin(BeginMode.Triangles);
            for (int i = 0; i < Mesh.triangles.Length; i++) {
                var triangle = Mesh.triangles[i];
                Vector4 color = new Vector4(1, 1, 1, 1);
                if (FaceColors != null) {
                    color = FaceColors[i];
                }
                Vector4 shadowColor = new Vector4(0.8f, 0, 0, 1);

                //draw triangle
                GL.Normal3(Mesh.normals[triangle.vertexA]);
                GL.Vertex4(new Vector4(Mesh.points[triangle.vertexA], 1));

                GL.Normal3(Mesh.normals[triangle.vertexB]);
                GL.Vertex4(new Vector4(Mesh.points[triangle.vertexB], 1));

                GL.Normal3(Mesh.normals[triangle.vertexC]);
                GL.Vertex4(new Vector4(Mesh.points[triangle.vertexC], 1));
            }
            GL.End();
        }

        public void RenderShadowVolume() {
            /* no upward shadows */
            if (shadow.Light.Y <= 0) {
                return;
            }
            /* floor Y coordinate */
            var minY = Mesh.points.Select(point => point.Y).Min() + Position.Y;

            /* no shading, just a translucent shadow volume */
            GL.Disable(EnableCap.Lighting);
            /* draw shadow volume */
            var light = shadow.Light;
            GL.Color4(0f, 0f, 1f, 0.4f);
            foreach (Pair<int> edge in shadow.SilhouetteEdges){
                GL.Begin(BeginMode.TriangleStrip);
                Vector3[] points = {
                    Mesh.points[edge.First],
                    Mesh.points[edge.Second],
                };
                foreach (var p in points) {
                    GL.Vertex3(p);
                    GL.Vertex3(p - new Vector3(light.X, light.Y, light.Z) * ((p.Y - minY) / light.Y));
                }
                GL.End();
            }
            GL.Enable(EnableCap.Lighting);
        }

        public void RenderShadowOutline() {
            if (shadow.Light.Y <= 0) {
                return;
            }
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
