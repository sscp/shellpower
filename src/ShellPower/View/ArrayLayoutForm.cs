using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public partial class ArrayLayoutForm : Form {
        private ArraySpec spec;

        public ArrayLayoutForm(ArraySpec spec) {
            Debug.Assert(spec != null);
            this.spec = spec;
            InitializeComponent();
            UpdateUI();
        }

        #region Public
        public Bitmap PromptUserForLayoutTexture() {
            DialogResult result = openFileDialogArray.ShowDialog();
            if (result != DialogResult.OK) return null;
            var texFile = openFileDialogArray.FileName;
            return new Bitmap(texFile);
        }
        #endregion

        #region Private
        private void UpdateUI() {
            // do we even have an array layout yet?
            bool hasLayout = (spec.LayoutTexture != null);
            arrayLayoutControl.Visible = hasLayout;
            panelNoLayoutTexture.Visible = !hasLayout;
            if (!hasLayout) {
                return;
            }
            // show the layout
            arrayLayoutControl.Array = this.spec;
            // TODO: array bounds
        }
        #endregion

        #region Events
        private void linkLabelLoadLayoutTexture_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Bitmap bmp = PromptUserForLayoutTexture();
            if (bmp != null) {
                spec.LayoutTexture = bmp;
                UpdateUI();
            }
        }

        private void buttonRelabel_Click(object sender, EventArgs e) {
            Debug.WriteLine("relabel");
        }
        private void buttonMakeString_Click(object sender, EventArgs e) {
            Debug.WriteLine("make string");
        }
        private void listViewStrings_SelectedIndexChanged(object sender, EventArgs e) {
        }
        #endregion
    }
}
