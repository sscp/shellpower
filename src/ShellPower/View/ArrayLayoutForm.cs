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
        private ArraySpec.CellString editingString;

        public ArrayLayoutForm(ArraySpec spec) {
            Debug.Assert(spec != null);
            this.spec = spec;
            InitializeComponent();
            arrayLayoutControl.SelectionChanged += new EventHandler(arrayLayoutControl_SelectionChanged);
            UpdateUI();
        }

        void arrayLayoutControl_SelectionChanged(object sender, EventArgs e) {
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

            // show the strings
            listViewStrings.Clear();
            foreach (ArraySpec.CellString cellStr in spec.Strings) {
                string cellStrStr = string.Join(",", cellStr.CellIDs);
                listViewStrings.Items.Add(new ListViewItem(cellStrStr));
            }
            if (editingString != null) {
                Debug.Assert(spec.Strings[spec.Strings.Count - 1] == editingString);
                listViewStrings.SelectedIndices.Clear();
                listViewStrings.SelectedIndices.Add(spec.Strings.Count - 1);
                listViewStrings.Enabled = false;
            } else {
                listViewStrings.Enabled = true;
            }
        }

        private void UpdateControls() {
            if (editingString == null) {
                buttonMakeString.Text = "Make string";
                labelMakeString.Visible = true;
                labelExplain.Visible = false;
            } else {
                buttonMakeString.Text = "Done";
                labelMakeString.Visible = false;
                labelExplain.Visible = true;
            }
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

        private void buttonMakeString_Click(object sender, EventArgs e) {
            if (editingString != null) {
                // create a new string, which we'll add cells to one at a time
                editingString = new ArraySpec.CellString();

            } else {
                editingString = null;
            }

            // update the view
            UpdateControls();
            UpdateUI();
        }
        private void listViewStrings_SelectedIndexChanged(object sender, EventArgs e) {
        }
        #endregion
    }
}
