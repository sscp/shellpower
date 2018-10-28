namespace SSCP.ShellPower {
    partial class ArrayLayoutForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.listViewStrings = new SSCP.ShellPower.RefreshListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxEditDiodes = new System.Windows.Forms.CheckBox();
            this.buttonCreateString = new System.Windows.Forms.Button();
            this.buttonDeleteString = new System.Windows.Forms.Button();
            this.labelMakeString = new System.Windows.Forms.Label();
            this.labelExplain = new System.Windows.Forms.Label();
            this.buttonEdit = new System.Windows.Forms.Button();
            this.panelNoLayoutTexture = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabelLoadLayoutTexture = new System.Windows.Forms.LinkLabel();
            this.labelNoLayout = new System.Windows.Forms.Label();
            this.arrayLayoutControl = new SSCP.ShellPower.ArrayLayoutControl();
            this.openFileDialogArray = new System.Windows.Forms.OpenFileDialog();
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelNoLayoutTexture.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.buttonCancel);
            this.splitContainer1.Panel1.Controls.Add(this.buttonOK);
            this.splitContainer1.Panel1.Controls.Add(this.listViewStrings);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.checkBoxEditDiodes);
            this.splitContainer1.Panel2.Controls.Add(this.buttonCreateString);
            this.splitContainer1.Panel2.Controls.Add(this.buttonDeleteString);
            this.splitContainer1.Panel2.Controls.Add(this.labelMakeString);
            this.splitContainer1.Panel2.Controls.Add(this.labelExplain);
            this.splitContainer1.Panel2.Controls.Add(this.buttonEdit);
            this.splitContainer1.Panel2.Controls.Add(this.panelNoLayoutTexture);
            this.splitContainer1.Panel2.Controls.Add(this.arrayLayoutControl);
            this.splitContainer1.Size = new System.Drawing.Size(578, 395);
            this.splitContainer1.SplitterDistance = 192;
            this.splitContainer1.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(89, 326);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(71, 23);
            this.buttonCancel.TabIndex = 14;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Visible = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonOK.Location = new System.Drawing.Point(12, 326);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(71, 23);
            this.buttonOK.TabIndex = 13;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // listViewStrings
            // 
            this.listViewStrings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewStrings.FormattingEnabled = true;
            this.listViewStrings.Location = new System.Drawing.Point(12, 27);
            this.listViewStrings.Name = "listViewStrings";
            this.listViewStrings.Size = new System.Drawing.Size(177, 290);
            this.listViewStrings.TabIndex = 1;
            this.listViewStrings.SelectedIndexChanged += new System.EventHandler(this.listViewStrings_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Strings";
            // 
            // checkBoxEditDiodes
            // 
            this.checkBoxEditDiodes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxEditDiodes.AutoSize = true;
            this.checkBoxEditDiodes.Location = new System.Drawing.Point(18, 323);
            this.checkBoxEditDiodes.Name = "checkBoxEditDiodes";
            this.checkBoxEditDiodes.Size = new System.Drawing.Size(114, 17);
            this.checkBoxEditDiodes.TabIndex = 13;
            this.checkBoxEditDiodes.Text = "Edit bypass diodes";
            this.checkBoxEditDiodes.UseVisualStyleBackColor = true;
            this.checkBoxEditDiodes.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonCreateString
            // 
            this.buttonCreateString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCreateString.Location = new System.Drawing.Point(118, 294);
            this.buttonCreateString.Name = "buttonCreateString";
            this.buttonCreateString.Size = new System.Drawing.Size(103, 23);
            this.buttonCreateString.TabIndex = 12;
            this.buttonCreateString.Text = "Create string";
            this.buttonCreateString.UseVisualStyleBackColor = true;
            this.buttonCreateString.Click += new System.EventHandler(this.buttonCreateString_Click);
            // 
            // buttonDeleteString
            // 
            this.buttonDeleteString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteString.Location = new System.Drawing.Point(227, 294);
            this.buttonDeleteString.Name = "buttonDeleteString";
            this.buttonDeleteString.Size = new System.Drawing.Size(103, 23);
            this.buttonDeleteString.TabIndex = 11;
            this.buttonDeleteString.Text = "Delete string";
            this.buttonDeleteString.UseVisualStyleBackColor = true;
            this.buttonDeleteString.Click += new System.EventHandler(this.buttonDeleteString_Click);
            // 
            // labelMakeString
            // 
            this.labelMakeString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelMakeString.AutoSize = true;
            this.labelMakeString.BackColor = System.Drawing.SystemColors.Control;
            this.labelMakeString.Location = new System.Drawing.Point(6, 352);
            this.labelMakeString.Name = "labelMakeString";
            this.labelMakeString.Size = new System.Drawing.Size(243, 13);
            this.labelMakeString.TabIndex = 10;
            this.labelMakeString.Text = "Click on each cell, in the order that they are wired.";
            // 
            // labelExplain
            // 
            this.labelExplain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelExplain.AutoSize = true;
            this.labelExplain.Location = new System.Drawing.Point(3, 352);
            this.labelExplain.Name = "labelExplain";
            this.labelExplain.Size = new System.Drawing.Size(260, 26);
            this.labelExplain.TabIndex = 8;
            this.labelExplain.Text = "A string is a list of cells wired in series.\r\nEach string should correspond to on" +
                "e MPPT channel.";
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonEdit.Location = new System.Drawing.Point(9, 294);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(103, 23);
            this.buttonEdit.TabIndex = 7;
            this.buttonEdit.Text = "Edit string";
            this.buttonEdit.UseVisualStyleBackColor = true;
            this.buttonEdit.Click += new System.EventHandler(this.buttonEditString_Click);
            // 
            // panelNoLayoutTexture
            // 
            this.panelNoLayoutTexture.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelNoLayoutTexture.Controls.Add(this.label3);
            this.panelNoLayoutTexture.Controls.Add(this.linkLabelLoadLayoutTexture);
            this.panelNoLayoutTexture.Controls.Add(this.labelNoLayout);
            this.panelNoLayoutTexture.Location = new System.Drawing.Point(128, 109);
            this.panelNoLayoutTexture.Name = "panelNoLayoutTexture";
            this.panelNoLayoutTexture.Size = new System.Drawing.Size(146, 103);
            this.panelNoLayoutTexture.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 39);
            this.label3.TabIndex = 4;
            this.label3.Text = "Top-down ortho projection, \r\none flat color per cell, \r\nflat white background";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabelLoadLayoutTexture
            // 
            this.linkLabelLoadLayoutTexture.AutoSize = true;
            this.linkLabelLoadLayoutTexture.Location = new System.Drawing.Point(36, 31);
            this.linkLabelLoadLayoutTexture.Name = "linkLabelLoadLayoutTexture";
            this.linkLabelLoadLayoutTexture.Size = new System.Drawing.Size(66, 13);
            this.linkLabelLoadLayoutTexture.TabIndex = 3;
            this.linkLabelLoadLayoutTexture.TabStop = true;
            this.linkLabelLoadLayoutTexture.Text = "Load texture";
            this.linkLabelLoadLayoutTexture.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLoadLayoutTexture_LinkClicked);
            // 
            // labelNoLayout
            // 
            this.labelNoLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelNoLayout.AutoSize = true;
            this.labelNoLayout.Location = new System.Drawing.Point(3, 0);
            this.labelNoLayout.Name = "labelNoLayout";
            this.labelNoLayout.Size = new System.Drawing.Size(131, 13);
            this.labelNoLayout.TabIndex = 2;
            this.labelNoLayout.Text = "  No layout texture loaded.";
            // 
            // arrayLayoutControl
            // 
            this.arrayLayoutControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.arrayLayoutControl.AnimatedSelection = true;
            this.arrayLayoutControl.Array = null;
            this.arrayLayoutControl.CellString = null;
            this.arrayLayoutControl.Editable = false;
            this.arrayLayoutControl.EditBypassDiodes = false;
            this.arrayLayoutControl.Location = new System.Drawing.Point(3, 3);
            this.arrayLayoutControl.Name = "arrayLayoutControl";
            this.arrayLayoutControl.Size = new System.Drawing.Size(379, 285);
            this.arrayLayoutControl.TabIndex = 9;
            this.arrayLayoutControl.CellStringChanged += new System.EventHandler(this.arrayLayoutControl_CellStringChanged);
            // 
            // openFileDialogArray
            // 
            this.openFileDialogArray.Filter = "PNG Files|*.png|GIF Files|*.gif|Bitmap files|*.bmp|All files|*.*";
            this.openFileDialogArray.FilterIndex = 4;
            this.openFileDialogArray.Title = "Load array texture (cell layout)";
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
            // 
            // ArrayLayoutForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(578, 395);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ArrayLayoutForm";
            this.Text = "Solar Array Layout";
            this.Load += new System.EventHandler(this.ArrayLayoutForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelNoLayoutTexture.ResumeLayout(false);
            this.panelNoLayoutTexture.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelNoLayout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelNoLayoutTexture;
        private System.Windows.Forms.LinkLabel linkLabelLoadLayoutTexture;
        private System.Windows.Forms.Label labelExplain;
        private System.Windows.Forms.Button buttonEdit;
        private System.Windows.Forms.OpenFileDialog openFileDialogArray;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private SSCP.ShellPower.ArrayLayoutControl arrayLayoutControl;
        private System.Windows.Forms.Button buttonDeleteString;
        private System.Windows.Forms.Label labelMakeString;
        private System.Windows.Forms.Button buttonCreateString;
        private RefreshListBox listViewStrings;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxEditDiodes;
    }
}