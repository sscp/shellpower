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
        KDTree<MeshTriangle> kdTree;
        Mesh mesh;
        Shadow shadow;

        /* view */
        ShadowMeshSprite shadowSprite;

        /* data i/o */
        GeoNames geoNamesApi = new GeoNames();

        /* simulation state */
        ArraySimulationStepInput simInput = new ArraySimulationStepInput();
        ArraySimulationStepOutput simOutput = new ArraySimulationStepOutput();

        /* ui */
        bool select;
        Point dragStart;

        public MainForm() {
            InitializeComponent();
            LoadModel("../../../../meshes/apogee.3dxml");
            GuiSimStepInputs(null, null);
        }

        void LoadModel(string filename) {
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
                //} else if (extension.Equals("stl")) {
                //    parser = new MeshParserStl();
            } else {
                throw new ArgumentException("unsupported filetype: " + extension);
            }
            parser.Parse(filename);
            return parser.GetMesh();
        }

        private void SetModel(Mesh mesh) {
            this.mesh = mesh;

            //create shadows volumes
            shadow = new Shadow(mesh);
            shadow.Initialize();
            shadow.Light = new Vector4(glControl.SunDirection, 0);
            shadow.ComputeShadows();

            //render the mesh, with shadows, centered in the viewport
            shadowSprite = new ShadowMeshSprite(shadow);
            var center = (mesh.BoundingBox.Max + mesh.BoundingBox.Min) / 2;
            shadowSprite.Position = new Vector4(-center, 1);

            //create kd tree
            var tris = mesh.triangles
                .Select((tri, ix) => new MeshTriangle() {
                    Mesh = mesh,
                    Triangle = ix
                })
                .ToList();
            kdTree = new KDTree<MeshTriangle>();
            kdTree.AddAll(tris);

            glControl.Sprite = shadowSprite;
        }

        Vector3 MouseProject(int x, int y) {
            /* change screen to world coordinates */
            Vector3 world = glControl.ScreenToWorld(x, y);
            Vector3 position = glControl.Position;
            Vector3 direction = world - position;
            Vector3 intercept = position - direction * (position.Y / direction.Y);
            return intercept;
        }

        void MouseHighlight(Rectangle screenArea) {
            /* intersect ray thru mouseclick with the ground */
            Vector3 model = MouseProject(screenArea.Left, screenArea.Top);
            Vector3 model2 = MouseProject(screenArea.Right, screenArea.Bottom);
            var pos = new Vector3(glControl.Sprite.Position);
            Quad3 volume = new Quad3() {
                Min = new Vector3(
                    Math.Min(model.X, model2.X),
                    -100f,
                    Math.Min(model.Z, model2.Z)) - pos,
                Max = new Vector3(
                    Math.Max(model.X, model2.X),
                    100f,
                    Math.Max(model.Z, model2.Z)) - pos
            };

            /* paint part of the car red */
            DateTime start = DateTime.Now;
            shadowSprite.FaceColors = new Vector4[mesh.triangles.Length];
            int count = 0;
            foreach (var tri in kdTree.GetElementsInVolume(volume)) {
                count++;
                shadowSprite.FaceColors[tri.Triangle] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            }
            Debug.WriteLine("{0} tris selected in {1:0.000}s", count, (DateTime.Now - start).TotalSeconds);

            glControl.Refresh();
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
            //(float)(simInput.Heading + Math.Cos(simInput.LocalTime.TimeOfDay.TotalHours * 2 * Math.PI / 24));
            var elevation = Astro.solar_elevation(
                (int)sidereal.TimeOfDay.TotalSeconds,
                simInput.LocalTime.DayOfYear,
                simInput.Latitude);
            //(float)(-Math.Cos(simInput.LocalTime.TimeOfDay.TotalHours * 2 * Math.PI / 24) - 0.2 * Math.Cos(simInput.LocalTime.DayOfYear * 2 * Math.PI / 365));
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

            //update the view
            glControl.SunDirection = new Vector3(
                (float)(-Math.Cos(elevation) * Math.Cos(azimuth)), (float)(Math.Sin(elevation)),
                (float)(-Math.Cos(elevation) * Math.Sin(azimuth)));
            if (elevation < 0) {
                glControl.SunDirection = Vector3.Zero;
            }

            //recalculate the shadows
            shadow.Light = new Vector4(glControl.SunDirection, 0);
            shadow.ComputeShadows();
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
            dateTimePicker.Value = simInput.Utc;
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

        }

        private void openSimParamsToolStripMenuItem_Click(object sender, EventArgs e) {

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

        private void glControl_Mouse(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                select = false;
            }
        }

        private void glControl_Move(object sender, MouseEventArgs e) {
            if (select) {
                var screenArea = new Rectangle(dragStart, new Size(e.Location.X - dragStart.X, e.Location.Y - dragStart.Y));
                MouseHighlight(screenArea);
            }
            glControl.Cursor = MouseProject(e.X, e.Y);
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                select = true;
                dragStart = e.Location;
            }
        }
    }
}
