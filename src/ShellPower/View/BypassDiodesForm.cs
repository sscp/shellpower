using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    public partial class BypassDiodesForm : Form {
        private double fwdDrop;
        private DiodeSpec spec;
        public DiodeSpec Spec {
            get { return spec; }
            set { spec = value; UpdateView(); }
        }
        public BypassDiodesForm() {
            InitializeComponent();
        }

        private void UpdateView() {
            if (Spec == null) return;
            textBoxFwdDrop.Text = "" + Spec.VoltageDrop;
        }
        private bool ValidateEntries() {
            bool valid = true;
            valid &= ViewUtil.ValidateEntry(textBoxFwdDrop, out fwdDrop, 0.0, 10.0);
            return valid;
        }

        private void textBoxFwdDrop_TextChanged(object sender, EventArgs e) {
            ValidateEntries();
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            if (ValidateEntries()) {
                Spec.VoltageDrop = fwdDrop;
                Close();
            } else {
                MessageBox.Show("Some of those entries don't look right. Try again.");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
