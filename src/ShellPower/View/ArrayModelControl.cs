using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace SSCP.ShellPower {
    class ArrayModelControl : GLControl {
        /* convenience */
        const float pi = (float)Math.PI;

        /* render stats */
        double emaDelay = 1;
        int framesRendered = 0;

        /* input state */
        Point lastMouse;
        bool mouseRotate = false;

        /* view state */
        const double INITIAL_ZOOM = 20;
        double zoom = INITIAL_ZOOM; /* zoom, in meters away from the model */
        Matrix4 rotation = Matrix4.Identity; /* model */

        /* graphics state */
        bool loaded = false;
        public MeshSprite Sprite { get; set; }
        public Vector3 SunDirection { get; set; }


        public ArrayModelControl() : base(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 4)) {
            VSync = true;

            KeyDown += new KeyEventHandler(Game_KeyPress);
            MouseMove += new MouseEventHandler(Mouse_Move);
            MouseDown += new MouseEventHandler(Mouse_ButtonDown);
            MouseUp += new MouseEventHandler(Mouse_ButtonUp);
            MouseWheel += new MouseEventHandler(Mouse_WheelChanged);
        }

        private void InitGL() {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Light(LightName.Light0, LightParameter.Diffuse, new OpenTK.Graphics.Color4(255, 255, 255, 255));
            GL.Light(LightName.Light0, LightParameter.Specular, new OpenTK.Graphics.Color4(255, 255, 255, 255));
            GL.Light(LightName.Light0, LightParameter.Ambient, new OpenTK.Graphics.Color4(0, 0, 0, 255));
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);
        }

        private void SetViewport() {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
        }

        private void Mouse_WheelChanged(object sender, MouseEventArgs e) {
            double sensitivity = 1.0 / 300.0;
            zoom *= Math.Exp(-e.Delta * sensitivity);
            Refresh();
        }
        private void Mouse_ButtonDown(object sender, MouseEventArgs e) {
            lastMouse = e.Location;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                mouseRotate = true;
        }
        private void Mouse_ButtonUp(object sender, MouseEventArgs e) {
            lastMouse = e.Location;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                mouseRotate = false;
        }
        private void Mouse_Move(object sender, MouseEventArgs e) {
            if (mouseRotate) {
                float sensitivity = 1.0f / 100;
                var xdelta = e.X - lastMouse.X;
                var ydelta = e.Y - lastMouse.Y;
                rotation *=
                    Matrix4.CreateRotationY(xdelta * sensitivity) *
                    Matrix4.CreateRotationX(-ydelta * sensitivity);
            }
            lastMouse = e.Location;

            Refresh();
        }
        private void Game_KeyPress(object sender, KeyEventArgs e) {
            float zoomSensitivity = 0.05f;
            float rotateSensitivity = pi / 16;
            if (e.Shift) {
                zoomSensitivity = .5f;
                rotateSensitivity = pi / 2;
            }
            /* WASD to zoom and rotate */
            if (e.KeyCode == Keys.W) {
                zoom *= (1 - zoomSensitivity);
            } else if (e.KeyCode == Keys.A) {
                rotation *= Matrix4.CreateRotationY(-rotateSensitivity);
            } else if (e.KeyCode == Keys.S) {
                zoom /= (1 - zoomSensitivity);
            } else if (e.KeyCode == Keys.D) {
                rotation *= Matrix4.CreateRotationY(rotateSensitivity);
            }
                /* XYZ to view the model from that axis
                 * Shift+XYZ to view from the opposite side */
            if (e.KeyCode == Keys.X) {
                rotation = Matrix4.CreateRotationY(pi / 2 * (e.Shift ? -1 : 1));
            } else if (e.KeyCode == Keys.Y) {
                rotation = Matrix4.CreateRotationX(pi / 2 * (e.Shift ? -1 : 1));
            } else if (e.KeyCode == Keys.Z) {
                rotation = Matrix4.CreateRotationY(pi / 2 * (e.Shift ? 2 : 0));
            } else if (e.KeyCode == Keys.D0) {
                rotation = Matrix4.Identity;
                zoom = INITIAL_ZOOM;
            }
            Refresh();
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            if (!this.DesignMode) {
                InitGL();
                loaded = true;
            }
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            /*if (loaded) {
                SetViewport();
            }*/
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (loaded) {
                Render();
            }
        }

        /// <summary>
        /// Sets up the modelview matrix from the camera's point of view. (For GUI)
        /// </summary>
        private void SetModelViewCamera() {
            var position = -Vector3.UnitZ * (float)zoom;
            Matrix4 modelview = Matrix4.LookAt(position, Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);
            GL.MultMatrix(ref rotation);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(SunDirection, 0));
        }


        private void Render() {
            DateTime startRender = DateTime.Now;

            /* gl state */
            GL.UseProgram(0);
            SetViewport();
            GL.ClearColor(0f, 0f, 0.1f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SetModelViewCamera();
            GLUtils.SetCameraProjectionOrtho(Width, Height);

            /* render obj display list */
            if (Sprite != null) {
                Sprite.PushTransform();
                Sprite.Render();
                Sprite.PopTransform();
            }
            SwapBuffers();

            /* render stats */
            framesRendered++;
            int period = Math.Min(1000, framesRendered);
            emaDelay = (DateTime.Now - startRender).TotalSeconds / period + emaDelay * (period - 1) / period;
            startRender = DateTime.Now;
            if (framesRendered % 100 == 0) {
                Debug.WriteLine(string.Format("{0:0.00} fps", 1.0 / emaDelay));
            }
        }
    }
}