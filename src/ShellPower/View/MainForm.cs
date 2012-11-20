using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using OpenTK;

namespace SSCP.ShellPower {
    public partial class MainForm : Form {
        /* model */
        ArraySpec array = new ArraySpec();
        Shadow shadow;

        /* apis */
        GeoNames geoNamesApi = new GeoNames();

        /* current simulation state */
        ArraySimulationStepInput simInput = new ArraySimulationStepInput();
        ArraySimulationStepOutput simOutput = new ArraySimulationStepOutput();

        /* sub views */
        ArrayLayoutForm arrayLayoutForm;
        CellParamsForm cellParamsForm;

        public MainForm() {
            // init view
            InitializeComponent();
            arrayLayoutForm = new ArrayLayoutForm(array);
            cellParamsForm = new CellParamsForm(array);

            // init model
            InitializeArraySpec();

            // run the first update step
            CalculateSimInput();
            CalculateSimStep();
        }

        private void InitializeArraySpec() {
            //TODO: remove hack, here to make debugging faster
            LoadModel("C:/shellpower/meshes/sunbadThinCarWholeRotSmall.stl");

            // Sunpower C60 Bin I
            // http://www.kyletsai.com/uploads/9/7/5/3/9753015/sunpower_c60_bin_ghi.pdf
            array.CellSpec.IscStc = 6.27;
            array.CellSpec.VocStc = 0.686;
            array.CellSpec.DIscDT = -0.0020; // approx, computed
            array.CellSpec.DVocDT = -0.0018;
            array.CellSpec.Area = 0.015555; // m^2
            array.CellSpec.Temperature = 25; // deg c
            array.CellSpec.NIdeal = 1.26; // fudge
            array.CellSpec.SeriesR = 0.003; // ohms
        }

        private void LoadModel(string filename) {
            Mesh mesh = LoadMesh(filename);
            toolStripStatusLabel.Text = string.Format("Loaded model {0}, {1} triangles",
                System.IO.Path.GetFileName(filename),
                mesh.triangles.Length);
            SetModel(mesh);
        }

        private Mesh LoadMesh(String filename) {
            String extension = filename.Split('.').Last().ToLower();
            IMeshParser parser;
            if (extension.Equals("3dxml")) {
                parser = new MeshParser3DXml();
            } else if (extension.Equals("stl")) {
                parser = new MeshParserStl();
            } else {
                throw new ArgumentException("unsupported filetype: " + extension);
            }
            parser.Parse(filename);
            return parser.GetMesh();
        }
        
        /// <summary>
        /// Uses the given mesh for rendering and calculation.
        /// 
        /// Computes shadow volumes for rendering.
        /// </summary>
        private void SetModel(Mesh mesh) {
            array.Mesh = mesh;
            
            // create shadow volumes
            Logger.info("computing shadows...");
            shadow = new Shadow(mesh);
            shadow.Initialize();

            // render them
            ShadowMeshSprite shadowSprite = new ShadowMeshSprite(shadow);
            var center = (mesh.BoundingBox.Max + mesh.BoundingBox.Min) / 2;
            shadowSprite.Position = new Vector4(-center, 1);
            glControl.Sprite = shadowSprite;
        }

        private Vector3 GetSunDir() {
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

            //recalculate the shadows
            var lightDir = new Vector3(
                (float)(-Math.Cos(elevation) * Math.Cos(azimuth)), (float)(Math.Sin(elevation)),
                (float)(-Math.Cos(elevation) * Math.Sin(azimuth)));
            if (elevation < 0) {
                lightDir = Vector3.Zero;
            }
            if (shadow != null && lightDir.LengthSquared > 0) {
                shadow.Light = new Vector4(lightDir, 0);
                shadow.ComputeShadows();
            }
            return lightDir;
        }

        private void CalculateSimStep() {
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

            //recalculate the shadows
            var lightDir = new Vector3(
                (float)(-Math.Cos(elevation) * Math.Cos(azimuth)), (float)(Math.Sin(elevation)),
                (float)(-Math.Cos(elevation) * Math.Sin(azimuth)));
            if (elevation < 0) {
                lightDir = Vector3.Zero;
            }
            if (shadow != null && lightDir.LengthSquared > 0) {
                shadow.Light = new Vector4(lightDir, 0);
                shadow.ComputeShadows();
            }

            // TODO: clean up MainForm. delete the above.

            // update the view
            glControl.SunDirection = lightDir;

            // calculate array params
            //TODO: fix this hackery
            const float insolation = 1000f; // W/m^2
            const float efficiency = 0.227f; 
            float arrayArea = 0.0f, shadedArea = 0.0f, totalWatts = 0.0f;
            int nt = array.Mesh == null ? 0 : array.Mesh.triangles.Length;
            for (int i = 0; i < nt; i++) {
                var tri = array.Mesh.triangles[i];
                var vA = array.Mesh.points[tri.vertexA];
                var vB = array.Mesh.points[tri.vertexB];
                var vC = array.Mesh.points[tri.vertexC];
                float area = Vector3.Cross(vC - vA, vB - vA).Length / 2;
                arrayArea += area;

                int nshad = 0;
                if (shadow.VertShadows[tri.vertexA]) nshad++;
                if (shadow.VertShadows[tri.vertexB]) nshad++;
                if (shadow.VertShadows[tri.vertexC]) nshad++;
                shadedArea += area * nshad / 3.0f;

                // if we're not in a shadow, get cosine rule insolation.
                if (nshad < 2) {
                    var cosInsolation = Math.Max(Vector3.Dot(tri.normal, lightDir), 0f) * insolation;
                    var watts = cosInsolation * efficiency * area;
                    totalWatts += watts;
                }
            }

            //update ui
            this.labelArrPower.Text = string.Format(
                "{0:0}W over {1:0.00}m\u00B2, {2:0.00}m\u00B2 shaded",
                totalWatts, arrayArea, shadedArea);
        }

        /// <summary>
        /// Updates 3D rendering (view) from environment (model).
        /// </summary>
        private void RefreshModelView() {
            glControl.Refresh();
        }

        /// <summary>
        /// Updates sim controls (view) from sim state (model).
        /// </summary>
        private void UpdateSimStateView() {
            /* set heading */
            string[] headings = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            int dirIx = (int)Math.Round(simInput.Heading / (2 * Math.PI) * 16);
            if (dirIx >= headings.Length) dirIx -= headings.Length;
            labelCarDirection.Text = headings[dirIx];

            /* set date/time */
            dateTimePicker.Value = simInput.Utc; // fix roundoff problems
            labelTimezone.Text = string.Format("GMT{0}{1:0.0}", simInput.Timezone >= 0 ? "+" : "", simInput.Timezone);
            var name = geoNamesApi.GetTimezoneName(simInput.Latitude, simInput.Longitude);
            if (name != null)
                labelTimezone.Text += " " + name;
            labelLocalTime.Text = simInput.LocalTime.ToString("HH:mm:ss");
            trackBarTimeOfDay.Value = (int)(simInput.LocalTime.TimeOfDay.TotalHours * (trackBarTimeOfDay.Maximum + 1) / 24);
        }

        private void openModelToolStripMenuItem_Click(object sender, EventArgs e) {
            if (openFileDialogModel.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                LoadModel(openFileDialogModel.FileName);
                CalculateSimStep();
                RefreshModelView();
            }
        }

        private void openLayoutToolStripMenuItem_Click(object sender, EventArgs e) {
            Bitmap bitmap = arrayLayoutForm.PromptUserForLayoutTexture();
            if (bitmap != null) {
                array.LayoutTexture = bitmap;
            }
        }

        private void openSimParamsToolStripMenuItem_Click(object sender, EventArgs e) {

        }


        /// <summary>
        /// Called whenever the array changes.
        /// Uses our model of the array (mesh, texture, etc) to
        /// * update the view
        /// * update the simulator
        /// * recalculate
        /// </summary>
        private void UpdateModel() {
        }

        /// <summary>
        /// Called when one of the sim input GUIs changes.
        /// </summary>
        private void GuiSimStepInputs(object sender, EventArgs e) {
            CalculateSimInput();
        }

        private void CalculateSimInput(){
            var lat = double.Parse(textBoxLat.Text);
            var lon = double.Parse(textBoxLon.Text);

            /* get timezone */
            var tz = geoNamesApi.GetTimezone(lat, lon);

            /* get local time */
            DateTime utcTime = dateTimePicker.Value;
            DateTime localTime = utcTime + new TimeSpan((long)(tz * 60 * 60 * 10000000));

            /* get car orientation */
            double dir = 2 * Math.PI * trackBarCarDirection.Value / (trackBarCarDirection.Maximum + 1);

            /* set direction */
            double heading = 2 * Math.PI * trackBarCarDirection.Value / (trackBarCarDirection.Maximum + 1);

            /* update sim input */
            simInput.Heading = heading;
            simInput.Latitude = lat;
            simInput.Longitude = lon;
            simInput.Timezone = tz;
            simInput.LocalTime = localTime;
            simInput.Utc = utcTime;
            UpdateSimStateView();
            CalculateSimStep();
            RefreshModelView();
        }

        private void buttonRun_Click(object sender, EventArgs e) {

        }

        private void buttonAnimate_Click(object sender, EventArgs e) {

        }

        private void trackBarTimeOfDay_Scroll(object sender, EventArgs e) {
            double hours = (double)trackBarTimeOfDay.Value / (trackBarTimeOfDay.Maximum + 1) * 24;
            var timeOfDay = new TimeSpan((long)(hours * 60 * 60 * 10000000) + 1);
            simInput.LocalTime = simInput.LocalTime.Date + timeOfDay;
            simInput.Utc = simInput.LocalTime - new TimeSpan((long)(simInput.Timezone * 60 * 60 * 10000000));
            UpdateSimStateView();
        }

        private void btnRecalc_Click(object sender, EventArgs e) {
            var simulator = new ArraySimulator(array);
            try {
                simOutput = simulator.Simulate(simInput);

                Debug.WriteLine("array simulation output");
                Debug.WriteLine("   ... " + simOutput.ArrayLitArea + " m^2 exposed to sunlight");
                Debug.WriteLine("   ... " + simOutput.WattsInsolation + " W insolation");
                Debug.WriteLine("   ... " + simOutput.WattsOutput + " W output");
            } catch (Exception exc) {
                MessageBox.Show(exc.Message);
            }
        }

        private void layoutToolStripMenuItem_Click(object sender, EventArgs e) {
            arrayLayoutForm.ShowDialog();
        }

        private void cellParametersToolStripMenuItem_Click(object sender, EventArgs e) {
            cellParamsForm.ShowDialog();
        }
    }
}
