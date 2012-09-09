// Released to the public domain. Use, modify and relicense at will.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    class ArrayModelControl : GLControl {
        /* convenience */
        const float pi = (float)Math.PI;
        float sin(float theta) {
            return (float)Math.Sin(theta);
        }
        float cos(float theta) {
            return (float)Math.Cos(theta);
        }
        float sqrt(float r) {
            return (float)Math.Sqrt(r);
        }

        /* render stats */
        double emaDelay = 1;
        int framesRendered = 0;

        /* input state */
        Point lastMouse;
        bool mouseRotate = false;

        /* view state TODO: just use projection and modelview matrices */
        static readonly Vector3 startPosition = new Vector3(0, 0, -20);
        Vector3 position = startPosition; /* eye */
        Vector3 direction = new Vector3(0, 0, 1); /* eye */
        Matrix4 rotation = Matrix4.Identity; /* model */

        /* graphics state */
        int list0, nLists;
        bool loaded = false;

        public Vector3 Position {
            get {
                Matrix4 rot = rotation;
                rot.Invert();
                return Vector3.Transform(position, rot);
            }
        }

        public MeshSprite Sprite { get; set; }

        public Vector3 SunDirection { get; set; }

        public Vector3 SunInsolation { get; set; }

        public Vector3 AmbientInsolation { get; set; }

        public ArrayModelControl()
            : base(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 4)) {
            VSync = true;

            KeyDown += new KeyEventHandler(Game_KeyPress);
            MouseMove += new MouseEventHandler(Mouse_Move);
            MouseDown += new MouseEventHandler(Mouse_ButtonDown);
            MouseUp += new MouseEventHandler(Mouse_ButtonUp);
            MouseWheel += new MouseEventHandler(Mouse_WheelChanged);
        }

        private void InitGL() {
            GL.ClearColor(0f, 0f, 0.1f, 0.0f);
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
        private void InitTextures() {
            /* textures */
            Bitmap bmp = new Bitmap(600, 400);
            Graphics g = Graphics.FromImage(bmp);
            g.DrawString("hello world", new Font("Verdana", 20f), Brushes.Black, 20f, 20f);

            //texture = GL.GenTexture();
            //GL.BindTexture(TextureTarget.Texture2D, texture);
            //GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            //BitmapData data = bmp.LockBits(new Rectangle(0, 0, 600, 400), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 600, 400, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.Bitmap, data.Scan0);
            //bmp.UnlockBits(data);
            //TexUtil.InitTexturing();
            //texture = TexUtil.CreateTextureFromBitmap(bmp);
        }
        private void InitDisplayLists() {
            /* display lists */
            nLists = 1;
            list0 = GL.GenLists(nLists);
            GL.NewList(list0, ListMode.Compile);
            Sprite.PushTransform();
            Sprite.Render();
            Sprite.PopTransform();
            GL.EndList();
        }
        private void ResizeGL() {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 6, Width / (float)Height, 1.0f, 640.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        /// <summary>
        /// Undoes the OpenGL stack of transformations
        /// (world *ModelView=> eye *Projection=> normalized device coordinates => screen coords)
        /// to go from screen coords (in pixels) to the corresponding point on the near clipping plane.
        /// </summary>
        public Vector3 ScreenToWorld(int x, int y) {
            Vector3 world = EyeToWorld(NdcToEye(ScreenToNdc(x, y)));
            //Debug.WriteLine("unprojected ({0}, {1}) to ({2:0.00}, {3:0.00}, {4:0.00}).", x, y, world.X, world.Y, world.Z);
            return world;
        }

        /// <summary>
        /// Converts viewport coordinates (in pixels) to
        /// a point in normalized device coords:
        /// (x, y, z) in [-1, 1]^3
        /// z = 0, corresponding to a point halfway between the near and far clipping planes.
        /// </summary>
        public Vector3 ScreenToNdc(int x, int y) {
            Vector4 viewport;
            GL.GetFloat(GetPName.Viewport, out viewport);
            //viewport = (x,y,z,w) = (x, y, width, height)
            return new Vector3(
                ((float)x - viewport.X) * 2f / viewport.Z - 1f,
                1f - ((float)y - viewport.Y) * 2f / viewport.W,
                0);
        }
        public Vector3 NdcToEye(Vector3 ndc) {
            Matrix4 proj;
            GL.GetFloat(GetPName.ProjectionMatrix, out proj);
            proj.Invert();
            Vector4 ndc4 = new Vector4(ndc, 1);
            Vector4 eye4 = Vector4.Transform(ndc4, proj);
            Vector3 eye = new Vector3(eye4) / eye4.W;
            return eye;
        }
        public Vector3 EyeToWorld(Vector3 eye) {
            Matrix4 modelView;
            GL.GetFloat(GetPName.ModelviewMatrix, out modelView);
            modelView.Invert();
            Vector4 eye4 = new Vector4(eye, 1);
            Vector4 world = Vector4.Transform(eye4, modelView);
            return new Vector3(world) / world.W;
        }

        private void Mouse_WheelChanged(object sender, MouseEventArgs e) {
            double sensitivity = 1.0 / 300.0;
            position = position * (float)Math.Exp(-e.Delta * sensitivity);
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
                position *= (1 - zoomSensitivity);
            } else if (e.KeyCode == Keys.A) {
                rotation *= Matrix4.CreateRotationY(-rotateSensitivity);
            } else if (e.KeyCode == Keys.S) {
                position /= (1 - zoomSensitivity);
            } else if (e.KeyCode == Keys.D) {
                rotation *= Matrix4.CreateRotationY(rotateSensitivity);
            }
                /* XYZ to view the model from that axis
                 * Shift+XYZ to view from the opposite side */
              else if (e.KeyCode == Keys.X) {
                rotation = Matrix4.CreateRotationY(pi / 2 * (e.Shift ? -1 : 1));
            } else if (e.KeyCode == Keys.Y) {
                rotation = Matrix4.CreateRotationX(pi / 2 * (e.Shift ? -1 : 1));
            } else if (e.KeyCode == Keys.Z) {
                rotation = Matrix4.CreateRotationY(pi / 2 * (e.Shift ? 2 : 0));
            } else if (e.KeyCode == Keys.D0) {
                rotation = Matrix4.Identity;
                position = startPosition;
            }
            Refresh();
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            if (!this.DesignMode) {
                InitGL();
                InitTextures();
                //InitDisplayLists();

                ResizeGL();
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

            if (loaded)
                ResizeGL();
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (loaded)
                Render();
        }

        private void Render() {
            DateTime startRender = DateTime.Now;

            /* gl state */
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 modelview = Matrix4.LookAt(position, position + direction, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);
            GL.MultMatrix(ref rotation);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(SunDirection, 0));

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

        //void RenderControlPointss()
        //{
        //    GL.Color3(Color.White);
        //    GL.Begin(BeginMode.Points);
        //    foreach (var nurb in nurbs)
        //        foreach (var point in nurb.ControlPoints)
        //            GL.Vertex3(point);
        //    GL.End();
        //}
        //void RenderNurbsWithEvaluators()
        //{
        //    GL.Enable(EnableCap.Map2Vertex3);

        //    foreach (var nurb in nurbs)
        //    {
        //        int un = 4, vn = 4;
        //        GL.MapGrid2(
        //            un * 3, 0, 1,
        //            vn * 3, 0, 1);
        //        float[] controlPoints = new float[un * vn * 3];
        //        for (int i = 0; i < un; i++)
        //        {
        //            for (int j = 0; j < vn; j++)
        //            {
        //                controlPoints[3 * (i * vn + j)] = (float)nurb.ControlPoints[i, j].X - (float)nurb.ControlPoints[0, 0].X;
        //                controlPoints[3 * (i * vn + j) + 1] = (float)nurb.ControlPoints[i, j].Y - (float)nurb.ControlPoints[0, 0].Y;
        //                controlPoints[3 * (i * vn + j) + 2] = (float)nurb.ControlPoints[i, j].Z - (float)nurb.ControlPoints[0, 0].Z;
        //            }
        //        }
        //        float[] uknots = new float[un + nurb.UDegree];
        //        for (int i = 0; i < uknots.Length; i++)
        //            uknots[i] = (float)nurb.UKnots[i + 1] / (float)nurb.UKnots[un + nurb.UDegree];
        //        float[] vknots = new float[vn + nurb.VDegree];
        //        for (int i = 0; i < vknots.Length; i++)
        //            vknots[i] = (float)nurb.VKnots[i + 1] / (float)nurb.VKnots[vn + nurb.VDegree];
        //        OpenTK.Graphics.Glu.NurbsSurface(
        //            OpenTK.Graphics.Glu.NewNurbsRenderer(),
        //            uknots.Length,
        //            uknots,
        //            vknots.Length,
        //            vknots,
        //            3, 3 * un,
        //            controlPoints,
        //            nurb.UDegree,
        //            nurb.VDegree,
        //            OpenTK.Graphics.MapTarget.Map2Vertex3);
        //        GL.EvalMesh2(MeshMode2.Line, 0, nurb.UControlPoints * 3, 0, nurb.VControlPoints * 3);
        //    }


        //    //float[] corners = new float[4 * 4 * 3]
        //    //{
        //    //    -1.5f, -1.5f, 4.0f,
        //    //    -0.5f, -1.5f, 2.0f,
        //    //    0.5f, -1.5f, -1.0f,
        //    //    1.5f, -1.5f, 2.0f,

        //    //    -1.5f, -0.5f, 1.0f,
        //    //    -0.5f, -0.5f, 3.0f,
        //    //    0.5f, -0.5f, 0.0f,
        //    //    1.5f, -0.5f, -1.0f,

        //    //    -1.5f, 0.5f, 4.0f,
        //    //    -0.5f, 0.5f, 0.0f,
        //    //    0.5f, 0.5f, 3.0f,
        //    //    1.5f, 0.5f, 4.0f,

        //    //    -1.5f, 1.5f, -2.0f,
        //    //    -0.5f, 1.5f, -2.0f,
        //    //    0.5f, 1.5f, 0.0f,
        //    //    1.5f, 1.5f, -1.0f
        //    //};
        //    //OpenTK.Graphics.Glu.NurbsSurface(
        //    //    OpenTK.Graphics.Glu.NewNurbsRenderer(),
        //    //    7,
        //    //    new float[] { 0, 0, .2f, .4f, .6f, .8f, 1 },
        //    //    7,
        //    //    new float[] { 0, 0, .2f, .4f, .6f, .8f, 1 },
        //    //    12, 3,
        //    //    corners,
        //    //    3,
        //    //    3,
        //    //    OpenTK.Graphics.MapTarget.Map2Vertex3);

        //    //unsafe
        //    //{
        //    //    fixed (double* ctrlPoint = &nurb.ControlPoints[0, 0].X)
        //    //    {
        //    //        GL.Map2(MapTarget.Map2Vertex3,
        //    //            0, 1, 3, 4,
        //    //            0, 1, 12, 4,
        //    //            corners);

        //    //        GL.Map2(MapTarget.Map2Vertex3,
        //    //            0, 1, 3, nurb.VControlPoints,
        //    //            0, 1, 3 * nurb.VControlPoints, nurb.UControlPoints,
        //    //            ctrlPoint);
        //    //    }
        //    //}
        //}
        //void RenderTestCube()
        //{
        //    GL.Begin(BeginMode.TriangleStrip);
        //    GL.Color3(.5f, .5f, 0f);
        //    GL.Normal3(0, 1, 0);
        //    GL.Vertex3(0, 0, 0);
        //    GL.Color3(.5f, .5f, 0f);
        //    GL.Normal3(0, 1, 0);
        //    GL.Vertex3(3, 0, 0);
        //    GL.Color3(.5f, .5f, 0f);
        //    GL.Normal3(0, 1, 0);
        //    GL.Vertex3(0, 0, 3);
        //    GL.End();

        //    //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Color4.White);
        //    //GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, Color4.Black);

        //    //GL.BindTexture(TextureTarget.Texture2D, texture);
        //    //GL.Color3(Color.Red);
        //    //GL.TexCoord2(1, 1);
        //    //GL.Normal3(1, 0, 0);
        //    //GL.Vertex3(1, 1, 1);
        //    //GL.TexCoord2(1, 400);
        //    //GL.Normal3(1, 0, 0);
        //    //GL.Vertex3(1, 1, -1);
        //    //GL.TexCoord2(400, 400);
        //    //GL.Normal3(1, 0, 0);
        //    //GL.Vertex3(1, -1, -1);
        //    //GL.TexCoord2(400, 1);
        //    //GL.Normal3(1, 0, 0);
        //    //GL.Vertex3(1, -1, 1);

        //    GL.Begin(BeginMode.Quads);
        //    GL.Color3(Color.Green);
        //    GL.Normal3(-1, 0, 0);
        //    GL.Vertex3(-1, -1, -1);
        //    GL.Normal3(-1, 0, 0);
        //    GL.Vertex3(-1, 1, -1);
        //    GL.Normal3(-1, 0, 0);
        //    GL.Vertex3(-1, 1, 1);
        //    GL.Normal3(-1, 0, 0);
        //    GL.Vertex3(-1, -1, 1);

        //    GL.Color3(Color.Yellow);
        //    GL.Normal3(0, 1, 0);
        //    GL.Vertex3(1, 1, 1);
        //    GL.Normal3(0, 1, 0);
        //    GL.Vertex3(-1, 1, 1);
        //    GL.Normal3(0, 1, 0);
        //    GL.Vertex3(-1, 1, -1);
        //    GL.Normal3(0, 1, 0);
        //    GL.Vertex3(1, 1, -1);

        //    GL.Color3(Color.Cyan);
        //    GL.Normal3(0, -1, 0);
        //    GL.Vertex3(-1, -1, -1);
        //    GL.Normal3(0, -1, 0);
        //    GL.Vertex3(1, -1, -1);
        //    GL.Normal3(0, -1, 0);
        //    GL.Vertex3(1, -1, 1);
        //    GL.Normal3(0, -1, 0);
        //    GL.Vertex3(-1, -1, 1);

        //    GL.Color3(Color.Magenta);
        //    GL.Normal3(0, 0, 1);
        //    GL.Vertex3(1, 1, 1);
        //    GL.Normal3(0, 0, 1);
        //    GL.Vertex3(1, -1, 1);
        //    GL.Normal3(0, 0, 1);
        //    GL.Vertex3(-1, -1, 1);
        //    GL.Normal3(0, 0, 1);
        //    GL.Vertex3(-1, 1, 1);

        //    GL.Color3(Color.Blue);
        //    GL.Normal3(0, 0, -1);
        //    GL.Vertex3(-1, -1, -1);
        //    GL.Normal3(0, 0, -1);
        //    GL.Vertex3(-1, 1, -1);
        //    GL.Normal3(0, 0, -1);
        //    GL.Vertex3(1, 1, -1);
        //    GL.Normal3(0, 0, -1);
        //    GL.Vertex3(1, -1, -1);

        //    GL.End();
        //}

        private int nThreads = 1;
        private Semaphore[] sema_start, sema_stop;
        private Thread[] threads;
        private int resolution = 1500;
        private void InitWaveThreads() {
            sema_start = new Semaphore[nThreads];
            sema_stop = new Semaphore[nThreads];

            for (int thread = 0; thread < nThreads; thread++) {
                sema_start[thread] = new Semaphore(0, nThreads);
                sema_stop[thread] = new Semaphore(0, nThreads);

                threads[thread] = new Thread(new ParameterizedThreadStart((Object obj) => {
                    int t = (int)obj;
                    while (true) {
                        sema_start[t].WaitOne();
                        int i_min = (2 * resolution / nThreads) * t;
                        int i_max = (2 * resolution / nThreads) * (t + 1);
                        for (int i = i_min; i < i_max; i++) {
                            for (int j = -resolution; j <= resolution; j++) {
                                float x = (float)i / resolution;
                                float y = (float)j / resolution;
                                float r = sqrt(x * x + y * y) * 12; //disance from origin in x/y plane
                                float z = sin(r) * (20 - r) / 50;
                                GL.Vertex3(x, y, z);
                            }
                        }
                        sema_stop[t].Release();
                    }
                }));
                threads[thread].Start(thread);
            }
        }
        private void RenderWaves() {
            GL.Color3(Color.White);
            GL.PointSize(0.01f);
            GL.Begin(BeginMode.Points);

            for (int thread = 0; thread < nThreads; thread++) {
                sema_start[thread].Release();
            }
            for (int thread = 0; thread < nThreads; thread++) {
                sema_stop[thread].WaitOne();
            }

            GL.End();
        }
        private void RenderDonut() {

            float radius0 = 5.0f;
            float radius1 = 2.0f;
            float warble = radius1 / 5f;

            int sectors0 = 100;
            int sectors1 = 40;
            float twoPi = 2.0f * (float)Math.PI;
            for (int i = 0; i < sectors0; i++) {
                float theta0 = i * twoPi / sectors0;
                float theta0Prime = (i + 1) * twoPi / sectors0;
                GL.Begin(BeginMode.TriangleStrip);

                for (int j = 0; j < sectors1; j++) {
                    float theta1 = j * twoPi / (sectors1 - 1);
                    double r = radius0 + radius1 * Math.Cos(theta1) + warble * Math.Sin(10 * theta0 + theta1);
                    double rPrime = radius0 + radius1 * Math.Cos(theta1) + warble * Math.Sin(10 * theta0Prime + theta1);

                    GL.Color3(.5f + .5f * Math.Cos(theta1), .5f + .5f * Math.Sin(theta1) * Math.Cos(theta0), .5f + .5f * Math.Sin(theta0));
                    //GL.Color3(.5, .5, .5);
                    GL.Normal3(Math.Cos(theta0) * Math.Cos(theta1), Math.Sin(theta1), Math.Sin(theta0) * Math.Cos(theta1));
                    GL.Vertex3(r * Math.Cos(theta0), radius1 * Math.Sin(theta1), r * Math.Sin(theta0));
                    GL.Color3(.5f + .5f * Math.Cos(theta1), .5f + .5f * Math.Sin(theta1) * Math.Cos(theta0Prime), .5f + .5f * Math.Sin(theta0Prime));
                    //GL.Color3(.5, .5, .5);
                    GL.Normal3(Math.Cos(theta0Prime) * Math.Cos(theta1), Math.Sin(theta1), Math.Sin(theta0Prime) * Math.Cos(theta1));
                    GL.Vertex3(rPrime * Math.Cos(theta0Prime), radius1 * Math.Sin(theta1), rPrime * Math.Sin(theta0Prime));

                }
                GL.End();
            }
        }
        //void RenderTorus(float radius0, float radius1, Vector3 center, Vector3 axis, Vector3 normAxis)
        //{
        //    int sectors0 = 100;
        //    int sectors1 = 10;

        //    /* get arbitrary perpendicular in a num stable way */
        //    Vector3 major = new Vector3[]{
        //        Vector3.Cross(Vector3.UnitX, axis),
        //        Vector3.Cross(Vector3.UnitY, axis),
        //        Vector3.Cross(Vector3.UnitZ, axis)
        //    }.ArgMin(v => -v.LengthSquared);
        //    Vector3 minor = Vector3.Cross(axis, major);
        //    major.Normalize();
        //    minor.Normalize();
        //    axis.Normalize();

        //    for (int i = 0; i < sectors0; i++)
        //    {
        //        float theta0 = i * 2 *pi / sectors0;
        //        float theta0Prime = (i + 1) * 2*pi / sectors0;
        //        GL.Begin(BeginMode.TriangleStrip);

        //        for (int j = 0; j < sectors1; j++)
        //        {
        //            float theta1 = j * 2*pi / (sectors1 - 1);
        //            float r = radius0 + radius1 * cos(theta1);

        //            //GL.Color3(.5f, .5f, .5f);
        //            GL.Color3(.5f + .5f * cos(theta1), .5f + .5f * sin(theta1) * cos(theta0), .5f + .5f * sin(theta0));
        //            GL.Normal3(
        //                major * cos(theta0) * cos(theta1)
        //                + minor * sin(theta0) * cos(theta1)
        //                + normAxis * sin(theta1));
        //                //+ normalOffset);
        //            GL.Vertex3(
        //                major * cos(theta0) * r
        //                + minor * sin(theta0) * r
        //                + axis * sin(theta1) * radius1
        //                + center);
        //            GL.Color3(.5f + .5f * cos(theta1), .5f + .5f * sin(theta1) * cos(theta0Prime), .5f + .5f * sin(theta0Prime));
        //            GL.Normal3(
        //                major * cos(theta0Prime) * cos(theta1)
        //                + minor * sin(theta0Prime) * cos(theta1)
        //                + normAxis * sin(theta1));
        //                //+ normalOffset);
        //            GL.Vertex3(
        //                major * cos(theta0Prime) * r
        //                + minor * sin(theta0Prime) * r
        //                + axis * sin(theta1) * radius1
        //                + center);

        //        }
        //        GL.End();
        //    }
        //}
        //void PenroseHelper(float t, float radius, out float theta, out Vector3 offset)
        //{
        //    float sideLength = 10f;
        //    float[] cutoffs = new float[3 * 2 + 1];
        //    float cutoff = 0f;
        //    cutoffs[0] = cutoff;
        //    for (int i = 1; i < cutoffs.Length; )
        //    {
        //        cutoff += (2 * (float)Math.PI / 3) * radius;
        //        cutoffs[i++] = cutoff;
        //        cutoff += sideLength;
        //        cutoffs[i++] = cutoff;
        //    }

        //    t *= cutoffs[cutoffs.Length - 1];
        //    theta = 0f;
        //    offset = new Vector3(
        //        (float)Math.Sqrt(5) / 6,
        //        0,
        //        .5f) * sideLength;
        //    for (int j = 0; j < cutoffs.Length - 1 && cutoffs[j] < t; j++)
        //    {
        //        float delta = Math.Min(t - cutoffs[j], cutoffs[j + 1] - cutoffs[j]);
        //        if (j % 2 == 0)
        //        {
        //            Debug.WriteLine("adding " + delta);
        //            theta += delta / radius;
        //        }
        //        else
        //        {
        //            offset += new Vector3((float)Math.Cos(theta+pi/2), 0, (float)Math.Sin(theta+pi/2)) * delta;
        //        }
        //    }
        //}
        //void RenderSlinky()
        //{
        //    float radius0 = 4,
        //        radius1 = 2,
        //        radius2 = 0.2f;

        //    int straightSteps = 20, turnSteps = 15;
        //    float twoPi = 2.0f * (float)Math.PI;
        //    Vector3 axis = new Vector3(1, 0, 0);
        //    Vector3 normAxis = new Vector3(1, 0, 0);
        //    float spacing = .7f;
        //    Vector3 center = new Vector3(
        //        (float)Math.Sqrt(5) / 6,
        //        0,
        //        .5f) * straightSteps * spacing;

        //    Vector3 up = new Vector3(0, 1, 0);
        //    Matrix3 turnLeft = Matrix3.Slice(Matrix4.CreateFromAxisAngle(up, twoPi / 3f / turnSteps));

        //    for (int j = 0; j < 3; j++)
        //    {
        //        for (int i = 0; i < turnSteps; i++)
        //        {
        //            RenderTorus(radius1, radius2, center, axis, normAxis);
        //            center += axis * spacing;
        //            Matrix3 twistleft = Matrix3.Slice(Matrix4.CreateFromAxisAngle(Vector3.Cross(normAxis, up), twoPi / 4f / turnSteps));
        //            axis = turnLeft * axis;
        //            normAxis = turnLeft * normAxis;
        //        }
        //        for (int i = 0; i < straightSteps; i++)
        //        {
        //            RenderTorus(radius1, radius2, center, axis, normAxis);
        //            center += axis * spacing;
        //        }
        //    }

        //    //for (int i = 0; i < steps; i++)
        //    //{


        //    //    float t = (float)i / sectors0;
        //    //    float tPrime = (float)(i + 1) / sectors0;
        //    //    float theta0, theta0Prime;
        //    //    Vector3 offset, offsetPrime;
        //    //    PenroseHelper(t, radius0, out theta0, out offset);
        //    //    PenroseHelper(tPrime, out theta0Prime, out offsetPrime);

        //    //    RenderTorus(
        //    //        radius1,
        //    //        radius2,
        //    //        offset + radius0 * new Vector3(cos(theta0), 0, sin(theta0)),
        //    //        new Vector3(cos(theta0 + pi / 2), 0, sin(theta0 + pi / 2)),
        //    //        new Vector3(cos(theta0 + pi / 2), 0, sin(theta0 + pi / 2))
        //    //        );
        //    //    for (int j = 0; j < sectors1; j++)
        //    //    {
        //    //        float theta1 = j * twoPi / (sectors1 - 1);
        //    //        float r = length + radius * (float)Math.Cos(theta1);
        //    //        float rPrime = length + radius * (float)Math.Cos(theta1);

        //    //        var n = new Vector3d(Math.Cos(theta0) * Math.Cos(theta1), Math.Sin(theta1), Math.Sin(theta0) * Math.Cos(theta1));
        //    //        n += new Vector3d(Math.Cos(theta0 + pi / 2), 0, Math.Sin(theta0 + pi / 2)) * .5;
        //    //        n.Normalize();


        //    //        GL.Begin(BeginMode.TriangleStrip);
        //    //        for (int k = 0; k < sectors2; k++)
        //    //        {
        //    //            float theta2 = k * 2 * pi / (sectors2 - 1);

        //    //            GL.Normal3(n);
        //    //            GL.Vertex3(
        //    //                new Vector3(r * (float)Math.Cos(theta0), radius * (float)Math.Sin(theta1), r * (float)Math.Sin(theta0))
        //    //                + offset);
        //    //            GL.Color3(.5f, .5f, .5f);
        //    //        }
        //    //        GL.End();

        //    //    }
        //    //}
        //}
    }
}