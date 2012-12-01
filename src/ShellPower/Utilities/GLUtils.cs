using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    public class GLUtils {
        public static void SetCameraProjectionPerspective(int w, int h) {
            // perspective projection
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 6, w / (float)h, 0.1f, 1000.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }
        public static void SetCameraProjectionOrtho(int w, int h) {
            // orthographic projection
            float minWidth = 6f, minHeight = 4f; //meters
            float scale = Math.Max(minWidth / w, minHeight / h);
            float volWidth = scale * w, volHeight = scale * h;
            float zNear = 0.1f, zFar = 100.0f;
            Matrix4 projection = Matrix4.CreateOrthographic(volWidth, volHeight, zNear, zFar);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }
        public static void FastTexSettings() {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
        }

        /// <summary>
        /// Loads the given image as a 32bpp RGBA texture.
        /// </summary>
        public static void LoadTexture(Bitmap bmpTex, TextureUnit slot) {
            Debug.WriteLine("loading array texture");
            BitmapData bmpDataTex = bmpTex.LockBits(
                new Rectangle(0, 0, bmpTex.Width, bmpTex.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Debug.WriteLine("loaded " + bmpTex.Width + "x" + bmpTex.Height + " tex, binding");

            // set it as texture 0
            GL.ActiveTexture(slot);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bmpTex.Width, bmpTex.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte,
                bmpDataTex.Scan0);

            // clean up
            bmpTex.UnlockBits(bmpDataTex);
            Debug.WriteLine("array texture loaded");
        }
    }
}
