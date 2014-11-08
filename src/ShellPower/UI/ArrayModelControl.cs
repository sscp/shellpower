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
        int uniformX0, uniformX1, uniformZ0, uniformZ1;
        int uniformSolarCells, uniformSunDirection;
        int shaderProg, texArray;

        /* public properties */
        ShadowMeshSprite sprite;
        public ShadowMeshSprite Sprite {
            get {
                return sprite;
            }
            set {
                if (value != null) {
                    double arrayMaxDim = (value.BoundingBox.Max - value.BoundingBox.Min).Length;
                    zoom = arrayMaxDim * 4;
                }
                sprite = value;
            }
        }
        public ArraySpec Array { get; set; }

        public ArrayModelControl() : base(new OpenTK.Graphics.GraphicsMode(32, 24, 0, 4)) {
            VSync = true;

            KeyDown += new KeyEventHandler(Game_KeyPress);
            MouseMove += new MouseEventHandler(Mouse_Move);
            MouseDown += new MouseEventHandler(Mouse_ButtonDown);
            MouseUp += new MouseEventHandler(Mouse_ButtonUp);
            MouseWheel += new MouseEventHandler(Mouse_WheelChanged);

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 40;
            timer.Enabled = true;
            timer.Tick += new EventHandler((obj, args) => Refresh());
        }

        private void InitGL() {
            GL.Enable(EnableCap.DepthTest);
            
            GL.Enable(EnableCap.Texture2D);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            
            GL.Enable(EnableCap.ColorMaterial);
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Diffuse);

            //GL.Enable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Light0);
            //GL.Light(LightName.Light0, LightParameter.Diffuse, new OpenTK.Graphics.Color4(255, 255, 255, 255));
            //GL.Light(LightName.Light0, LightParameter.Specular, new OpenTK.Graphics.Color4(255, 255, 255, 255));
            //GL.Light(LightName.Light0, LightParameter.Ambient, new OpenTK.Graphics.Color4(0, 0, 0, 255));
        }

        private void InitGLShaders() {
            Debug.WriteLine("compiling shaders");
            int shaderFrag = GL.CreateShader(ShaderType.FragmentShader);
            int shaderVert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(shaderVert, @"
uniform float x0, x1, z0, z1;
uniform vec3 sunDirection;
varying float cosRule;
void main()
{
    vec4 mv = gl_ModelViewMatrix * gl_Vertex;
    gl_Position = gl_ProjectionMatrix * mv;
    // vec3 normal = gl_NormalMatrix * gl_Normal;
    cosRule = dot(gl_Normal, sunDirection);
    gl_TexCoord[0] = vec4((gl_Vertex.x - x0) / (x1 - x0), (gl_Vertex.z - z0) / (z1 - z0), 0,0);
}");
            GL.ShaderSource(shaderFrag, @"
varying float cosRule;
uniform sampler2D solarCells;
void main()
{
    vec4 solarCell = texture2D(solarCells, gl_TexCoord[0].xy);
    float watts = cosRule;
    if(solarCell.x == solarCell.y && solarCell.y == solarCell.z){
        gl_FragData[0] = vec4(watts, watts, watts, 1.0);
    } else {
        gl_FragData[0] = vec4(solarCell.xyz, 1.0);
    }
}");

            GL.CompileShader(shaderVert);
            Debug.WriteLine("info (vert shader): " + GL.GetShaderInfoLog(shaderVert));
            GL.CompileShader(shaderFrag);
            Debug.WriteLine("info (frag shader): " + GL.GetShaderInfoLog(shaderFrag));
            shaderProg = GL.CreateProgram();
            GL.AttachShader(shaderProg, shaderVert);
            GL.AttachShader(shaderProg, shaderFrag);
            GL.LinkProgram(shaderProg);
            Debug.WriteLine("shader linked");

            // get uniform locations
            uniformX0 = GL.GetUniformLocation(shaderProg, "x0");
            uniformX1 = GL.GetUniformLocation(shaderProg, "x1");
            uniformZ0 = GL.GetUniformLocation(shaderProg, "z0"); 
            uniformZ1 = GL.GetUniformLocation(shaderProg, "z1");
            uniformSolarCells = GL.GetUniformLocation(shaderProg, "solarCells");
            uniformSunDirection = GL.GetUniformLocation(shaderProg, "sunDirection");
            Debug.Assert(uniformX0 != -1 && uniformX1 != -1 && uniformZ0 != -1 && uniformZ1 != -1);
            Debug.Assert(uniformSolarCells != -1 && uniformSunDirection != -1);
        }

        private void InitGLTextures() {
            texArray = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texArray);
            GLUtils.FastTexSettings();
        }

        private void SetViewport() {
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
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
        }

        private void SetUniforms() {
            GL.UseProgram(shaderProg);
            GL.Uniform1(uniformX0, Array.LayoutBoundsXZ.Left);
            GL.Uniform1(uniformX1, Array.LayoutBoundsXZ.Right);
            GL.Uniform1(uniformZ0, Array.LayoutBoundsXZ.Top);
            GL.Uniform1(uniformZ1, Array.LayoutBoundsXZ.Bottom); 
            GL.Uniform1(uniformSolarCells, (float)TextureUnit.Texture0);
            var sunDir = new Vector3();
            if (Sprite != null && Sprite.Shadow.Light.Length > 0) {
                sunDir = Sprite.Shadow.Light.Xyz;
                sunDir.Normalize();
            }
            GL.Uniform3(uniformSunDirection, sunDir);
        }

        public static readonly Bitmap DEFAULT_TEX = new Bitmap(800, 400);
        private Bitmap tex;
        private void SetTexture() {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texArray);
            if (Array == null || Array.LayoutTexture == tex) {
                return; // already up-to-date
            }
            tex = Array.LayoutTexture;
            Bitmap bmp = (tex == null) ? DEFAULT_TEX : tex;
            GLUtils.LoadTexture(bmp, TextureUnit.Texture0);
        }

        private void Render() {
            // simply stop rendering while the compute render is happening
            if (!Monitor.TryEnter(typeof(GL), 1)) return;
            try {
                DateTime startRender = DateTime.Now;

                /* gl state */
                SetViewport();
                GL.DrawBuffers(1, new DrawBuffersEnum[] { DrawBuffersEnum.FrontLeft });
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
                GL.ClearColor(0, 0, 0.1f, 1.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                /* render array, then shadow */
                if (Sprite != null) {
                    SetModelViewCamera();
                    GLUtils.SetCameraProjectionPerspective(Width, Height);

                    Sprite.PushTransform();

                    GL.UseProgram(shaderProg);
                    SetUniforms();
                    SetTexture();
                    Sprite.RenderMesh();

                    GL.UseProgram(0);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    Sprite.RenderShadowOutline();
                    Sprite.RenderShadowVolume();

                    Sprite.PopTransform();
                }
                SwapBuffers();

                /* render stats */
                framesRendered++;
                int period = Math.Min(1000, framesRendered);
                emaDelay = (DateTime.Now - startRender).TotalSeconds / period + emaDelay * (period - 1) / period;
                startRender = DateTime.Now;
                if (framesRendered % 1000 == 0) {
                    Debug.WriteLine(string.Format("{0:0.00} fps", 1.0 / emaDelay));
                }
            } catch(Exception e){
                Debug.WriteLine("ArrayModelControl render error: "+e);
            } finally {
                Monitor.Exit(typeof(GL));
            }
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
                InitGLShaders();
                InitGLTextures();
                loaded = true;
            }
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
    }
}