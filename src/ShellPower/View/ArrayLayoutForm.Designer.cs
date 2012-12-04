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
            this.listViewStrings = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.labelExplain = new System.Windows.Forms.Label();
            this.buttonMakeString = new System.Windows.Forms.Button();
            this.panelNoLayoutTexture = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabelLoadLayoutTexture = new System.Windows.Forms.LinkLabel();
            this.labelNoLayout = new System.Windows.Forms.Label();
            this.arrayLayoutControl = new SSCP.ShellPower.ArrayLayoutControl();
            this.openFileDialogArray = new System.Windows.Forms.OpenFileDialog();
            this.directorySearcher1 = new System.DirectoryServices.DirectorySearcher();
            this.labelMakeString = new System.Windows.Forms.Label();
            this.buttonDeleteString = new System.Windows.Forms.Button();
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
            this.splitContainer1.Panel1.Controls.Add(this.listViewStrings);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.buttonDeleteString);
            this.splitContainer1.Panel2.Controls.Add(this.labelMakeString);
            this.splitContainer1.Panel2.Controls.Add(this.labelExplain);
            this.splitContainer1.Panel2.Controls.Add(this.buttonMakeString);
            this.splitContainer1.Panel2.Controls.Add(this.panelNoLayoutTexture);
            this.splitContainer1.Panel2.Controls.Add(this.arrayLayoutControl);
            this.splitContainer1.Size = new System.Drawing.Size(578, 395);
            this.splitContainer1.SplitterDistance = 192;
            this.splitContainer1.TabIndex = 0;
            // 
            // listViewStrings
            // 
            this.listViewStrings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewStrings.Location = new System.Drawing.Point(0, 25);
            this.listViewStrings.Name = "listViewStrings";
            this.listViewStrings.Size = new System.Drawing.Size(193, 367);
            this.listViewStrings.TabIndex = 2;
            this.listViewStrings.UseCompatibleStateImageBehavior = false;
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
            // buttonMakeString
            // 
            this.buttonMakeString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonMakeString.Location = new System.Drawing.Point(6, 326);
            this.buttonMakeString.Name = "buttonMakeString";
            this.buttonMakeString.Size = new System.Drawing.Size(103, 23);
            this.buttonMakeString.TabIndex = 7;
            this.buttonMakeString.Text = "Create string";
            this.buttonMakeString.UseVisualStyleBackColor = true;
            this.buttonMakeString.Click += new System.EventHandler(this.buttonMakeString_Click);
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
            this.arrayLayoutControl.Array = null;
            this.arrayLayoutControl.Location = new System.Drawing.Point(3, 3);
            this.arrayLayoutControl.Name = "arrayLayoutControl";
            this.arrayLayoutControl.Size = new System.Drawing.Size(379, 317);
            this.arrayLayoutControl.TabIndex = 9;
            // 
            // openFileDialogArray
            // 
            this.openFileDialogArray.Filter = "PNG Files|*.png|JPG Files|*.jpg|Bitmap files|*.bmp";
            this.openFileDialogArray.Title = "Load array texture (cell layout)";
            // 
            // directorySearcher1
            // 
            this.directorySearcher1.ClientTimeout = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerPageTimeLimit = System.TimeSpan.Parse("-00:00:01");
            this.directorySearcher1.ServerTimeLimit = System.TimeSpan.Parse("-00:00:01");
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
            // buttonDeleteString
            // 
            this.buttonDeleteString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteString.Location = new System.Drawing.Point(115, 326);
            this.buttonDeleteString.Name = "buttonDeleteString";
            this.buttonDeleteString.Size = new System.Drawing.Size(103, 23);
            this.buttonDeleteString.TabIndex = 11;
            this.buttonDeleteString.Text = "Delete string";
            this.buttonDeleteString.UseVisualStyleBackColor = true;
            // 
            // ArrayLayoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 395);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ArrayLayoutForm";
            this.Text = "Solar Array Layout";
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
        private System.Windows.Forms.ListView listViewStrings;
        private System.Windows.Forms.Label labelExplain;
        private System.Windows.Forms.Button buttonMakeString;
        private System.Windows.Forms.OpenFileDialog openFileDialogArray;
        private System.DirectoryServices.DirectorySearcher directorySearcher1;
        private SSCP.ShellPower.ArrayLayoutControl arrayLayoutControl;
        private System.Windows.Forms.Button buttonDeleteString;
        private System.Windows.Forms.Label labelMakeString;
    }
}