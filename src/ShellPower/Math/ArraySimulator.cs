using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    public class ArraySimulator {

        /* opengl for computation program and args */
        private int shaderVert, shaderFrag, shaderProg;
        private int uniformPixelArea, uniformSolarCells;
        private int texArray;

        /* ... program outputs to a texture */
        private int texWatts, texCells, texWattsWidth, texWattsHeight;
        private int fboWatts;

        /* model */
        private readonly ArraySpec spec;
        public ArraySimulator(ArraySpec spec) {
            this.spec = spec;

            InitGLArrayComputeShaders();
            InitGLComputeBuffer();
        }
        public ArraySpec Array {
            get {
                return spec;
            }
        }

        /// <summary>
        /// (TODO) Calculates an array simulation
        /// for a single array, single set of parameters, single moment in time.
        /// 
        /// Returns array output in watts, along with some other data.
        /// </summary>
        public ArraySimulationStepOutput Simulate(ArraySimulationStepInput simInput) {
            //TODO: clean up mainform. this should be set with ArraySpec, which should be immutable
            InitGLArrayTextures();
            ComputeRender(simInput);
            return AnalyzeComputeTex();
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

        private void InitGLComputeBuffer() {
            texWattsWidth = texWattsHeight = 2048;

            // one buffer for insolation in W...
            texWatts = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texWatts);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                texWattsWidth, texWattsHeight, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            // another that uses color to ID each cell, string, and panel
            // NB: you must use PixelInternalFormat.Rgb8 or Rgba8, 
            // PixelFormat.Rgb crashes with a cryptic error.
            // also, it seems two attachments in the same FBO must have the same
            // pixel format, else you get the same cryptic error
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
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, texDepth, 0);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
        }

        /// <summary>
        /// Loads the solar array as a texture.
        /// 
        /// Assumes a top-down projection, ie the texture UVs are the
        /// vertex X and Z coords with a scale factor and an offset.
        /// </summary>
        private void InitGLArrayTextures() {
            texArray = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texArray);
            /*GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 
                (float)TextureEnvMode.Modulate);*/
            GLUtils.FastTexSettings();
            LoadGLArrayTexture();
        }

        public void ComputeRender(ArraySimulationStepInput simInput) {
            Debug.WriteLine("rendering insolation+cells into a "
                + texWattsWidth + "x" + texWattsWidth + " fbo");

            /* gl state */
            GL.UseProgram(shaderProg);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboWatts);
            GL.DrawBuffers(2, new DrawBuffersEnum[]{
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment0Ext,
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment1Ext});
            GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters

            GL.BindTexture(TextureTarget.Texture2D, texArray);
            GL.Viewport(0, 0, texWattsWidth, texWattsHeight);
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SetModelViewSun(GetSunDir(simInput));
            GLUtils.SetCameraProjectionOrtho(texWattsWidth, texWattsHeight);

            MeshSprite sprite = new MeshSprite(spec.Mesh);
            sprite.PushTransform();
            sprite.Render();
            sprite.PopTransform();
            DebugSaveBuffers();

            //render
            GL.PopAttrib();
            // write to back buffer as normal
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.DrawBuffer(DrawBufferMode.Back);
        }

        private Vector3 GetSunDir(ArraySimulationStepInput simInput) {
            // update the astronomy model
            var utc_time = simInput.LocalTime - new TimeSpan((long)(simInput.Timezone * 3600.0) * 10000000);
            var sidereal = Astro.sidereal_time(
                utc_time,
                simInput.Longitude);
            var azimuth = Astro.solar_azimuth(
                (int)sidereal.TimeOfDay.TotalSeconds,
                simInput.LocalTime.DayOfYear,
                simInput.Latitude)
                - (float)simInput.Heading;
            var elevation = Astro.solar_elevation(
                (int)sidereal.TimeOfDay.TotalSeconds,
                simInput.LocalTime.DayOfYear,
                simInput.Latitude);
            Logger.info("sim step\n\t" +
                "lat {0:0.0} lon {1:0.0} heading {2:0.0}\n\t" +
                "azith {3:0.0} elev {4:0.0} utc {5} sidereal {6}",
                simInput.Latitude,
                simInput.Longitude,
                Astro.rad2deg(simInput.Heading),
                Astro.rad2deg(azimuth),
                Astro.rad2deg(elevation),
                utc_time,
                sidereal);

            var lightDir = new Vector3(
                (float)(-Math.Cos(elevation) * Math.Cos(azimuth)), (float)(Math.Sin(elevation)),
                (float)(-Math.Cos(elevation) * Math.Sin(azimuth)));
            return lightDir;
        }


        /// <summary>
        /// Sets up the modelview matrix from the sun's point of view (for computation)
        /// </summary>
        private void SetModelViewSun(Vector3 sunDir) {
            Matrix4 modelview = Matrix4.LookAt(sunDir * 50f, Vector3.Zero, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(sunDir, 0));
        }

        private void DebugSaveBuffers() {
            Debug.WriteLine("saving the ext fbo buffers");
            DebugSaveBuffer(FramebufferAttachment.ColorAttachment0, "../../../../test0.png");
            DebugSaveBuffer(FramebufferAttachment.ColorAttachment1, "../../../../test1.png");
        }

        private void DebugSaveBuffer(FramebufferAttachment buf, String fname) {
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


        /// <summary>
        /// Reads the compute textures from OpenGL.
        /// This gives insolation for each cell.
        /// 
        /// Uses this to calculate IV curves, etc, and ultimately array power.
        /// </summary>
        private ArraySimulationStepOutput AnalyzeComputeTex() {
            ArraySimulationStepOutput output = new ArraySimulationStepOutput();
            output.ArrayArea = 10;
            output.ArrayLitArea = 9;
            output.WattsInsolation = 6500;
            output.WattsOutput = 1000;
            return output;
        }

        /// <summary>
        /// Reads an image from the given file.
        /// Sets it as Texture 0. See InitGLArrayTexture() for the Texture 0 params.
        /// </summary>
        private void LoadGLArrayTexture() {
            // read the image
            Debug.WriteLine("loading array texture");
            var bmpTex = Array.LayoutTexture;
            BitmapData bmpDataTex = bmpTex.LockBits(
                new Rectangle(0, 0, bmpTex.Width, bmpTex.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Debug.WriteLine("loaded " + bmpTex.Width + "x" + bmpTex.Height + " tex, binding");

            // set it as texture 0
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bmpTex.Width, bmpTex.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte,
                bmpDataTex.Scan0);

            // clean up
            bmpTex.UnlockBits(bmpDataTex);
            Debug.WriteLine("array texture ready");
        }


    }
}