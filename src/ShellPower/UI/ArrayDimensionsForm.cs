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
    public partial class ArrayDimensionsForm : Form {
        private BoundsSpec originalLayoutBounds;
        private bool updatingView = false;
        private ArraySpec array;
        public ArraySpec Array {
            get { 
                return array; 
            }
            set { 
                array = value;
                originalLayoutBounds = array.LayoutBounds;
                UpdateView(); 
            }
        }

        public ArrayDimensionsForm() {
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
            numX0.Value = (decimal)Array.LayoutBounds.MinX;
            numX1.Value = (decimal)Array.LayoutBounds.MaxX;
            numZ0.Value = (decimal)Array.LayoutBounds.MinZ;
            numZ1.Value = (decimal)Array.LayoutBounds.MaxZ;
            updatingView = false;
        }

        private void UpdateModel() {
            if (Array == null) return;
            Array.LayoutBounds.MinX = (double)numX0.Value;
            Array.LayoutBounds.MaxX = (double)numX1.Value;
            Array.LayoutBounds.MinZ = (double)numZ0.Value;
            Array.LayoutBounds.MaxZ = (double)numZ1.Value;
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
                Array.LayoutBounds = originalLayoutBounds;
            }
            Close();
        }
    }
}
