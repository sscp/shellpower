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

        // obsolete:
        Mesh mesh;
        Shadow shadow;
        Mesh amended;
        bool[] trisInArray;

        /* apis */
        GeoNames geoNamesApi = new GeoNames();

        /* current simulation state */
        ArraySimulationStepInput simInput = new ArraySimulationStepInput();
        ArraySimulationStepOutput simOutput = new ArraySimulationStepOutput();

        public MainForm() {
            InitializeComponent();
            GuiSimStepInputs(null, null);

            //TODO: remove hack, here to make debugging faster
            LoadModel("C:/shellpower/meshes/sunbadThinCarWholeRotSmall.stl");
            LoadArrayTexture("C:/shellpower/arrays/texture.png");
            CalculateSimStep();
        }

        private void LoadArrayTexture(String arrayTex) {
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
            this.mesh = mesh;
            MeshSprite sprite = new MeshSprite(mesh);
            var center = (mesh.BoundingBox.Max + mesh.BoundingBox.Min) / 2;
            sprite.Position = new Vector4(-center, 1);
            
            //split out the array
            Logger.info("creating solar array boundary in the mesh...");
            // TODO: split mesh using the array tex?
            trisInArray = new bool[mesh.triangles.Length];
            amended = mesh;
            Logger.info("mesh now has " + amended.points.Length + " verts, " + amended.triangles.Length + " tris");

            //create shadows volumes
            Logger.info("computing shadows...");
            shadow = new Shadow(amended);
            shadow.Initialize();

            // color the array green
            Logger.info("creating shadow sprite");
            ShadowMeshSprite shadowSprite = new ShadowMeshSprite(shadow);
            int nt = amended.triangles.Length;
            shadowSprite.FaceColors = new Vector4[nt];
            var green = new Vector4(0.3f, 0.8f, 0.3f, 1f);
            var white = new Vector4(1f, 1f, 1f, 1f);
            for (int i = 0; i < nt; i++) {
                //TODO(dc): clean up
                shadowSprite.FaceColors[i] = white; // trisInArray[i] ? green : white;
            }

            //render the mesh, with shadows, centered in the viewport
            //var center = (amended.BoundingBox.Max + amended.BoundingBox.Min) / 2;
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
            int nt = amended == null ? 0 : amended.triangles.Length;
            for (int i = 0; i < nt; i++) {
                if (!trisInArray[i]) {
                    continue;
                }
                var tri = amended.triangles[i];
                var vA = amended.points[tri.vertexA];
                var vB = amended.points[tri.vertexB];
                var vC = amended.points[tri.vertexC];
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
            DialogResult result = openFileDialogArray.ShowDialog();
            if (result != DialogResult.OK) return;
            var texFile = openFileDialogArray.FileName;
            array.LayoutTexture = new Bitmap(texFile);
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

        private void GuiSimStepInputs(object sender, EventArgs e) {
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
            array.Mesh = mesh;
            var simulator = new ArraySimulator(array);
            simOutput = simulator.Simulate(simInput);
            Debug.WriteLine("array simulation output");
            Debug.WriteLine("   ... " + simOutput.ArrayLitArea + " m^2 exposed to sunlight");
            Debug.WriteLine("   ... " + simOutput.WattsInsolation + " W insolation");
            Debug.WriteLine("   ... " + simOutput.WattsOutput + " W output");
        }
    }
}
