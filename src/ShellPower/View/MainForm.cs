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
        MeshSprite sprite;
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
            MeshSprite sprite = LoadMesh(filename);
            toolStripStatusLabel.Text = string.Format("Loaded model {0}, {1} triangles",
                System.IO.Path.GetFileName(filename),
                sprite.Triangles.Length);

            SetModel(sprite);
        }

        private MeshSprite LoadMesh(String filename) {
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

        private void SetModel(MeshSprite sprite) {
            this.sprite = sprite;

            var center = (sprite.BoundingBox.Max + sprite.BoundingBox.Min) / 2;
            sprite.Position = new Vector4(-center, 1);
            sprite.Initialize();

            //create shadows volumes
            shadowSprite = new ShadowMeshSprite(sprite);
            shadowSprite.Light = new Vector4(glControl.SunDirection, 0);
            shadowSprite.Initialize();

            //create kd tree
            var tris = shadowSprite.Triangles
                .Select((tri, ix) => new MeshTriangle() {
                    Sprite = shadowSprite,
                    Triangle = ix
                })
                //TODO: remove disgusting hack
                .Where(tri => shadowSprite.Triangles[tri.Triangle].VertexA>=0)
                .ToList();
            kdTree = new KDTree<MeshTriangle>();
            kdTree.AddAll(tris);

            glControl.Sprite = shadowSprite;
        }

        MeshSprite MirrorAndCombine(MeshSprite sprite, Vector3 axis) {
            /* if the current mesh has n points, we'll have up to 2n-1 points in the mirrored and combined one */
            var points = new List<Vector3>();
            points.AddRange(sprite.Points);
            /* find the point or points we're going to mirror around--the ones closest in the 'axis' direction
             * (in other words, find the mirror plane) */
            var minDot = sprite.Points.Min(point => Vector3.Dot(point, axis));
            /* if points are this close or closer to the mirror plane, they will not be duplicated--they will simply be shared by additional triangles */
            var epsilon = 0.002f;
            /* create a dictionary mapping point indices in the original mesh to the corresponding mirrored points */
            var pointIndexMap = new Dictionary<int, int>();

            for (int i = 0; i < sprite.Points.Length; i++) {
                var distance = Vector3.Dot(sprite.Points[i], axis) - minDot;
                if (distance < epsilon) {
                    /* point lies on mirror plane */
                    pointIndexMap.Add(i, i);
                } else {
                    /* point is not on mirror plane; create its mirror image as a new point */
                    pointIndexMap.Add(i, points.Count);
                    var mirrorPoint = sprite.Points[i] - 2 * distance * axis;
                    points.Add(mirrorPoint);
                }
            }

            /* normals correspond to points, but may need to be mirrored.
             * if a point is on the mirror plane, make sure the corresponding normal is in the mirror plane
             */
            var normals = new Vector3[points.Count];
            for (int i = 0; i < sprite.Points.Length; i++) {
                if (pointIndexMap[i] == i) {
                    normals[i] = sprite.Normals[i] - Vector3.Dot(sprite.Normals[i], axis) * axis;
                    normals[i].Normalize();
                } else {
                    normals[i] = sprite.Normals[i];
                    normals[pointIndexMap[i]] = sprite.Normals[i] - 2 * Vector3.Dot(sprite.Normals[i], axis) * axis;
                }
            }

            /* triangles are simply duplicated--each triangle gets a mirror image */
            var triangles = new MeshSprite.Triangle[sprite.Triangles.Length * 2];
            for (int i = 0; i < sprite.Triangles.Length; i++) {
                triangles[i] = sprite.Triangles[i];
            }
            for (int i = 0; i < sprite.Triangles.Length; i++) {
                var triangle = sprite.Triangles[i];
                triangles[sprite.Triangles.Length + i] = new MeshSprite.Triangle() {
                    VertexA = pointIndexMap[triangle.VertexA],
                    VertexB = pointIndexMap[triangle.VertexB],
                    VertexC = pointIndexMap[triangle.VertexC]
                };
            }

            return new MeshSprite() {
                Points = points.ToArray(),
                Normals = normals,
                Triangles = triangles
            };
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
            shadowSprite.FaceColors = new Vector4[shadowSprite.Triangles.Length];
            int count = 0;
            foreach (var tri in kdTree.GetElementsInVolume(volume)) {
                count++;
                shadowSprite.FaceColors[tri.Triangle] = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            }
            Debug.WriteLine("{0} tris selected in {1:0.000}s", count, (DateTime.Now - start).TotalSeconds);

            glControl.Refresh();
        }

        private void CalculateSimStep() {
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

            glControl.SunDirection = new Vector3(
                (float)(-Math.Cos(elevation) * Math.Cos(azimuth)), (float)(Math.Sin(elevation)),
                (float)(-Math.Cos(elevation) * Math.Sin(azimuth)));
            shadowSprite.Light = new Vector4(glControl.SunDirection, 0);

            if (elevation < 0)
                glControl.SunDirection = Vector3.Zero;

            shadowSprite.ComputeShadowVolumes();

            //calculate azimuth, elevation, insolation
            // * adjust insolation based on time of day
            // * adjust sun vs ambient (cloud and sky) insolation by weather
            //calculate insolation for each cell (and resulting Isc / Voc)
            // * use shadow volumes  
            //for each string, determining which cells are in bypass at MPP
            //display power/cell, power, efficiency, Isc, Voc, num cells, and num cells in bypass for each string
            //add totals line
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
