using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    public class ArraySimulator {

        /* opengl for computation program and args */
        private int shaderVert, shaderFrag, shaderProg;
        private int uniformPixelArea, uniformSolarCells;
        private int uniformX0, uniformX1, uniformZ0, uniformZ1;
        private int texArray;

        /* ... program outputs to a texture */
        private int texWatts, texCells, computeWidth, computeHeight;
        private int fboWatts;

        /* model */
        private readonly ArraySpec spec;
        public ArraySimulator(ArraySpec spec) {
            this.spec = spec;

            InitGLArrayComputeShaders();
            InitGLComputeBuffer();
            InitGLArrayTextures();
        }
        public ArraySpec Array {
            get {
                return spec;
            }
        }



        /// <summary>
        /// Shaders to compute array properties
        /// </summary>
        private void InitGLArrayComputeShaders() {
            Debug.WriteLine("compiling shaders");
            shaderFrag = GL.CreateShader(ShaderType.FragmentShader);
            shaderVert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(shaderVert, @"
uniform float x0, x1, z0, z1;
varying float cosRule;
void main()
{
    vec4 mv = gl_ModelViewMatrix * gl_Vertex;
    gl_Position = gl_ProjectionMatrix * mv;
    vec3 normal = gl_NormalMatrix * gl_Normal;
    cosRule = dot(normal, vec3(0,0,1));

    gl_TexCoord[0] = vec4((gl_Vertex.x - x0) / (x1 - x0), (gl_Vertex.z - z0) / (z1 - z0), 0,0);
}");
            GL.ShaderSource(shaderFrag, @"
uniform float pixelArea;
varying float cosRule;
uniform sampler2D solarCells;

void main()
{
    vec4 solarCell = texture2D(solarCells, gl_TexCoord[0].xy);
    float watts = pixelArea*cosRule;
    gl_FragData[0] = vec4(solarCell.xyz, 1.0); // cell id
    gl_FragData[1] = vec4(watts, 0, 0, 1.0);   // watts insolation
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
            uniformPixelArea = GL.GetUniformLocation(shaderProg, "pixelArea");
            uniformSolarCells = GL.GetUniformLocation(shaderProg, "solarCells");
            Debug.Assert(uniformX0 >= 0 && uniformX1 >= 0 && uniformZ0 >= 0 && uniformZ1 >= 0
                && uniformPixelArea >= 0 && uniformSolarCells >= 0);
        }

        /// <summary>
        /// Creates the output buffers.
        /// </summary>
        private void InitGLComputeBuffer() {
            computeWidth = computeHeight = 2048;

            // one buffer for insolation in W...
            texWatts = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texWatts);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                computeWidth, computeHeight, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            // another that uses color to ID each cell, string, and panel
            // NB: you must use PixelInternalFormat.Rgb8 or Rgba8, 
            // PixelFormat.Rgb crashes with a cryptic error.
            // also, it seems two attachments in the same FBO must have the same
            // pixel format, else you get the same cryptic error
            texCells = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texCells);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                computeWidth, computeHeight, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // depth buffer
            int texDepth = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texDepth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32,
                computeWidth, computeHeight, 0,
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
        }


        /// <summary>
        /// (TODO) Calculates an array simulation
        /// for a single array, single set of parameters, single moment in time.
        /// 
        /// Returns array output in watts, along with some other data.
        /// </summary>
        public ArraySimulationStepOutput Simulate(ArraySimulationStepInput simInput) {
            // validate that we're gtg
            if (Array.Mesh == null) throw new InvalidOperationException("No array shape (mesh) loaded.");
            if (Array.LayoutTexture == null) throw new InvalidOperationException("No array layout (texture) loaded.");

            SetUniforms();
            GLUtils.LoadTexture(Array.LayoutTexture, TextureUnit.Texture0);
            ComputeRender(simInput);
            return AnalyzeComputeTex();
        }

        public void ComputeRender(ArraySimulationStepInput simInput) {
            Debug.WriteLine("rendering insolation+cells into a "
                + computeWidth + "x" + computeWidth + " fbo");

            /* gl state */
            GL.UseProgram(shaderProg);

            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboWatts);
            GL.DrawBuffers(2, new DrawBuffersEnum[]{
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment0Ext,
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment1Ext});
            GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters

            GL.BindTexture(TextureTarget.Texture2D, texArray);
            GL.Viewport(0, 0, computeWidth, computeHeight);
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SetModelViewSun(GetSunDir(simInput));
            GLUtils.SetCameraProjectionOrtho(computeWidth, computeHeight);

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

        private void SetUniforms() {
            GL.UseProgram(shaderProg);
            GL.Uniform1(uniformX0, Array.LayoutBoundsXZ.Left); 
            GL.Uniform1(uniformX1, Array.LayoutBoundsXZ.Right); 
            GL.Uniform1(uniformZ0, Array.LayoutBoundsXZ.Top); 
            GL.Uniform1(uniformZ1, Array.LayoutBoundsXZ.Bottom); 
            GL.Uniform1(uniformPixelArea, 1.0f);
            GL.Uniform1(uniformSolarCells, (float)TextureUnit.Texture0);
            Debug.WriteLine("uniforms set.");
        }

        /// <summary>
        /// Reads the compute textures from OpenGL.
        /// This gives insolation for each cell.
        /// 
        /// Uses this to calculate IV curves, etc, and ultimately array power.
        /// </summary>
        private ArraySimulationStepOutput AnalyzeComputeTex() {
            byte[] texCellIdsRaw = new byte[computeWidth*computeHeight*4];
            Color[] texCellIdsC = new Color[computeWidth*computeHeight];
            string[] texCellIds = new string[computeWidth * computeHeight];
            float[] texWattsIn = new float[computeWidth * computeHeight];
            unsafe{
                fixed(byte* pb = texCellIdsRaw){
                    // cell id
                    GL.ReadBuffer((ReadBufferMode)FramebufferAttachment.ColorAttachment0);
                    GL.ReadPixels(0, 0, computeWidth, computeHeight,
                        OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr(pb));
                }
                fixed (float* pf = texWattsIn) {
                    // cell id
                    GL.ReadBuffer((ReadBufferMode)FramebufferAttachment.ColorAttachment1);
                    GL.ReadPixels(0, 0, computeWidth, computeHeight,
                        OpenTK.Graphics.OpenGL.PixelFormat.Blue, PixelType.Float, new IntPtr(pf));
                }
            }

            // find the cell id of each fragment...
            for (int i = 0; i < computeWidth * computeHeight; i++) {
                texCellIdsC[i] = Color.FromArgb(
                    texCellIdsRaw[i * 4 + 3], // rgba to argb
                    texCellIdsRaw[i * 4 + 0],
                    texCellIdsRaw[i * 4 + 1],
                    texCellIdsRaw[i * 4 + 2]);
            }
            //TODO: let the user id the colors, spec how they're wired
            var colorToId = spec.CellIDs;
            for (int i = 0; i < computeWidth * computeHeight; i++) {
                Color color = texCellIdsC[i];
                if (color.R == color.G && color.G == color.B) {
                    // ignore grayscale
                    continue;
                }
                string cellId;
                if (colorToId.ContainsKey(color)) {
                    cellId = colorToId[color];
                } else {
                    continue;
                }
                texCellIds[i] = cellId;
            }

            // now, find the total insolation for each cell id
            var idToInsolation = new Dictionary<string, double>();
            //var cellIds = new ArrayList<string>(colorToId.Values);
            foreach (String cellId in colorToId.Values) {
                idToInsolation[cellId] = 0.0;
            }
            for (int i = 0; i < computeWidth * computeHeight; i++) {
                String cellId = texCellIds[i];
                if (cellId == null) continue;
                double insolation = texWattsIn[i];
                idToInsolation[cellId] += insolation;
            }
            foreach (String cellId in idToInsolation.Keys) {
                Debug.WriteLine("cell " + cellId + " insolation " + idToInsolation[cellId]);
            }

            // find totals
            double totalArea = 0, totalWattsIn = 0;
            for (int i = 0; i < computeWidth * computeHeight; i++) {
                totalWattsIn += texWattsIn[i];
                totalArea += 0; // texArea[i];
            }

            // MPPT sweeps, first for each cell...
            double totalWattsOutByCell = 0, totalWattsOutByString = 0;
            foreach(String cellId in idToInsolation.Keys){
                double ff, vmp, imp;
                double[] veci, vecv;
                this.Array.CellSpec.CalcSweep(out ff, out vmp, out imp, out veci, out vecv);
                totalWattsOutByCell += vmp*imp;
            }
            // TODO: ... then MPPT for each string
            totalWattsOutByString = totalWattsOutByCell;

            ArraySimulationStepOutput output = new ArraySimulationStepOutput();
            output.ArrayArea = output.ArrayLitArea = totalArea;
            output.WattsInsolation = totalWattsIn;
            output.WattsOutputByCell = totalWattsOutByCell;
            output.WattsOutput = totalWattsOutByString;
            return output;
        }

        /// <summary>
        /// Reads an OpenGL buffer. Returns the contents as an image (specifically, 32-bpp ARGB bitmap).
        /// </summary>
        private Bitmap ReadBuffer(FramebufferAttachment buf) {
            GL.ReadBuffer((ReadBufferMode)buf);
            Bitmap bmp = new Bitmap(computeWidth, computeHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, computeWidth, computeHeight),
                ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.ReadPixels(0, 0, computeWidth, computeHeight,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bmpData.Scan0);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private void DebugSaveBuffers() {
            Debug.WriteLine("saving the ext fbo buffers");
            DebugSaveBuffer(FramebufferAttachment.ColorAttachment0, "../../../../test0.png");
            DebugSaveBuffer(FramebufferAttachment.ColorAttachment1, "../../../../test1.png");
        }
        private void DebugSaveBuffer(FramebufferAttachment buf, String fname) {
            Bitmap bmp = ReadBuffer(buf);
            Debug.WriteLine("writing " + fname);
            bmp.Save(fname);
        }

    }
}