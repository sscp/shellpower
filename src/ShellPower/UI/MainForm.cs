using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using OpenTK;
using System.IO;

namespace SSCP.ShellPower {
    public partial class MainForm : Form {
        /* model */
        ArraySimulationStepInput simInput = new ArraySimulationStepInput();
        Shadow shadow;

        /* sub views */
        ArrayLayoutForm arrayLayoutForm;
        CellParamsForm cellParamsForm;
        ArrayDimensionsForm arrayDimsForm;

        /* simulator */
        ArraySimulator simulator;

        public MainForm() {
            // init view
            InitializeComponent();
            tabControl1.SelectedIndex = 0;
            labelArrPower.Rtf = @"{\rtf1\ansi\deff0 Load model, load texture, then click simulate. }";

            // init model
            simInput.Array = new ArraySpec();
            InitTimeAndPlace();
            InitializeArraySpec();
            InitializeConditions();
            CalculateSimStepGui();

            // init subviews
            arrayLayoutForm = new ArrayLayoutForm(simInput.Array);
            cellParamsForm = new CellParamsForm(simInput);
            glControl.Array = simInput.Array;
            simInputControls.SimInput = simInput;
            InitOutputView();
        }

        private void InitOutputView() {
            outputArrayLayoutControl.Editable = false;
            outputArrayLayoutControl.Array = simInput.Array;
        }

        private void InitTimeAndPlace() {
            // Coober Pedy, SA, heading due south
            simInput.Longitude = 134.75555;
            simInput.Latitude = -29.01111;
            simInput.Heading = Math.PI;

            // Start of WSC 2013
            simInput.Utc = new DateTime(2013, 10, 6, 8, 0, 0).AddHours(-9.5);
            simInput.Timezone = TimeZoneInfo.FindSystemTimeZoneById("AUS Central Standard Time");
        }

        /// <summary>
        /// Hack to make debugging faster.
        /// </summary>
        private void InitializeArraySpec() {
            ArraySpec array = simInput.Array;
            array.LayoutBoundsXZ = new RectangleF(-0.115f, -0.23f, 2.15f, 4.820f);
            array.LayoutTexture = ArrayModelControl.DEFAULT_TEX;
            LoadModel("../../../../arrays/luminos/luminos.stl");

            // Sunpower C60 Bin I
            // http://www.kyletsai.com/uploads/9/7/5/3/9753015/sunpower_c60_bin_ghi.pdf
            CellSpec cellSpec = simInput.Array.CellSpec;
            cellSpec.IscStc = 6.27;
            cellSpec.VocStc = 0.686;
            cellSpec.DIscDT = -0.0020; // approx, computed
            cellSpec.DVocDT = -0.0018;
            cellSpec.Area = 0.015555; // m^2
            cellSpec.NIdeal = 1.26; // fudge
            cellSpec.SeriesR = 0.003; // ohms

            // Average bypass diode
            DiodeSpec diodeSpec = simInput.Array.BypassDiodeSpec;
            diodeSpec.VoltageDrop = 0.35;
        }

        private void InitializeConditions() {
            simInput.Temperature = 25; // STC, 25 Celcius
            simInput.Insolation = 1000; // STC, 1000 W/m^2
        }

        private void InitSimulator()
        {
            if (simulator == null)
            {
                simulator = new ArraySimulator();
            }
        }

        private void LoadModel(string filename) {
            Mesh mesh = LoadMesh(filename);
            Vector3 size = mesh.BoundingBox.Max - mesh.BoundingBox.Min;
            if (size.Length > 1000)
            {
                mesh = MeshUtils.Scale(mesh, 0.001f);
                size *= 0.001f;
            }
            toolStripStatusLabel.Text = string.Format("Loaded model {0}, {1} triangles, {2:0.00}x{3:0.00}x{4:0.00}m",
                System.IO.Path.GetFileName(filename),
                mesh.triangles.Length,
                size.X, size.Y, size.Z);
            
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
                throw new ArgumentException("Unsupported file type: " + extension);
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
            // create shadow volumes
            Logger.info("computing shadows...");
            Shadow newShadow = new Shadow(mesh);
            newShadow.Initialize(); // make sure init works before setting shadow
            shadow = newShadow;

            // render them
            ShadowMeshSprite shadowSprite = new ShadowMeshSprite(shadow);
            var center = (mesh.BoundingBox.Max + mesh.BoundingBox.Min) / 2;
            shadowSprite.Position = new Vector4(-center, 1);
            glControl.Sprite = shadowSprite;

            simInput.Array.Mesh = mesh;
        }

        /// <summary>
        /// Responds to simulation input change.
        /// Calculates as much as it can. Interactive, must be fast.
        /// Shadow visualization, not full calc.
        /// </summary>
        private void CalculateSimStepGui() {
            /* compute array power automatically? 
             * currently, only on Compute button press */
            /* update the view */
            UpdateShadowView();
        }

        /// <summary>
        /// Finds the position of the sun, or returns (0,0,0) if it's below the horizon.
        /// </summary>
        private Vector3 CalculateSunDir() {
            var lightDir = ArraySimulator.GetSunDir(simInput);
            // is the sun below the horizon? then it's night, return 0
            if (lightDir.Y < 0) {
                lightDir = Vector3.Zero;
            }
            return lightDir;
        }

        /// <summary>
        /// Updates 3D rendering (view) from sim inputs (model).
        /// </summary>
        private void UpdateShadowView() {
            /* compute the sun's position */
            Vector3 lightDir = CalculateSunDir();
            shadow.Light = new Vector4(lightDir, 0);
            shadow.ComputeShadows();
            glControl.Refresh();
        }

        private void openModelToolStripMenuItem_Click(object sender, EventArgs args) {
            if (openFileDialogModel.ShowDialog() != System.Windows.Forms.DialogResult.OK) {
                return;
            }
            try {
                LoadModel(openFileDialogModel.FileName);
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error loading model", MessageBoxButtons.OK);
            }
            CalculateSimStepGui();
        }

        private void openLayoutToolStripMenuItem_Click(object sender, EventArgs args) {
            // prompt the user for a texture image
            Debug.Assert(simInput != null && simInput.Array != null);
            Bitmap bitmap = arrayLayoutForm.PromptUserForLayoutTexture();
            if (bitmap == null) {
                return;
            }

            // apply the new texture, rollback if it fails
            Bitmap origTexture = simInput.Array.LayoutTexture;
            try {
                simInput.Array.LayoutTexture = bitmap;
                simInput.Array.ReadStringsFromColors();
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error loading layout texture", MessageBoxButtons.OK);
                simInput.Array.LayoutTexture = origTexture;
            }
            CalculateSimStepGui();
        }

        /// <summary>
        /// Called when one of the sim input GUIs changes.
        /// </summary>
        private void simInputs_Change(object sender, EventArgs e) {
            CalculateSimStepGui();
        }

        private void btnRecalc_Click(object sender, EventArgs e) {
            try {
                InitSimulator();
                ArraySimulationStepOutput simOutputNoon = simulator.Simulate(
                    simInput.Array, new Vector3(0.1f, 0.995f, 0.0f), simInput.Insolation, simInput.Temperature);
                ArraySimulationStepOutput simOutput = simulator.Simulate(simInput);
                double arrayAreaDistortion = Math.Abs(simOutputNoon.ArrayLitArea-simOutput.ArrayArea)/simOutput.ArrayArea;
                
                Debug.WriteLine("Array simulation output");
                Debug.WriteLine("   ... " + simOutput.ArrayArea + " m^2 nominal area, "
                    + simOutputNoon.ArrayLitArea + " m^2 simulated area" + (arrayAreaDistortion > 0.01 ? " MISMATCH" : ""));
                Debug.WriteLine("   ... " + simOutput.ArrayLitArea + " m^2 exposed to sunlight");
                Debug.WriteLine("   ... " + simOutput.WattsInsolation + " W insolation");
                Debug.WriteLine("   ... " + simOutput.WattsOutputByCell + " W output (assuming mppt per cell)");
                Debug.WriteLine("   ... " + simOutput.WattsOutput + " W output");

                //update ui
                String boldLine = string.Format("{0:0}W over {1:0.00}m\u00B2 cell area", 
                    simOutput.WattsOutput, simOutput.ArrayArea);
                String firstLine = string.Format(", {0:0.00}m\u00B2 lit cells{1}, {2:0.00}m\u00B2 shaded",
                    simOutputNoon.ArrayLitArea, arrayAreaDistortion>0.01 ? " (MISMATCH)":"", simOutputNoon.ArrayLitArea-simOutput.ArrayLitArea);
                String secondLine = string.Format("(Power breakdown: {0:0}W {1:0}% in, {2:0}W {3:0}% ideal mppt, {4:0}W {5:0}% output)",
                    simOutput.WattsInsolation, simOutput.WattsInsolation / simOutputNoon.WattsInsolation * 100,
                    simOutput.WattsOutputByCell, simOutput.WattsOutputByCell / simOutputNoon.WattsOutputByCell * 100,
                    simOutput.WattsOutput, simOutput.WattsOutput / simOutputNoon.WattsOutput * 100);
                this.labelArrPower.Rtf = @"{\rtf1\ansi\deff0 {\b "+boldLine+@"}"+firstLine
                    +@"\line "+ secondLine+"}";

                outputStringsListBox.Items.Clear();
                outputStringsListBox.Items.AddRange(simOutput.Strings);
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

        private void layoutTextureDimensionsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (arrayDimsForm != null && !arrayDimsForm.IsDisposed) {
                arrayDimsForm.BringToFront();
            } else {
                arrayDimsForm = new ArrayDimensionsForm();
                arrayDimsForm.Array = simInput.Array;
                arrayDimsForm.Show();
            }
        }

        private void outputStringsListBox_SelectedIndexChanged(object sender, EventArgs e) {
            ArraySimStringOutput output = (ArraySimStringOutput)outputStringsListBox.SelectedItem;
            if (output == null) return;

            // show details
            outputStringLabel.Text = "" + output.String;
            outputStringInsolationLabel.Text = string.Format("{0:0.0} W", 
                output.WattsIn);
            outputStringPowerLabel.Text = string.Format("{0:0.0} W ({1:0.0} %)", 
                output.WattsOutput, 100*output.WattsOutput/output.WattsOutputIdeal);
            outputStringPerfectMPPTLabel.Text = string.Format("{0:0.0} W ({1:0.0} %)",
                output.WattsOutputByCell, 100 * output.WattsOutputByCell / output.WattsOutputIdeal);
            outputStringFlattenedLabel.Text = string.Format("{0:0.0} W",
                output.WattsOutputIdeal);
            outputStringAreaLabel.Text = string.Format("{0:0.000} m^2",
                output.Area);
            outputStringShadedLabel.Text = string.Format("{0:0.000} m^2 ({1:0.0} %)",
                output.AreaShaded, 100*output.AreaShaded / output.Area);
            
            // show it on the layout
            outputArrayLayoutControl.CellString = output.String;
        }

        private void saveLayoutTextureToolStripMenuItem_Click(object sender, EventArgs e) {
            if(simInput.Array.LayoutTexture == null){
                MessageBox.Show("Nothing to save. Try opening and editing a layout first.");
                return;
            }
            DialogResult result = saveFileDialogLayout.ShowDialog();
            if (result != DialogResult.OK) return;
            simInput.Array.LayoutTexture.Save(saveFileDialogLayout.FileName);
        }

        private void outputStringIVLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ArraySimStringOutput output = (ArraySimStringOutput)outputStringsListBox.SelectedItem;
            if(output == null) {
                MessageBox.Show("No string selected.");
                return;
            }

            var form = new IVTraceForm();
            form.Label = output.String.ToString();
            form.IVTrace = output.IVTrace;
            form.Show();
        }

        private void bypassDiodeParametersToolStripMenuItem_Click(object sender, EventArgs e) {
            var form = new BypassDiodesForm();
            form.Spec = simInput.Array.BypassDiodeSpec;
            form.ShowDialog();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) {
            if (dateTimePicker2.Value <= dateTimePicker1.Value) {
                dateTimePicker2.Value = dateTimePicker1.Value.AddHours(1);
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e) {
            if (dateTimePicker2.Value <= dateTimePicker1.Value) {
                dateTimePicker1.Value = dateTimePicker2.Value.AddHours(-1);
            }
        }

        private void buttonRun_Click_1(object sender, EventArgs e) {
            TimeAveragedSim();
        }

        private void TimeAveragedSim() {
            // input time range; all other inputs come from simInput
            DateTime utcStart = dateTimePicker1.Value.Subtract(simInput.Timezone.GetUtcOffset(dateTimePicker1.Value));
            DateTime utcEnd = dateTimePicker2.Value.Subtract(simInput.Timezone.GetUtcOffset(dateTimePicker2.Value));

            // step-by-step output
            TextWriter csv = new StreamWriter("../../../../output.csv");
            csv.WriteLine("time_utc,insolation_w,output_w");

            // average output
            var simAvg = new ArraySimulationStepOutput();

            // simulate in 10-minute intervals
            InitSimulator();
            int nsim = 0;
            for (DateTime time = utcStart; time <= utcEnd; time = time.AddMinutes(10), nsim++) {
                simInput.Utc = time;
                simInputControls.UpdateView();
                ArraySimulationStepOutput simOutput = simulator.Simulate(simInput);

                // averate the outputs
                if (nsim > 0) {
                    Debug.Assert(simAvg.ArrayArea == simOutput.ArrayArea);
                }
                simAvg.ArrayArea = simOutput.ArrayArea;
                simAvg.ArrayLitArea += simOutput.ArrayLitArea;
                simAvg.WattsInsolation += simOutput.WattsInsolation;
                simAvg.WattsOutputByCell += simOutput.WattsOutputByCell;
                simAvg.WattsOutput += simOutput.WattsOutput;

                // debug output
                csv.WriteLine(time + "," + simOutput.WattsInsolation + "," + simOutput.WattsOutput);
            }
            csv.Close();

            // show the average output
            simAvg.ArrayLitArea /= nsim;
            simAvg.WattsInsolation /= nsim;
            simAvg.WattsOutputByCell /= nsim;
            simAvg.WattsOutput /= nsim;
            Debug.WriteLine("Array time-averaged simulation output");
            Debug.WriteLine("   ... " + simAvg.ArrayArea + " m^2 total cell area");
            Debug.WriteLine("   ... " + simAvg.ArrayLitArea + " m^2 exposed to sunlight");
            Debug.WriteLine("   ... " + simAvg.WattsInsolation + " W insolation");
            Debug.WriteLine("   ... " + simAvg.WattsOutputByCell + " W output (assuming mppt per cell)");
            Debug.WriteLine("   ... " + simAvg.WattsOutput + " W output");

        }
    }
}
