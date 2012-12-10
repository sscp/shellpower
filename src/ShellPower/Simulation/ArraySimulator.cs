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
        private int uniformPixelWattsIn, uniformSolarCells;
        private Bitmap cacheSolarCells;
        private int uniformX0, uniformX1, uniformZ0, uniformZ1;
        private int texArray;

        /* ... program outputs to a texture */
        private int texWatts, texCells, texArea;
        private int computeWidth, computeHeight;
        private int fboWatts;

        /* model */
        public ArraySimulator() {
            InitGLArrayComputeShaders();
            InitGLComputeBuffer();
            InitGLArrayTextures();
        }

        private const double ARRAY_DIM_M = 5; // largest dim in meters. TODO: make this not hardcoded
        private const int COMPUTE_TEX_SIZE = 2048; // width and height of the compute textures

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
varying float areaMult;
void main()
{
    vec4 mv = gl_ModelViewMatrix * gl_Vertex;
    gl_Position = gl_ProjectionMatrix * mv;
    vec3 normal = gl_NormalMatrix * gl_Normal;
    cosRule = normal.z; //dot(normal, vec3(0,0,1));
    areaMult = clamp(sqrt(dot(normal,normal))/normal.z, 0, 24);

    gl_TexCoord[0] = vec4((gl_Vertex.x - x0) / (x1 - x0), (gl_Vertex.z - z0) / (z1 - z0), 0,0);
}");
            GL.ShaderSource(shaderFrag, @"
varying float cosRule;
varying float areaMult;
uniform float pixelWattsIn;
uniform float pixelArea;
uniform sampler2D solarCells;

// encodes a float as an RGBA color
// precision: 1/256 (0.004) 
// range: 0 to 100
vec4 encodeFloat(float val){
    float mwRed = floor(val) * 2 / 255; 
    // should come out R=2 for val=0.1mw, 4 for 0.2mw, etc
    float mwGreen = val-floor(val);
    return vec4(mwRed, mwGreen, 0.0, 1.0);   // watts insolation
}

void main()
{
    vec4 solarCell = texture2D(solarCells, gl_TexCoord[0].xy);
    float watts10k = pixelWattsIn*max(cosRule,0)*10000.0; 

    gl_FragData[0] = vec4(solarCell.xyz, 1.0); // cell id
    gl_FragData[1] = encodeFloat(watts10k); // insolation in 0.1 mw
    gl_FragData[2] = encodeFloat(areaMult*4); // pixel area
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
            uniformPixelWattsIn = GL.GetUniformLocation(shaderProg, "pixelWattsIn");
            uniformSolarCells = GL.GetUniformLocation(shaderProg, "solarCells");
            Debug.Assert(uniformX0 >= 0 && uniformX1 >= 0 && uniformZ0 >= 0 && uniformZ1 >= 0
                && uniformPixelWattsIn >= 0 && uniformSolarCells >= 0);
        }

        /// <summary>
        /// Creates the output buffers.
        /// </summary>
        private void InitGLComputeBuffer() {
            computeWidth = computeHeight = COMPUTE_TEX_SIZE;

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

            // a third buffer for area in m^2...
            texArea = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texArea);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                computeWidth, computeHeight, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

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
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment2Ext, TextureTarget.Texture2D, texArea, 0);
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
            if (simInput == null || simInput.Array == null) throw new InvalidOperationException("No array specified.");
            if (simInput.Array.Mesh == null) throw new InvalidOperationException("No array shape (mesh) loaded.");
            if (simInput.Array.LayoutTexture == null) throw new InvalidOperationException("No array layout (texture) loaded.");

            SetUniforms(simInput.Array, simInput.Insolation);
            ComputeRender(simInput);
            return AnalyzeComputeTex(simInput);
        }

        public void ComputeRender(ArraySimulationStepInput simInput) {
            Debug.WriteLine("rendering insolation+cells into a "
                + computeWidth + "x" + computeWidth + " fbo");

            /* gl state */
            GL.UseProgram(shaderProg);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboWatts);
            GL.DrawBuffers(3, new DrawBuffersEnum[]{
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment0Ext,
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment1Ext,
                (DrawBuffersEnum)FramebufferAttachment.ColorAttachment2Ext});

            GL.BindTexture(TextureTarget.Texture2D, texArray);
            GL.Viewport(0, 0, computeWidth, computeHeight);
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SetModelViewSun(GetSunDir(simInput));
            GLUtils.SetCameraProjectionOrtho(ARRAY_DIM_M);

            //render
            MeshSprite sprite = new MeshSprite(simInput.Array.Mesh);
            sprite.PushTransform();
            sprite.Render();
            sprite.PopTransform();
            DebugSaveBuffers();
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

        private void SetUniforms(ArraySpec array, double insolation) {
            GL.UseProgram(shaderProg);

            // array layout alignment
            GL.Uniform1(uniformX0, array.LayoutBoundsXZ.Left); 
            GL.Uniform1(uniformX1, array.LayoutBoundsXZ.Right); 
            GL.Uniform1(uniformZ0, array.LayoutBoundsXZ.Top);
            GL.Uniform1(uniformZ1, array.LayoutBoundsXZ.Bottom);

            // array layout texture. shows where each cell is located.
            if (cacheSolarCells != array.LayoutTexture) {
                cacheSolarCells = array.LayoutTexture;
                GLUtils.LoadTexture(array.LayoutTexture, TextureUnit.Texture0);
            }
            GL.Uniform1(uniformSolarCells, (float)TextureUnit.Texture0);
            
            // solar insolation per pixel rendered 
            // (since we are rendering orth projection from the sun's pov, this is a constant)
            double m2PerPixel = ARRAY_DIM_M*ARRAY_DIM_M/COMPUTE_TEX_SIZE/COMPUTE_TEX_SIZE;
            double wattsPerPixel = m2PerPixel * insolation;
            GL.Uniform1(uniformPixelWattsIn, (float)wattsPerPixel);
            
            Debug.WriteLine("uniforms set.");
        }

        private float[] ReadFloatTexture(FramebufferAttachment fb, double scale) {
            // reading from an actual float texture is unreliable, so encode floats as colors instead
            byte[] tex = new byte[computeWidth * computeHeight * 4];
            unsafe {
                fixed (byte* pf = tex) {
                    // cell id
                    GL.ReadBuffer((ReadBufferMode)fb);
                    GL.ReadPixels(0, 0, computeWidth, computeHeight,
                        OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr(pf));
                }
            }
            float[] texDecoded = new float[computeWidth * computeHeight];
            for (int i = 0; i < computeWidth * computeHeight; i++) {
                byte r = tex[i * 4 + 0], g = tex[i * 4 + 1], b = tex[i * 4 + 2], a = tex[i * 4 + 3];
                if (r == 255 && g == 255 && b == 255) continue;
                Debug.Assert(a == 255);
                Debug.Assert(r % 2 == 0 && r < 200); // shader worked, no antialiasing -> red must be a small even num
                Debug.Assert(b == 0); // blue must be zero. green can be anything.
                float watts = (float)(scale*(r / 2 + (double)g / 255));
                texDecoded[i] = watts;
            }
            return texDecoded;
        }

        /// <summary>
        /// Reads an ARGB texture.
        /// Returns an array of colors (scanline order).
        /// </summary>
        private Color[] ReadColorTexture(FramebufferAttachment fb) {
            byte[] texRaw = new byte[computeWidth*computeHeight*4];
            Color[] tex = new Color[computeWidth*computeHeight];
            unsafe {
                fixed (byte* pb = texRaw) {
                    // cell id
                    GL.ReadBuffer((ReadBufferMode)fb);
                    GL.ReadPixels(0, 0, computeWidth, computeHeight,
                        OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, new IntPtr(pb));
                }
            }
            for (int i = 0; i < computeWidth * computeHeight; i++) {
                tex[i] = Color.FromArgb(
                    texRaw[i * 4 + 3],
                    texRaw[i * 4 + 2],
                    texRaw[i * 4 + 1],
                    texRaw[i * 4 + 0]);
            }
            return tex;
        }

        /// <summary>
        /// Reads the compute textures from OpenGL.
        /// This gives insolation for each cell.
        /// 
        /// Uses this to calculate IV curves, etc, and ultimately array power.
        /// </summary>
        private ArraySimulationStepOutput AnalyzeComputeTex(ArraySimulationStepInput input) {
            Color[] texColors = ReadColorTexture(FramebufferAttachment.ColorAttachment0);
            float[] texWattsIn = ReadFloatTexture(FramebufferAttachment.ColorAttachment1, 0.0001);
            double areaPerPixel = ARRAY_DIM_M*ARRAY_DIM_M/COMPUTE_TEX_SIZE/COMPUTE_TEX_SIZE;
            float[] texArea = ReadFloatTexture(FramebufferAttachment.ColorAttachment2, areaPerPixel/4);
            /*double dbgmin = texWattsIn[0], dbgmax = texWattsIn[0], dbgavg = 0;
            for (int i = 0; i < texWattsIn.Length; i++) {
                dbgmin = Math.Min(dbgmin, texWattsIn[i]);
                dbgmax = Math.Max(dbgmax, texWattsIn[i]);
                dbgavg += texWattsIn[i];
            }
            dbgavg /= texWattsIn.Length;*/

            // find the cell at each fragment...
            int ncells = 0;
            var cells = new List<ArraySpec.Cell>();
            var colorToId = new Dictionary<Color, int>();
            foreach (ArraySpec.CellString cellStr in input.Array.Strings) {
                foreach (ArraySpec.Cell cell in cellStr.Cells) {
                    cells.Add(cell);
                    colorToId.Add(cell.Color, ncells);
                    ncells++;
                }
            }

            // finally, find the area and insolation for each cell
            double[] wattsIn = new double[ncells];
            double[] areas = new double[ncells];
            double wattsInUnlinked = 0, areaUnlinked = 0;
            for (int i = 0; i < computeWidth * computeHeight; i++) {
                Color color = texColors[i];
                if (ColorUtils.IsGrayscale(color)) continue;
                if (colorToId.ContainsKey(color)) {
                    int id = colorToId[color];
                    wattsIn[id] += texWattsIn[i];
                    areas[id] += texArea[i];
                } else {
                    wattsInUnlinked += texWattsIn[i];
                    areaUnlinked += texArea[i];
                }
            }
            if (areaUnlinked > 0 || wattsInUnlinked > 0) {
                Logger.warn("Found texels that are not grayscale, " +
                    "but also doesn't correspond to any cell. Have you finished your layout?" +
                    "\n\tTotal of {0}m^2 cell area not in any string, with {1}W insolation.", 
                    areaUnlinked,wattsInUnlinked);
            }

            // find totals
            double totalArea = 0, totalWattsIn = 0;
            for (int i = 0; i < ncells; i++) {
                totalWattsIn += wattsIn[i];
                totalArea += areas[i];
                Debug.WriteLine("cell {0}: {1}W, {2}m^2", i, wattsIn[i], areas[i]);
            }
            Debug.WriteLine("total: {0}W, {1}m^2", totalWattsIn, totalArea);

            // MPPT sweeps, for each cell and each string. 
            // Inputs:
            CellSpec spec = input.Array.CellSpec;
            double tempC = input.Temperature;
            int nstrings = input.Array.Strings.Count;
            // Outputs:
            double totalWattsOutByCell = 0;
            double totalWattsOutByString = 0;
            var strings = new ArraySimStringOutput[nstrings];
            int cellIx = 0;
            for(int i = 0; i < nstrings; i++){
                var cellStr = input.Array.Strings[i];
                double stringWattsIn = 0, stringWattsOutByCell = 0, stringLitArea = 0;

                // per-cell sweeps
                var cellSweeps = new IVTrace[cellStr.Cells.Count];
                for(int j = 0; j < cellStr.Cells.Count; j++){
                    double cellWattsIn = wattsIn[cellIx++];
                    double cellInsolation = cellWattsIn / spec.Area;
                    IVTrace cellSweep = CellSimulator.CalcSweep(spec, cellInsolation, tempC);
                    cellSweeps[j] = cellSweep;

                    stringWattsIn += cellWattsIn;
                    stringWattsOutByCell += cellSweep.Pmp;
                    totalWattsOutByCell += cellSweep.Pmp;

                    // shading stats
                    stringLitArea += areas[i];
                }

                // string sweep
                strings[i] = new ArraySimStringOutput();
                strings[i].WattsIn = stringWattsIn;
                strings[i].WattsOutputByCell = stringWattsOutByCell;
                IVTrace stringSweep = StringSimulator.CalcStringIV(cellStr, cellSweeps, input.Array.BypassDiodeSpec);
                strings[i].WattsOutput = stringSweep.Pmp;
                strings[i].IVTrace = stringSweep;

                // higher-level string info
                strings[i].String = cellStr;
                CellSpec cellSpec = input.Array.CellSpec;
                strings[i].Area = cellStr.Cells.Count*cellSpec.Area;
                strings[i].AreaShaded = strings[i].Area - stringLitArea;
                IVTrace cellSweepIdeal = CellSimulator.CalcSweep(cellSpec,input.Insolation,input.Temperature);
                strings[i].WattsOutputIdeal = cellSweepIdeal.Pmp * cellStr.Cells.Count;
            }
            totalWattsOutByString = totalWattsOutByCell;

            ArraySimulationStepOutput output = new ArraySimulationStepOutput();
            output.ArrayArea = ncells * spec.Area;
            output.ArrayLitArea = totalArea;
            output.WattsInsolation = totalWattsIn;
            output.WattsOutputByCell = totalWattsOutByCell;
            output.WattsOutput = totalWattsOutByString;
            output.Strings = strings;
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
            DebugSaveBuffer(FramebufferAttachment.ColorAttachment2, "../../../../test2.png");
        }
        private void DebugSaveBuffer(FramebufferAttachment buf, String fname) {
            Bitmap bmp = ReadBuffer(buf);
            Debug.WriteLine("writing " + fname);
            bmp.Save(fname);
        }

    }
}