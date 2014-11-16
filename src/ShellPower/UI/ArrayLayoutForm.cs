using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public partial class ArrayLayoutForm : Form {
        private ArraySpec array;

        public ArrayLayoutForm(ArraySpec spec) {
            Debug.Assert(spec != null);
            this.array = spec;

            InitializeComponent();
            UpdateView();
        }

        #region Public
        public Bitmap PromptUserForLayoutTexture() {
            DialogResult result = openFileDialogArray.ShowDialog();
            if (result != DialogResult.OK) return null;
            var texFile = openFileDialogArray.FileName;
            try {
                return new Bitmap(texFile);
            } catch {
                MessageBox.Show("Could not open bitmap " + texFile + ". Is it open in another program? Is it a valid bitmap?");
                return null;
            }
        }
        #endregion

        #region Private
        private void UpdateView() {
            UpdateArrayLayout();
            UpdateStrings();
            UpdateControls();
        }
        private void UpdateArrayLayout() {
            // do we even have an array layout yet?
            bool hasLayout = (array.LayoutTexture != null);
            arrayLayoutControl.Visible = hasLayout;
            panelNoLayoutTexture.Visible = !hasLayout;
            if (!hasLayout) {
                return;
            }

            // show the layout
            arrayLayoutControl.Array = this.array;

            arrayLayoutControl.EditBypassDiodes = checkBoxEditDiodes.Checked;
        }
        private void UpdateStrings() {
            // show the strings
            for (int i = listViewStrings.Items.Count - 1; i >= 0; i--) {
                if (!array.Strings.Contains(listViewStrings.Items[i])) {
                    listViewStrings.Items.RemoveAt(i);
                }
            }
            int ix = 0;
            foreach(ArraySpec.CellString cellStr in array.Strings){
                if (ix == listViewStrings.Items.Count) {
                    listViewStrings.Items.Add(cellStr);
                } else if(listViewStrings.Items[ix] != cellStr){
                    listViewStrings.Items.Insert(ix, cellStr);
                    ix++;
                }
                ix++;
            }
            Debug.Assert(array.Strings.Count == listViewStrings.Items.Count);
            for (int i = 0; i < array.Strings.Count; i++) {
                Debug.Assert(array.Strings[i] == listViewStrings.Items[i]);
                array.Strings[i].Name = "String " + (i + 1);
            }
            listViewStrings.RefreshItems();
        }
        private void UpdateControls() {
            // update buttons
            if (arrayLayoutControl.Editable && listViewStrings.SelectedItem != null) {
                buttonEdit.Text = "Done";
                buttonEdit.Enabled = true;
                buttonCreateString.Enabled = false;
                buttonDeleteString.Enabled = false;
                labelMakeString.Visible = false;
                labelExplain.Visible = true;
                checkBoxEditDiodes.Visible = true;
                arrayLayoutControl.Editable = true;

                listViewStrings.Enabled = false;
            } else {
                buttonEdit.Text = "Edit";
                buttonEdit.Enabled = listViewStrings.SelectedItem != null;
                buttonCreateString.Enabled = true;
                buttonDeleteString.Enabled = listViewStrings.SelectedItem != null;
                labelMakeString.Visible = true;
                labelExplain.Visible = false;
                checkBoxEditDiodes.Visible = false;
                checkBoxEditDiodes.Checked = false;
                arrayLayoutControl.Editable = false;
                listViewStrings.Enabled = true;
            }
        }
        #endregion

        #region Events
        private void linkLabelLoadLayoutTexture_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Bitmap bmp = PromptUserForLayoutTexture();
            if (bmp != null) {
                array.LayoutTexture = bmp;
                array.ReadStringsFromColors();
                UpdateView();
            }
        }
        private void buttonEditString_Click(object sender, EventArgs args) {
            arrayLayoutControl.Editable = !arrayLayoutControl.Editable;
            if (!arrayLayoutControl.Editable) {
                // just clicked "Done"
                var editedStr = arrayLayoutControl.CellString;
                array.Strings.RemoveAll((cellStr) => {
                    if (cellStr != editedStr) {
                        cellStr.Cells.RemoveAll((cell) => {
                            return editedStr.Cells.Contains(cell);
                        });
                    }
                    return cellStr.Cells.Count == 0;
                });
            }
            UpdateView();
        }
        private bool selectionChanging = false;
        private void listViewStrings_SelectedIndexChanged(object sender, EventArgs e) {
            if (selectionChanging) return;
            selectionChanging = true;
            arrayLayoutControl.CellString = (ArraySpec.CellString)listViewStrings.SelectedItem;
            UpdateView();
            selectionChanging = false;
        }
        private void buttonCreateString_Click(object sender, EventArgs e) {
            var newString = new ArraySpec.CellString();
            array.Strings.Add(newString);
            arrayLayoutControl.CellString = newString;
            arrayLayoutControl.Editable = true;
            arrayLayoutControl.AnimatedSelection = true;
            listViewStrings.Items.Add(newString);
            listViewStrings.SelectedItem = newString; // calls UpdateView()
        }
        private void buttonDeleteString_Click(object sender, EventArgs e) {
            array.Strings.Remove((ArraySpec.CellString)listViewStrings.SelectedItem);
            UpdateView();
        }
        private void arrayLayoutControl_CellStringChanged(object sender, EventArgs e) {
            listViewStrings.RefreshItems();
        }
        #endregion

        private void buttonOK_Click(object sender, EventArgs e) {
            if (array.LayoutTexture != null) {
                array.Recolor();
            }
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            //TODO: support cancel
        }

        private void ArrayLayoutForm_Load(object sender, EventArgs e) {
            UpdateView();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            UpdateView();
        }
    }
}
