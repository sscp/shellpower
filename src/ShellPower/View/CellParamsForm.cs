using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    public partial class CellParamsForm : Form {
        private ArraySimulationStepInput input;
        private double voc, isc, dvocdt, discdt;
        private double nideal, seriesr;
        private double area;

        private double tempC, wattsIn;

        public CellParamsForm(ArraySimulationStepInput input) {
            this.input = input;
            InitializeComponent();
            ResetTextBoxes();
        }

        private void ResetTextBoxes() {
            var cellSpec = input.Array.CellSpec;

            textBoxVoc.Text = "" + cellSpec.VocStc;
            textBoxIsc.Text = "" + cellSpec.IscStc;
            textBoxVocTemp.Text = "" + cellSpec.DVocDT;
            textBoxIscTemp.Text = "" + cellSpec.DIscDT;
            textBoxArea.Text = "" + cellSpec.Area;
            textBoxNIdeal.Text = "" + cellSpec.NIdeal;
            textBoxSeriesR.Text = "" + cellSpec.SeriesR;

            textBoxTemp.Text = "" + input.Temperature;
            textBoxInsolation.Text = "" + input.Insolation;
        }

        private bool ValidateEntry(TextBox textBox, out double val, double min, double max) {
            if (!double.TryParse(textBox.Text, out val) || val < min || val > max) {
                textBox.BackColor = Color.FromArgb(0xff, 0xff, 0xbb, 0xaa);
                return false;
            } else {
                textBox.BackColor = Color.White;
                return true;
            }
        }

        private bool ValidateEntries() {
            bool valid = true;
            valid &= ValidateEntry(textBoxVoc, out voc, double.Epsilon, 100);
            valid &= ValidateEntry(textBoxIsc, out isc, double.Epsilon, 100);
            valid &= ValidateEntry(textBoxVocTemp, out dvocdt, -10, 10);
            valid &= ValidateEntry(textBoxIscTemp, out discdt, -10, 10);
            valid &= ValidateEntry(textBoxArea, out area, 0.0, 1.0);
            valid &= ValidateEntry(textBoxNIdeal, out nideal, 1.0, 10.0);
            valid &= ValidateEntry(textBoxSeriesR, out seriesr, 0.0, 0.1);

            valid &= ValidateEntry(textBoxTemp, out tempC, -Constants.C_IN_KELVIN, 1000.0);
            valid &= ValidateEntry(textBoxInsolation, out wattsIn, 0, 1600);
            return valid;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            if (ValidateEntries()) {
                UpdateSpec(input.Array.CellSpec);
                input.Temperature = tempC;
                input.Insolation = wattsIn;
                Close();
            } else {
                MessageBox.Show("Some of those entries don't look right. Try again.");
            }
        }

        private void UpdateSpec(CellSpec spec) {
            spec.VocStc = voc;
            spec.IscStc = isc;
            spec.DVocDT = dvocdt;
            spec.DIscDT = discdt;
            spec.Area = area;
            spec.NIdeal = nideal;
            spec.SeriesR = seriesr;
        }

        private void Recalculate() {
            // recalc
            CellSpec spec = new CellSpec();
            UpdateSpec(spec);

            // show sanity checks
            double i0 = spec.CalcI0(wattsIn, tempC);
            double isc = spec.CalcIsc(wattsIn, tempC);
            double voc = spec.CalcVoc(wattsIn, tempC);
            double ff, vmp, imp;
            double[] veci, vecv;
            spec.CalcSweep(wattsIn, tempC, out ff, out vmp, out imp, out veci, out vecv);
            labelMaxPower.Text = string.Format(
                "Isc={0:0.000}A Voc={1:0.000}V @{2:0.00}C\n" +
                "Imp={3:0.000}A Vmp={4:0.000}V Pmp={5:0.000}W\n" +
                "Rev. sat. current {6:0.000}A, fill factor {7:0.0}%",
                isc, voc, tempC,
                imp, vmp, imp * vmp,
                i0, ff * 100.0);

            // show an iv plot
            chartIV.X = vecv;
            chartIV.Y = veci;
        }

        private void textBox_TextChanged(object sender, EventArgs e) {
            if (ValidateEntries()) {
                Recalculate();
            }
        }

        private void CellParamsForm_Load(object sender, EventArgs e) {
            ResetTextBoxes();
        }
    }
}
