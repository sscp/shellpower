using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    public class ArraySimulator
    {
        private const int COMPUTE_TEX_SIZE = 2048; // width and height of the compute textures

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
            InitGLInputShaders();
            InitGLOutputBuffers();
            InitGLInputArrayTexture();
        }

        /// <summary>
        /// Shaders to compute array properties
        /// </summary>
        private void InitGLInputShaders() {
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
            Debug.WriteLine("vert shader compiled: " + GL.GetShaderInfoLog(shaderVert));
            GL.CompileShader(shaderFrag);
            Debug.WriteLine("frag shader compiled: " + GL.GetShaderInfoLog(shaderFrag));
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
        private void InitGLOutputBuffers() {
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
        }

        /// <summary>
        /// Loads the solar array as a texture.
        /// 
        /// Assumes a top-down projection, ie the texture UVs are the
        /// vertex X and Z coords with a scale factor and an offset.
        /// </summary>
        private void InitGLInputArrayTexture() {
            texArray = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texArray);
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
            if (simInput == null) throw new InvalidOperationException("No input specified.");
            Vector3 sunDir = GetSunDir(simInput);
            return Simulate(simInput.Array, sunDir, simInput.Irradiance, simInput.IndirectIrradiance, simInput.Temperature);
        }

        public ArraySimulationStepOutput Simulate(ArraySpec array, Vector3 sunDir, 
            double wPerM2Insolation, double wPerM2Indirect, double cTemp) {
            // validate that we're gtg
            if (array == null) throw new ArgumentException("No array specified.");
            if (array.Mesh == null) throw new ArgumentException("No array shape (mesh) loaded.");
            if (array.LayoutTexture == null) throw new ArgumentException("No array layout (texture) loaded.");
            if (wPerM2Insolation < 0) throw new ArgumentException("Invalid (negative) insolation.");
            if (Math.Abs(sunDir.Length - 1.0) > 1e-3) throw new ArgumentException("Sun direction must be a unit vector.");

            ArraySimulationStepOutput output;
            lock (typeof(GL)) {
                DateTime dt1 = DateTime.Now;
                SetUniforms(array, wPerM2Insolation);
                ComputeRender(array, sunDir);
                DebugSaveBuffers();
                output = AnalyzeComputeTex(array, wPerM2Insolation, wPerM2Indirect, cTemp);
                DateTime dt2 = DateTime.Now;

                Debug.WriteLine(string.Format("finished sim step! {0:0.000}s {1:0.0}/{2:0.0}W",
                    (double)(dt2.Ticks - dt1.Ticks) * 0.0000001,
                    output.WattsInsolation, output.WattsOutput));
            }

            return output;
        }

        /// <summary>
        /// Renders the array from the sun's point of view, into several buffers:
        /// one for cell id (texture map), one for insolation, and one for area.
        /// 
        /// After this step, results be read using OpenGL ReadBuffer and analyzed.
        /// </summary>
        public void ComputeRender(ArraySpec array, Vector3 sunDir) {
            Debug.WriteLine("rendering insolation+cells into a "
                + computeWidth + "x" + computeWidth + " fbo");
            
            // gl state
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

            Vector3 arrayCenter = ComputeArrayCenter(array);
            double arrayMaxDimension = ComputeArrayMaxDimension(array);
            SetCameraSunPOV(sunDir, arrayCenter, arrayMaxDimension);

            //render
            MeshSprite sprite = new MeshSprite(array.Mesh);
            sprite.PushTransform();
            sprite.Render();
            sprite.PopTransform();
        }

        //TODO: move to a utility class
        public static Vector3 GetSunDir(ArraySimulationStepInput simInput) {
            // update the astronomy model
            var utc_time = simInput.Utc;
            var sidereal = Astro.sidereal_time(
                utc_time,
                simInput.Longitude);
            var solarAzimuth = Astro.solar_azimuth(
                (int)sidereal.TimeOfDay.TotalSeconds,
                sidereal.DayOfYear,
                simInput.Latitude);
            var solarElevation = Astro.solar_elevation(
                (int)sidereal.TimeOfDay.TotalSeconds,
                sidereal.DayOfYear,
                simInput.Latitude);

            // correct for the car's heading and tilt
            var phi = solarAzimuth - simInput.Heading;

            var x = Math.Cos(solarElevation) * Math.Cos(phi); // phi 0 = forward = +X
            var y = Math.Cos(solarElevation) * Math.Sin(phi); // phi 90deg = left = +Y
            var z = Math.Sin(solarElevation); // up = +Z

            z = Math.Cos(simInput.Tilt) * z + Math.Sin(simInput.Tilt) * y; // +tilt = tilt right
            y = Math.Cos(simInput.Tilt) * y - Math.Sin(simInput.Tilt) * z;

            //recalculate the shadows
            return new Vector3((float)x, (float)z, (float)y);
        }

        /// <summary>
        /// Sets up the modelview and projection matrices.
        /// 
        /// Looks at a given model from a given point of view, orthographic projection,
        /// include the entire model in the viewport, Y direction is up.
        /// </summary>
        private void SetCameraSunPOV(Vector3 sunDir, Vector3 modelLocation, double modelMaxDimension) {
            // Look at the car from 50m away, from the POV of the sun
            // It needs to be far away to avoid view plane clipping.
            Matrix4 modelview = Matrix4.LookAt(modelLocation + sunDir*50f, modelLocation, -Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);
            GL.Light(LightName.Light0, LightParameter.Position, new Vector4(sunDir, 0));

            GLUtils.SetCameraProjectionOrtho(modelMaxDimension);
        }

        private void SetUniforms(ArraySpec array, double insolation) {
            GL.UseProgram(shaderProg);

            // array layout alignment
            GL.Uniform1(uniformX0, (float)array.LayoutBounds.MinX);
            GL.Uniform1(uniformX1, (float)array.LayoutBounds.MaxX);
            GL.Uniform1(uniformZ0, (float)array.LayoutBounds.MinZ);
            GL.Uniform1(uniformZ1, (float)array.LayoutBounds.MaxZ);

            // array layout texture. shows where each cell is located.
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texArray);
            if (cacheSolarCells != array.LayoutTexture) {
                cacheSolarCells = array.LayoutTexture;
                GLUtils.LoadTexture(array.LayoutTexture, TextureUnit.Texture0);
            }
            GL.Uniform1(uniformSolarCells, (float)TextureUnit.Texture0);
            
            // solar insolation per pixel rendered 
            // (since we are rendering orth projection from the sun's pov, this is a constant)
            double arrayDimM = ComputeArrayMaxDimension(array);
            double m2PerPixel = arrayDimM*arrayDimM/COMPUTE_TEX_SIZE/COMPUTE_TEX_SIZE;
            double wattsPerPixel = m2PerPixel * insolation;
            GL.Uniform1(uniformPixelWattsIn, (float)wattsPerPixel);
            
            Debug.WriteLine("uniforms set.");
        }

        /// <summary>
        /// Computes the diameter of the array (ie, the maximum dimension) in meters.
        /// 
        /// Approximates by finding the diagonal length of the bounding box, for simplicity.
        /// </summary>
        private double ComputeArrayMaxDimension(ArraySpec array) {
            Quad3 arrayBB = array.Mesh.BoundingBox;
            return (arrayBB.Max - arrayBB.Min).Length;
        }

        /// <summary>
        /// Finds the center of the array mesh. Uses the middle of the bounding box, for simplicity.
        /// </summary>
        private Vector3 ComputeArrayCenter(ArraySpec array) {
            Quad3 arrayBB = array.Mesh.BoundingBox;
            return (arrayBB.Max + arrayBB.Min) * 0.5f;
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
        private ArraySimulationStepOutput AnalyzeComputeTex(ArraySpec array, double wPerM2Insolation, double wPerM2Iindirect, double cTemp) {
            Color[] texColors = ReadColorTexture(FramebufferAttachment.ColorAttachment0);
            float[] texWattsIn = ReadFloatTexture(FramebufferAttachment.ColorAttachment1, 0.0001);
            double arrayDimM = ComputeArrayMaxDimension(array);
            double m2PerPixel = arrayDimM * arrayDimM / COMPUTE_TEX_SIZE / COMPUTE_TEX_SIZE;
            float[] texArea = ReadFloatTexture(FramebufferAttachment.ColorAttachment2, m2PerPixel / 4);
            double dbgmin = texArea[0], dbgmax = texArea[0], dbgavg = 0;
            for (int i = 0; i < texArea.Length; i++)
            {
                dbgmin = Math.Min(dbgmin, texArea[i]);
                dbgmax = Math.Max(dbgmax, texArea[i]);
                dbgavg += texArea[i];
            }
            dbgavg /= texArea.Length;

            // find the cell at each fragment...
            int ncells = 0;
            var cells = new List<ArraySpec.Cell>();
            var colorToId = new Dictionary<Color, int>();
            foreach (ArraySpec.CellString cellStr in array.Strings) {
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

            // add indirect insolation, encapsulation loss
            for (int i = 0; i < ncells; i++) {
                wattsIn[i] += array.CellSpec.Area * wPerM2Iindirect;
                wattsIn[i] *= (1.0 - array.EncapsulationLoss);
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
            CellSpec cellSpec = array.CellSpec;
            int nstrings = array.Strings.Count;
            // Outputs:
            double totalWattsOutByCell = 0;
            double totalWattsOutByString = 0;
            var strings = new ArraySimStringOutput[nstrings];
            int cellIx = 0;
            for(int i = 0; i < nstrings; i++){
                var cellStr = array.Strings[i];
                double stringWattsIn = 0, stringWattsOutByCell = 0, stringLitArea = 0;

                // per-cell sweeps
                var cellSweeps = new IVTrace[cellStr.Cells.Count];
                for(int j = 0; j < cellStr.Cells.Count; j++){
                    double cellWattsIn = wattsIn[cellIx++];
                    double cellInsolation = cellWattsIn / cellSpec.Area;
                    IVTrace cellSweep = CellSimulator.CalcSweep(cellSpec, cellInsolation, cTemp);
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
                IVTrace stringSweep = StringSimulator.CalcStringIV(cellStr, cellSweeps, array.BypassDiodeSpec);
                strings[i].WattsOutput = stringSweep.Pmp;
                strings[i].IVTrace = stringSweep;

                // higher-level string info
                strings[i].String = cellStr;
                strings[i].Area = cellStr.Cells.Count*cellSpec.Area;
                strings[i].AreaShaded = strings[i].Area - stringLitArea;
                IVTrace cellSweepIdeal = CellSimulator.CalcSweep(cellSpec,wPerM2Insolation,cTemp);
                strings[i].WattsOutputIdeal = cellSweepIdeal.Pmp * cellStr.Cells.Count;

                // total array power
                totalWattsOutByString += stringSweep.Pmp;
            }

            ArraySimulationStepOutput output = new ArraySimulationStepOutput();
            output.ArrayArea = ncells * cellSpec.Area;
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