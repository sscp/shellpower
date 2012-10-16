// Released to the public domain. Use, modify and relicense at will.

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

        /* HACK: opengl for computation */
        int shaderVert, shaderFrag, shaderProg;
        int uniformPixelArea, uniformSolarCells;
        int texArray;

        int shaderFragWatts, shaderProgWatts;
        int texWatts, texCells, texWattsWidth, texWattsHeight;
        int fboWatts;

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

        /// <summary>
        /// Shaders to compute array properties
        /// </summary>
        private void InitGLArrayComputeShaders() {
            Debug.WriteLine("compiling shaders");
            shaderFrag = GL.CreateShader(ShaderType.FragmentShader);
            shaderVert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(shaderVert, @"
varying float cosRule;
void main()
{
    vec4 mv = gl_ModelViewMatrix * gl_Vertex;
    gl_Position = gl_ProjectionMatrix * mv;
    vec3 normal = gl_NormalMatrix * gl_Normal;
    cosRule = dot(normal, vec3(0,0,1));

    gl_TexCoord[0] = vec4(-gl_Vertex.x / 4.0 + 0.5, gl_Vertex.z / 4.0 + 0.5, 0,0);
}");
            GL.ShaderSource(shaderFrag, @"
uniform float pixelArea;
varying float cosRule;
uniform sampler2D solarCells;

void main()
{
    vec4 solarCell = texture2D(solarCells, gl_TexCoord[0].xy);
    float watts = pixelArea*cosRule;
    gl_FragData[0] = vec4(solarCell.xyz, 1.0);
    gl_FragData[1] = vec4(watts, 0, 0, 1.0);
}");

            GL.CompileShader(shaderVert);
            Debug.WriteLine("info (vert shader): " + GL.GetShaderInfoLog(shaderVert));
            GL.CompileShader(shaderFrag);
            Debug.WriteLine("info (frag shader): " + GL.GetShaderInfoLog(shaderFrag));
            shaderProg = GL.CreateProgram();
            GL.AttachShader(shaderProg, shaderVert);
            GL.AttachShader(shaderProg, shaderFrag);
            GL.LinkProgram(shaderProg);
            GL.UseProgram(shaderProg);
            Debug.WriteLine("shader attached.");

            uniformPixelArea = GL.GetUniformLocation(shaderProg, "pixelArea");
            GL.Uniform1(uniformPixelArea, 1.0f);
            uniformSolarCells = GL.GetUniformLocation(shaderProg, "solarCells");
            GL.Uniform1(uniformSolarCells, (float)TextureUnit.Texture0);
            Debug.WriteLine("uniforms set.");
        }

        /// <summary>
        /// Loads the solar array as a texture.
        /// 
        /// Assumes a top-down projection, ie the texture UVs are the
        /// vertex X and Z coords with a scale factor and an offset.
        /// </summary>
        private void InitGLArrayTextures() {
            String fnameTex = "../../../../arrays/texture.png";
            Debug.WriteLine("loading texture " + fnameTex);
            Bitmap bmpTex = new Bitmap(fnameTex);
            BitmapData bmpDataTex = bmpTex.LockBits(
                new Rectangle(0, 0, bmpTex.Width, bmpTex.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Debug.WriteLine("loaded " + bmpTex.Width + "x" + bmpTex.Height + " tex, binding");
            texArray = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texArray);
            /*GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 
                (float)TextureEnvMode.Modulate);*/
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (float)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (float)TextureMinFilter.Nearest);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bmpTex.Width, bmpTex.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte,
                bmpDataTex.Scan0);
            bmpTex.UnlockBits(bmpDataTex);

            Debug.WriteLine("textures ready.");
        }

        private void InitGLComputeBuffer() {
            texWattsWidth = texWattsHeight = 2048;

            // one buffer for insolation in W...
            texWatts = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texWatts);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 
                texWattsWidth, texWattsHeight, 0, 
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            // another that uses color to ID each cell, string, and panel
            // NB: you must use PixelInternalFormat.Rgb8 or Rgba8, 
            // PixelFormat.Rgb crashes with a cryptic error
            texCells = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texCells);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                texWattsWidth, texWattsHeight, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // depth buffer
            int texDepth = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texDepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32,
                texWattsWidth, texWattsHeight, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);

            //fbo
            GL.Ext.GenFramebuffers(1, out fboWatts);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboWatts);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, texCells, 0);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment1Ext, TextureTarget.Texture2D, texWatts, 0);
            //GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBufWatts);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texDepth, 0);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
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

        private void SetViewport() {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
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
                InitGLArrayComputeShaders();
                InitGLArrayTextures();
                InitGLComputeBuffer();
                //InitDisplayLists();
                loaded = true;

                ComputeRender();
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

        private void SetModelView() {
            Matrix4 modelview = Matrix4.LookAt(position, position + direction, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);
            GL.MultMatrix(ref rotation);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(SunDirection, 0));
        }

        private void SetCameraProjection(int w, int h) {
            // perspective projection
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 6, Width / (float)Height, 1.0f, 640.0f);

            // orthographic projection
            float minWidth = 6f, minHeight = 4f; //meters
            float scale = Math.Max(minWidth / w, minHeight / h);
            float volWidth = scale*w, volHeight = scale*h;
            float zNear = 0.1f, zFar = 100.0f;
            Matrix4 projection = Matrix4.CreateOrthographic(volWidth, volHeight, zNear, zFar);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        private void ComputeRender() {
            Debug.WriteLine("rendering insolation+cells into a " 
                + texWattsWidth + "x" + texWattsWidth + " fbo");

            /* gl state */
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboWatts);
            GL.DrawBuffers(2, new DrawBuffersEnum[]{
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment0Ext,
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment1Ext});
            GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters

            GL.BindTexture(TextureTarget.Texture2D, texArray);
            GL.Viewport(0, 0, texWattsWidth, texWattsHeight);
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SetModelView();
            SetCameraProjection(texWattsWidth, texWattsHeight);

            /* render obj display list */
            if (Sprite != null) {
                Sprite.PushTransform();
                Sprite.Render();
                Sprite.PopTransform();
            }
            DebugSaveBuffers();

            //render
            GL.PopAttrib();
            // write to back buffer as normal
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.DrawBuffer(DrawBufferMode.Back);
        }

        private void DebugSaveBuffers() {
            Debug.WriteLine("saving the ext fbo buffers");
            DebugSaveBuffer(FramebufferAttachment.ColorAttachment0, "../../../../test0.png");
            DebugSaveBuffer(FramebufferAttachment.ColorAttachment1, "../../../../test1.png");
        }

        private void DebugSaveBuffer(FramebufferAttachment buf, String fname){
            GL.ReadBuffer((ReadBufferMode)buf);
            Bitmap bmp = new Bitmap(texWattsWidth, texWattsHeight, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, texWattsWidth, texWattsHeight),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.ReadPixels(0, 0, texWattsWidth, texWattsHeight, 
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bmpData.Scan0);
            bmp.UnlockBits(bmpData);

            Debug.WriteLine("writing " + fname);
            bmp.Save(fname);
        }

        private void Render() {
            DateTime startRender = DateTime.Now;

            /* gl state */
            GL.BindTexture(TextureTarget.Texture2D, texArray);
            SetViewport();
            GL.ClearColor(0f, 0f, 0.1f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SetModelView();
            SetCameraProjection(Width, Height);

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