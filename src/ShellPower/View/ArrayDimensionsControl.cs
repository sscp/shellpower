using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace SSCP.ShellPower {
    public partial class ArrayDimensionsControl : Form {
        private RectangleF originalLayoutBounds;
        private bool updatingView = false;
        private ArraySpec array;
        public ArraySpec Array {
            get { 
                return array; 
            }
            set { 
                array = value;
                originalLayoutBounds = array.LayoutBoundsXZ;
                UpdateView(); 
            }
        }

        public ArrayDimensionsControl() {
            InitializeComponent();
            UpdateView();
        }

        private void UpdateView() {
            if (Array == null) {
                numX0.Enabled = numX1.Enabled = numZ0.Enabled = numZ1.Enabled = buttonOK.Enabled = false;
                return;
            }
            updatingView = true;
            numX0.Enabled = numX1.Enabled = numZ0.Enabled = numZ1.Enabled = buttonOK.Enabled = true;
            numX0.Value = (decimal)Array.LayoutBoundsXZ.X;
            numX1.Value = (decimal)Array.LayoutBoundsXZ.Width;
            numZ0.Value = (decimal)Array.LayoutBoundsXZ.Y;
            numZ1.Value = (decimal)Array.LayoutBoundsXZ.Height;
            updatingView = false;
        }

        private void UpdateModel() {
            if (Array == null) return;
            var b = Array.LayoutBoundsXZ;
            b.X = (float)numX0.Value;
            b.Y = (float)numZ0.Value;
            b.Width = (float)numX1.Value;
            b.Height = (float)numZ1.Value;
            Array.LayoutBoundsXZ = b;
        }

        private void numX0_ValueChanged(object sender, EventArgs e) {
            if(!updatingView) UpdateModel();
        }
        private void numX1_ValueChanged(object sender, EventArgs e) {
            if (!updatingView) UpdateModel();
        }
        private void numZ0_ValueChanged(object sender, EventArgs e) {
            if (!updatingView) UpdateModel();
        }
        private void numZ1_ValueChanged(object sender, EventArgs e) {
            if (!updatingView) UpdateModel();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            if (Array != null) {
                Array.LayoutBoundsXZ = originalLayoutBounds;
            }
            Close();
        }
    }
}
