namespace SSCP.ShellPower {
    partial class MainForm {
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLayoutTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutTextureDimensionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bypassDiodeParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogModel = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogLayout = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageRun = new System.Windows.Forms.TabPage();
            this.simInputControls = new SSCP.ShellPower.ArraySimInputControl();
            this.btnRecalc = new System.Windows.Forms.Button();
            this.labelArrPower = new System.Windows.Forms.RichTextBox();
            this.labelArrayPower = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPageOutputs = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.outputStringFlattenedLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.outputStringsListBox = new System.Windows.Forms.ListBox();
            this.outputStringPerfectMPPTLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.outputStringInsolationLabel = new System.Windows.Forms.Label();
            this.outputStringLabel = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.outputStringIVLinkLabel = new System.Windows.Forms.LinkLabel();
            this.outputStringPowerLabel = new System.Windows.Forms.Label();
            this.outputStringShadedLabel = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.outputStringAreaLabel = new System.Windows.Forms.Label();
            this.outputArrayLayoutControl = new SSCP.ShellPower.ArrayLayoutControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBoxSimulation = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label18 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.labelSimTotalEnergy = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelSimAvgEfficiency = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.labelSimAvgPower = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.glControl = new SSCP.ShellPower.ArrayModelControl();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageRun.SuspendLayout();
            this.tabPageOutputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBoxSimulation.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 540);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(984, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openModelToolStripMenuItem,
            this.openLayoutToolStripMenuItem,
            this.saveLayoutTextureToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openModelToolStripMenuItem
            // 
            this.openModelToolStripMenuItem.Name = "openModelToolStripMenuItem";
            this.openModelToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.openModelToolStripMenuItem.Text = "Open model";
            this.openModelToolStripMenuItem.ToolTipText = "Open a 3D mesh file that defines the shape of the array.";
            this.openModelToolStripMenuItem.Click += new System.EventHandler(this.openModelToolStripMenuItem_Click);
            // 
            // openLayoutToolStripMenuItem
            // 
            this.openLayoutToolStripMenuItem.Name = "openLayoutToolStripMenuItem";
            this.openLayoutToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.openLayoutToolStripMenuItem.Text = "Open layout texture";
            this.openLayoutToolStripMenuItem.ToolTipText = "Open a texture (image file, lossless compression like GIF or PNG, no antialiasing" +
                ") that defines where each cell is on your array.";
            this.openLayoutToolStripMenuItem.Click += new System.EventHandler(this.openLayoutToolStripMenuItem_Click);
            // 
            // saveLayoutTextureToolStripMenuItem
            // 
            this.saveLayoutTextureToolStripMenuItem.Name = "saveLayoutTextureToolStripMenuItem";
            this.saveLayoutTextureToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.saveLayoutTextureToolStripMenuItem.Text = "Save layout texture";
            this.saveLayoutTextureToolStripMenuItem.Click += new System.EventHandler(this.saveLayoutTextureToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layoutToolStripMenuItem,
            this.layoutTextureDimensionsToolStripMenuItem,
            this.cellParametersToolStripMenuItem,
            this.bypassDiodeParametersToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // layoutToolStripMenuItem
            // 
            this.layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
            this.layoutToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.layoutToolStripMenuItem.Text = "Layout";
            this.layoutToolStripMenuItem.ToolTipText = "Define how the individual cells are wired.";
            this.layoutToolStripMenuItem.Click += new System.EventHandler(this.layoutToolStripMenuItem_Click);
            // 
            // layoutTextureDimensionsToolStripMenuItem
            // 
            this.layoutTextureDimensionsToolStripMenuItem.Name = "layoutTextureDimensionsToolStripMenuItem";
            this.layoutTextureDimensionsToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.layoutTextureDimensionsToolStripMenuItem.Text = "Layout texture dimensions";
            this.layoutTextureDimensionsToolStripMenuItem.ToolTipText = "Define which part of the model is covered by the layout texture image--in other w" +
                "ords, where the cells are.";
            this.layoutTextureDimensionsToolStripMenuItem.Click += new System.EventHandler(this.layoutTextureDimensionsToolStripMenuItem_Click);
            // 
            // cellParametersToolStripMenuItem
            // 
            this.cellParametersToolStripMenuItem.Name = "cellParametersToolStripMenuItem";
            this.cellParametersToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.cellParametersToolStripMenuItem.Text = "Cell parameters";
            this.cellParametersToolStripMenuItem.ToolTipText = "Define the characteristics of the indiviudual cell. Efficiency, fill factor, etc." +
                "";
            this.cellParametersToolStripMenuItem.Click += new System.EventHandler(this.cellParametersToolStripMenuItem_Click);
            // 
            // bypassDiodeParametersToolStripMenuItem
            // 
            this.bypassDiodeParametersToolStripMenuItem.Name = "bypassDiodeParametersToolStripMenuItem";
            this.bypassDiodeParametersToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.bypassDiodeParametersToolStripMenuItem.Text = "Bypass diode parameters";
            this.bypassDiodeParametersToolStripMenuItem.Click += new System.EventHandler(this.bypassDiodeParametersToolStripMenuItem_Click);
            // 
            // openFileDialogModel
            // 
            this.openFileDialogModel.Filter = "3DXML files|*.3dxml|STL Files (ascii)|*.stl|All files|*.*";
            this.openFileDialogModel.Title = "Load solar car model";
            // 
            // saveFileDialogLayout
            // 
            this.saveFileDialogLayout.Filter = "PNG Images|*.png|GIF Images|*.gif|Bitmap images|*.bmp";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.glControl);
            this.splitContainer1.Size = new System.Drawing.Size(984, 516);
            this.splitContainer1.SplitterDistance = 383;
            this.splitContainer1.TabIndex = 2;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageRun);
            this.tabControl1.Controls.Add(this.tabPageOutputs);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 1;
            this.tabControl1.Size = new System.Drawing.Size(383, 516);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPageRun
            // 
            this.tabPageRun.Controls.Add(this.simInputControls);
            this.tabPageRun.Controls.Add(this.btnRecalc);
            this.tabPageRun.Controls.Add(this.labelArrPower);
            this.tabPageRun.Controls.Add(this.labelArrayPower);
            this.tabPageRun.Controls.Add(this.label5);
            this.tabPageRun.Location = new System.Drawing.Point(4, 22);
            this.tabPageRun.Name = "tabPageRun";
            this.tabPageRun.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRun.Size = new System.Drawing.Size(375, 490);
            this.tabPageRun.TabIndex = 0;
            this.tabPageRun.Text = "Params";
            this.tabPageRun.UseVisualStyleBackColor = true;
            // 
            // simInputControls
            // 
            this.simInputControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.simInputControls.Location = new System.Drawing.Point(0, 0);
            this.simInputControls.Name = "simInputControls";
            this.simInputControls.SimInput = null;
            this.simInputControls.Size = new System.Drawing.Size(375, 383);
            this.simInputControls.TabIndex = 16;
            this.simInputControls.Change += new System.EventHandler(this.simInputs_Change);
            // 
            // btnRecalc
            // 
            this.btnRecalc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecalc.Location = new System.Drawing.Point(268, 407);
            this.btnRecalc.Name = "btnRecalc";
            this.btnRecalc.Size = new System.Drawing.Size(82, 28);
            this.btnRecalc.TabIndex = 15;
            this.btnRecalc.Text = "Simulate";
            this.btnRecalc.UseVisualStyleBackColor = true;
            this.btnRecalc.Click += new System.EventHandler(this.btnRecalc_Click);
            // 
            // labelArrPower
            // 
            this.labelArrPower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelArrPower.AutoSize = true;
            this.labelArrPower.BackColor = System.Drawing.SystemColors.Control;
            this.labelArrPower.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.labelArrPower.Location = new System.Drawing.Point(81, 445);
            this.labelArrPower.Name = "labelArrPower";
            this.labelArrPower.ReadOnly = true;
            this.labelArrPower.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.labelArrPower.Size = new System.Drawing.Size(288, 39);
            this.labelArrPower.TabIndex = 12;
            this.labelArrPower.Text = "-";
            this.labelArrPower.WordWrap = false;
            // 
            // labelArrayPower
            // 
            this.labelArrayPower.AutoSize = true;
            this.labelArrayPower.Location = new System.Drawing.Point(81, 471);
            this.labelArrayPower.Name = "labelArrayPower";
            this.labelArrayPower.Size = new System.Drawing.Size(0, 13);
            this.labelArrayPower.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 445);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Array power:";
            // 
            // tabPageOutputs
            // 
            this.tabPageOutputs.Controls.Add(this.splitContainer2);
            this.tabPageOutputs.Location = new System.Drawing.Point(4, 22);
            this.tabPageOutputs.Name = "tabPageOutputs";
            this.tabPageOutputs.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOutputs.Size = new System.Drawing.Size(406, 395);
            this.tabPageOutputs.TabIndex = 1;
            this.tabPageOutputs.Text = "Outputs";
            this.tabPageOutputs.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.outputStringFlattenedLabel);
            this.splitContainer2.Panel1.Controls.Add(this.label6);
            this.splitContainer2.Panel1.Controls.Add(this.label17);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringsListBox);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringPerfectMPPTLabel);
            this.splitContainer2.Panel1.Controls.Add(this.label9);
            this.splitContainer2.Panel1.Controls.Add(this.label15);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringInsolationLabel);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringLabel);
            this.splitContainer2.Panel1.Controls.Add(this.label14);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringIVLinkLabel);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringPowerLabel);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringShadedLabel);
            this.splitContainer2.Panel1.Controls.Add(this.label16);
            this.splitContainer2.Panel1.Controls.Add(this.label13);
            this.splitContainer2.Panel1.Controls.Add(this.outputStringAreaLabel);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.outputArrayLayoutControl);
            this.splitContainer2.Size = new System.Drawing.Size(400, 389);
            this.splitContainer2.SplitterDistance = 218;
            this.splitContainer2.TabIndex = 17;
            // 
            // outputStringFlattenedLabel
            // 
            this.outputStringFlattenedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringFlattenedLabel.AutoSize = true;
            this.outputStringFlattenedLabel.Location = new System.Drawing.Point(307, 114);
            this.outputStringFlattenedLabel.Name = "outputStringFlattenedLabel";
            this.outputStringFlattenedLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringFlattenedLabel.TabIndex = 16;
            this.outputStringFlattenedLabel.Text = "0 W";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(134, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Performance of each string";
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(241, 114);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(60, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "...flattened:";
            // 
            // outputStringsListBox
            // 
            this.outputStringsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringsListBox.FormattingEnabled = true;
            this.outputStringsListBox.Location = new System.Drawing.Point(5, 28);
            this.outputStringsListBox.Name = "outputStringsListBox";
            this.outputStringsListBox.Size = new System.Drawing.Size(209, 186);
            this.outputStringsListBox.TabIndex = 1;
            this.outputStringsListBox.SelectedIndexChanged += new System.EventHandler(this.outputStringsListBox_SelectedIndexChanged);
            // 
            // outputStringPerfectMPPTLabel
            // 
            this.outputStringPerfectMPPTLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringPerfectMPPTLabel.AutoSize = true;
            this.outputStringPerfectMPPTLabel.Location = new System.Drawing.Point(307, 93);
            this.outputStringPerfectMPPTLabel.Name = "outputStringPerfectMPPTLabel";
            this.outputStringPerfectMPPTLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringPerfectMPPTLabel.TabIndex = 14;
            this.outputStringPerfectMPPTLabel.Text = "0 W";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(246, 52);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Insolation:";
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(216, 93);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(85, 13);
            this.label15.TabIndex = 13;
            this.label15.Text = "...perfect MPPT:";
            // 
            // outputStringInsolationLabel
            // 
            this.outputStringInsolationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringInsolationLabel.AutoSize = true;
            this.outputStringInsolationLabel.Location = new System.Drawing.Point(307, 52);
            this.outputStringInsolationLabel.Name = "outputStringInsolationLabel";
            this.outputStringInsolationLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringInsolationLabel.TabIndex = 4;
            this.outputStringInsolationLabel.Text = "0 W";
            // 
            // outputStringLabel
            // 
            this.outputStringLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringLabel.AutoSize = true;
            this.outputStringLabel.Location = new System.Drawing.Point(232, 28);
            this.outputStringLabel.Name = "outputStringLabel";
            this.outputStringLabel.Size = new System.Drawing.Size(101, 13);
            this.outputStringLabel.TabIndex = 12;
            this.outputStringLabel.Text = "(No string selected.)";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(227, 73);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Output power:";
            // 
            // outputStringIVLinkLabel
            // 
            this.outputStringIVLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringIVLinkLabel.AutoSize = true;
            this.outputStringIVLinkLabel.Location = new System.Drawing.Point(307, 184);
            this.outputStringIVLinkLabel.Name = "outputStringIVLinkLabel";
            this.outputStringIVLinkLabel.Size = new System.Drawing.Size(78, 13);
            this.outputStringIVLinkLabel.TabIndex = 11;
            this.outputStringIVLinkLabel.TabStop = true;
            this.outputStringIVLinkLabel.Text = "Show IV Trace";
            this.outputStringIVLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.outputStringIVLinkLabel_LinkClicked);
            // 
            // outputStringPowerLabel
            // 
            this.outputStringPowerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringPowerLabel.AutoSize = true;
            this.outputStringPowerLabel.Location = new System.Drawing.Point(307, 73);
            this.outputStringPowerLabel.Name = "outputStringPowerLabel";
            this.outputStringPowerLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringPowerLabel.TabIndex = 6;
            this.outputStringPowerLabel.Text = "0 W";
            // 
            // outputStringShadedLabel
            // 
            this.outputStringShadedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringShadedLabel.AutoSize = true;
            this.outputStringShadedLabel.Location = new System.Drawing.Point(307, 158);
            this.outputStringShadedLabel.Name = "outputStringShadedLabel";
            this.outputStringShadedLabel.Size = new System.Drawing.Size(36, 13);
            this.outputStringShadedLabel.TabIndex = 10;
            this.outputStringShadedLabel.Text = "0 m^2";
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(269, 138);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Area:";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(247, 158);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "...shaded:";
            // 
            // outputStringAreaLabel
            // 
            this.outputStringAreaLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputStringAreaLabel.AutoSize = true;
            this.outputStringAreaLabel.Location = new System.Drawing.Point(307, 138);
            this.outputStringAreaLabel.Name = "outputStringAreaLabel";
            this.outputStringAreaLabel.Size = new System.Drawing.Size(36, 13);
            this.outputStringAreaLabel.TabIndex = 8;
            this.outputStringAreaLabel.Text = "0 m^2";
            // 
            // outputArrayLayoutControl
            // 
            this.outputArrayLayoutControl.AnimatedSelection = false;
            this.outputArrayLayoutControl.Array = null;
            this.outputArrayLayoutControl.CellString = null;
            this.outputArrayLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputArrayLayoutControl.Editable = false;
            this.outputArrayLayoutControl.EditBypassDiodes = false;
            this.outputArrayLayoutControl.Location = new System.Drawing.Point(0, 0);
            this.outputArrayLayoutControl.Name = "outputArrayLayoutControl";
            this.outputArrayLayoutControl.Size = new System.Drawing.Size(400, 167);
            this.outputArrayLayoutControl.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.Controls.Add(this.groupBoxSimulation);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(406, 395);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Time-averaged sim";
            // 
            // groupBoxSimulation
            // 
            this.groupBoxSimulation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSimulation.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxSimulation.Controls.Add(this.label19);
            this.groupBoxSimulation.Controls.Add(this.dateTimePicker2);
            this.groupBoxSimulation.Controls.Add(this.label18);
            this.groupBoxSimulation.Controls.Add(this.dateTimePicker1);
            this.groupBoxSimulation.Controls.Add(this.label11);
            this.groupBoxSimulation.Controls.Add(this.labelSimTotalEnergy);
            this.groupBoxSimulation.Controls.Add(this.label12);
            this.groupBoxSimulation.Controls.Add(this.labelSimAvgEfficiency);
            this.groupBoxSimulation.Controls.Add(this.label8);
            this.groupBoxSimulation.Controls.Add(this.buttonRun);
            this.groupBoxSimulation.Controls.Add(this.labelSimAvgPower);
            this.groupBoxSimulation.Controls.Add(this.label10);
            this.groupBoxSimulation.Location = new System.Drawing.Point(8, 6);
            this.groupBoxSimulation.Name = "groupBoxSimulation";
            this.groupBoxSimulation.Size = new System.Drawing.Size(392, 214);
            this.groupBoxSimulation.TabIndex = 3;
            this.groupBoxSimulation.TabStop = false;
            this.groupBoxSimulation.Text = "Simulation";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(45, 67);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(167, 13);
            this.label19.TabIndex = 23;
            this.label19.Text = "UTC: See timezone under Params";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm (local)";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(102, 41);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(194, 20);
            this.dateTimePicker2.TabIndex = 22;
            this.dateTimePicker2.Value = new System.DateTime(2013, 10, 9, 17, 0, 0, 0);
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.dateTimePicker2_ValueChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(42, 44);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 13);
            this.label18.TabIndex = 21;
            this.label18.Text = "End time:";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm (local)";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(102, 15);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(194, 20);
            this.dateTimePicker1.TabIndex = 20;
            this.dateTimePicker1.Value = new System.DateTime(2013, 10, 9, 8, 0, 0, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(42, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Start time:";
            // 
            // labelSimTotalEnergy
            // 
            this.labelSimTotalEnergy.AutoSize = true;
            this.labelSimTotalEnergy.Location = new System.Drawing.Point(78, 77);
            this.labelSimTotalEnergy.Name = "labelSimTotalEnergy";
            this.labelSimTotalEnergy.Size = new System.Drawing.Size(0, 13);
            this.labelSimTotalEnergy.TabIndex = 18;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 184);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(69, 13);
            this.label12.TabIndex = 17;
            this.label12.Text = "Total energy:";
            // 
            // labelSimAvgEfficiency
            // 
            this.labelSimAvgEfficiency.AutoSize = true;
            this.labelSimAvgEfficiency.Location = new System.Drawing.Point(188, 54);
            this.labelSimAvgEfficiency.Name = "labelSimAvgEfficiency";
            this.labelSimAvgEfficiency.Size = new System.Drawing.Size(0, 13);
            this.labelSimAvgEfficiency.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(127, 161);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "efficiency:";
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(9, 103);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 9;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click_1);
            // 
            // labelSimAvgPower
            // 
            this.labelSimAvgPower.AutoSize = true;
            this.labelSimAvgPower.Location = new System.Drawing.Point(78, 54);
            this.labelSimAvgPower.Name = "labelSimAvgPower";
            this.labelSimAvgPower.Size = new System.Drawing.Size(0, 13);
            this.labelSimAvgPower.TabIndex = 14;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 161);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Avg power:";
            // 
            // glControl
            // 
            this.glControl.Array = null;
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(597, 516);
            this.glControl.Sprite = null;
            this.glControl.TabIndex = 0;
            this.glControl.VSync = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainForm";
            this.Text = "Stanford Solar Car Project - ShellPower";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageRun.ResumeLayout(false);
            this.tabPageRun.PerformLayout();
            this.tabPageOutputs.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBoxSimulation.ResumeLayout(false);
            this.groupBoxSimulation.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openModelToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageRun;
        private System.Windows.Forms.ToolStripMenuItem openLayoutToolStripMenuItem;
        private System.Windows.Forms.Label labelArrayPower;
        private System.Windows.Forms.Label label5;
        private ArrayModelControl glControl;
        private System.Windows.Forms.OpenFileDialog openFileDialogModel;
        private System.Windows.Forms.RichTextBox labelArrPower;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cellParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layoutTextureDimensionsToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPageOutputs;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox outputStringsListBox;
        private ArrayLayoutControl outputArrayLayoutControl;
        private System.Windows.Forms.Label outputStringAreaLabel;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label outputStringPowerLabel;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label outputStringInsolationLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label outputStringShadedLabel;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.LinkLabel outputStringIVLinkLabel;
        private System.Windows.Forms.Label outputStringLabel;
        private System.Windows.Forms.ToolStripMenuItem saveLayoutTextureToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialogLayout;
        private System.Windows.Forms.Label outputStringPerfectMPPTLabel;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label outputStringFlattenedLabel;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.ToolStripMenuItem bypassDiodeParametersToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBoxSimulation;
        private System.Windows.Forms.Label labelSimTotalEnergy;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelSimAvgEfficiency;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Label labelSimAvgPower;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btnRecalc;
        private SSCP.ShellPower.ArraySimInputControl simInputControls;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}