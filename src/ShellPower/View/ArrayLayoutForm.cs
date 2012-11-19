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
            // TODO: array bounds

            // show the strings
            listViewStrings.Clear();
            foreach (ArraySpec.CellString cellStr in spec.Strings) {
                string cellStrStr = string.Join(",", cellStr.CellIDs);
                listViewStrings.Items.Add(new ListViewItem(cellStrStr));
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

        private void buttonRelabel_Click(object sender, EventArgs e) {
            Debug.WriteLine("relabel");
        }
        private void buttonMakeString_Click(object sender, EventArgs e) {
            HashSet<String> cellIds = arrayLayoutControl.SelectedIDs;
            Debug.WriteLine("make string: " + string.Join(",", cellIds));

            // remove from existing strings
            List<ArraySpec.CellString> newStrings = new List<ArraySpec.CellString>();
            foreach (ArraySpec.CellString cellStr in spec.Strings) {
                List<string> stringIds = cellStr.CellIDs;
                for (int i = stringIds.Count - 1; i >= 0; i--) {
                    if (cellIds.Contains(stringIds[i])) {
                        stringIds.RemoveAt(i);
                        if (i < stringIds.Count) {
                            i++;
                        }
                    }
                }
                if (cellStr.CellIDs.Count > 0) {
                    newStrings.Add(cellStr);
                }
            }
            spec.Strings.Clear();
            spec.Strings.AddRange(newStrings);

            // create the new string
            ArraySpec.CellString newString = new ArraySpec.CellString();
            newString.CellIDs.AddRange(cellIds);
            spec.Strings.Add(newString);

            // update the view
            UpdateUI();
        }
        private void listViewStrings_SelectedIndexChanged(object sender, EventArgs e) {
        }
        #endregion
    }
}
