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
        private ArraySpec array;
        private double voc, isc, dvocdt, discdt;
        private double nideal, seriesr;
        private double area;
        private double tempC;

        public CellParamsForm(ArraySpec spec) {
            this.array = spec;
            InitializeComponent();
            ResetTextBoxes();
        }

        public ArraySpec Array {
            get {
                return array;
            }
        }

        private void ResetTextBoxes() {
            textBoxVoc.Text = "" + array.CellSpec.VocStc;
            textBoxIsc.Text = "" + array.CellSpec.IscStc;
            textBoxVocTemp.Text = "" + array.CellSpec.DVocDT;
            textBoxIscTemp.Text = "" + array.CellSpec.DIscDT;
            textBoxArea.Text = "" + array.CellSpec.Area;
            textBoxTemp.Text = "" + array.CellSpec.Temperature;
            textBoxNIdeal.Text = "" + array.CellSpec.NIdeal;
            textBoxSeriesR.Text = "" + array.CellSpec.SeriesR;
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
            valid &= ValidateEntry(textBoxTemp, out tempC, -Constants.C_IN_KELVIN, 1000.0);
            valid &= ValidateEntry(textBoxNIdeal, out nideal, 1.0, 10.0);
            valid &= ValidateEntry(textBoxSeriesR, out seriesr, 0.0, 0.1);
            return valid;
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            if (ValidateEntries()) {
                UpdateSpec(array.CellSpec);
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
            spec.Temperature = tempC;
            spec.NIdeal = nideal;
            spec.SeriesR = seriesr;
        }

        private void Recalculate(){
            // recalc
            CellSpec spec = new CellSpec();
            UpdateSpec(spec);

            // show sanity checks
            double wattsIn = CellSpec.STC_INSOLATION;
            double i0 = spec.CalcI0(wattsIn);
            double isc = spec.CalcIsc(wattsIn);
            double voc = spec.CalcVoc(wattsIn);
            double ff, vmp, imp;
            double[] veci, vecv;
            spec.CalcSweep(wattsIn, out ff, out vmp, out imp, out veci, out vecv);
            labelMaxPower.Text = string.Format(
                "Isc={0:0.000}A Voc={1:0.000}V @{2:0.00}C\n" +
                "Imp={3:0.000}A Vmp={4:0.000}V Pmp={5:0.000}W\n"+
                "Rev. sat. current {6:0.000}A, fill factor {7:0.0}%",
                isc, voc, tempC, 
                imp, vmp, imp*vmp,
                i0, ff*100.0);

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
