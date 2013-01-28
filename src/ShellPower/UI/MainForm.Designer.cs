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
            this.labelArrPower = new System.Windows.Forms.Label();
            this.labelArrayPower = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxInstantaneous = new System.Windows.Forms.GroupBox();
            this.labelTimezone = new System.Windows.Forms.Label();
            this.comboBoxTimezone = new System.Windows.Forms.ComboBox();
            this.label20 = new System.Windows.Forms.Label();
            this.btnRecalc = new System.Windows.Forms.Button();
            this.labelCarDirection = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBarCarDirection = new System.Windows.Forms.TrackBar();
            this.labelAzimuthElevation = new System.Windows.Forms.Label();
            this.trackBarTimeOfDay = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxLon = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLat = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelLocalTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.tabPageOutputs = new System.Windows.Forms.TabPage();
            this.outputStringFlattenedLabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.outputStringPerfectMPPTLabel = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.outputStringLabel = new System.Windows.Forms.Label();
            this.outputStringIVLinkLabel = new System.Windows.Forms.LinkLabel();
            this.outputStringShadedLabel = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.outputStringAreaLabel = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.outputStringPowerLabel = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.outputStringInsolationLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.outputStringsListBox = new System.Windows.Forms.ListBox();
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
            this.outputArrayLayoutControl = new SSCP.ShellPower.ArrayLayoutControl();
            this.glControl = new SSCP.ShellPower.ArrayModelControl();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageRun.SuspendLayout();
            this.groupBoxInstantaneous.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCarDirection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTimeOfDay)).BeginInit();
            this.tabPageOutputs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBoxSimulation.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 445);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1062, 22);
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
            this.menuStrip1.Size = new System.Drawing.Size(1062, 24);
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
            this.splitContainer1.Size = new System.Drawing.Size(1062, 421);
            this.splitContainer1.SplitterDistance = 414;
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
            this.tabControl1.Size = new System.Drawing.Size(414, 421);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPageRun
            // 
            this.tabPageRun.Controls.Add(this.labelArrPower);
            this.tabPageRun.Controls.Add(this.labelArrayPower);
            this.tabPageRun.Controls.Add(this.label5);
            this.tabPageRun.Controls.Add(this.groupBoxInstantaneous);
            this.tabPageRun.Location = new System.Drawing.Point(4, 22);
            this.tabPageRun.Name = "tabPageRun";
            this.tabPageRun.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRun.Size = new System.Drawing.Size(406, 395);
            this.tabPageRun.TabIndex = 0;
            this.tabPageRun.Text = "Params";
            this.tabPageRun.UseVisualStyleBackColor = true;
            // 
            // labelArrPower
            // 
            this.labelArrPower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelArrPower.AutoSize = true;
            this.labelArrPower.Location = new System.Drawing.Point(81, 369);
            this.labelArrPower.Name = "labelArrPower";
            this.labelArrPower.Size = new System.Drawing.Size(10, 13);
            this.labelArrPower.TabIndex = 12;
            this.labelArrPower.Text = "-";
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
            this.label5.Location = new System.Drawing.Point(9, 369);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Array power:";
            // 
            // groupBoxInstantaneous
            // 
            this.groupBoxInstantaneous.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInstantaneous.Controls.Add(this.labelTimezone);
            this.groupBoxInstantaneous.Controls.Add(this.comboBoxTimezone);
            this.groupBoxInstantaneous.Controls.Add(this.label20);
            this.groupBoxInstantaneous.Controls.Add(this.btnRecalc);
            this.groupBoxInstantaneous.Controls.Add(this.labelCarDirection);
            this.groupBoxInstantaneous.Controls.Add(this.label7);
            this.groupBoxInstantaneous.Controls.Add(this.trackBarCarDirection);
            this.groupBoxInstantaneous.Controls.Add(this.labelAzimuthElevation);
            this.groupBoxInstantaneous.Controls.Add(this.trackBarTimeOfDay);
            this.groupBoxInstantaneous.Controls.Add(this.label4);
            this.groupBoxInstantaneous.Controls.Add(this.textBoxLon);
            this.groupBoxInstantaneous.Controls.Add(this.label3);
            this.groupBoxInstantaneous.Controls.Add(this.textBoxLat);
            this.groupBoxInstantaneous.Controls.Add(this.label2);
            this.groupBoxInstantaneous.Controls.Add(this.labelLocalTime);
            this.groupBoxInstantaneous.Controls.Add(this.label1);
            this.groupBoxInstantaneous.Controls.Add(this.dateTimePicker);
            this.groupBoxInstantaneous.Location = new System.Drawing.Point(3, 6);
            this.groupBoxInstantaneous.Name = "groupBoxInstantaneous";
            this.groupBoxInstantaneous.Size = new System.Drawing.Size(400, 199);
            this.groupBoxInstantaneous.TabIndex = 1;
            this.groupBoxInstantaneous.TabStop = false;
            this.groupBoxInstantaneous.Text = "Instantaneous";
            // 
            // labelTimezone
            // 
            this.labelTimezone.AutoSize = true;
            this.labelTimezone.Location = new System.Drawing.Point(218, 74);
            this.labelTimezone.Name = "labelTimezone";
            this.labelTimezone.Size = new System.Drawing.Size(41, 13);
            this.labelTimezone.TabIndex = 18;
            this.labelTimezone.Text = "tzoffset";
            // 
            // comboBoxTimezone
            // 
            this.comboBoxTimezone.FormattingEnabled = true;
            this.comboBoxTimezone.Location = new System.Drawing.Point(65, 71);
            this.comboBoxTimezone.Name = "comboBoxTimezone";
            this.comboBoxTimezone.Size = new System.Drawing.Size(147, 21);
            this.comboBoxTimezone.TabIndex = 17;
            this.comboBoxTimezone.SelectedIndexChanged += new System.EventHandler(this.simInputs_AnyChange);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 74);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 13);
            this.label20.TabIndex = 15;
            this.label20.Text = "Timezone";
            // 
            // btnRecalc
            // 
            this.btnRecalc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecalc.Location = new System.Drawing.Point(312, 165);
            this.btnRecalc.Name = "btnRecalc";
            this.btnRecalc.Size = new System.Drawing.Size(82, 28);
            this.btnRecalc.TabIndex = 14;
            this.btnRecalc.Text = "Simulate";
            this.btnRecalc.UseVisualStyleBackColor = true;
            this.btnRecalc.Click += new System.EventHandler(this.btnRecalc_Click);
            // 
            // labelCarDirection
            // 
            this.labelCarDirection.AutoSize = true;
            this.labelCarDirection.Location = new System.Drawing.Point(218, 117);
            this.labelCarDirection.Name = "labelCarDirection";
            this.labelCarDirection.Size = new System.Drawing.Size(45, 13);
            this.labelCarDirection.TabIndex = 13;
            this.labelCarDirection.Text = "heading";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Car dir";
            // 
            // trackBarCarDirection
            // 
            this.trackBarCarDirection.AutoSize = false;
            this.trackBarCarDirection.LargeChange = 6;
            this.trackBarCarDirection.Location = new System.Drawing.Point(65, 110);
            this.trackBarCarDirection.Maximum = 35;
            this.trackBarCarDirection.Name = "trackBarCarDirection";
            this.trackBarCarDirection.Size = new System.Drawing.Size(147, 20);
            this.trackBarCarDirection.TabIndex = 11;
            this.trackBarCarDirection.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarCarDirection.Scroll += new System.EventHandler(this.simInputs_AnyChange);
            // 
            // labelAzimuthElevation
            // 
            this.labelAzimuthElevation.AutoSize = true;
            this.labelAzimuthElevation.Location = new System.Drawing.Point(6, 154);
            this.labelAzimuthElevation.Name = "labelAzimuthElevation";
            this.labelAzimuthElevation.Size = new System.Drawing.Size(0, 13);
            this.labelAzimuthElevation.TabIndex = 9;
            // 
            // trackBarTimeOfDay
            // 
            this.trackBarTimeOfDay.AutoSize = false;
            this.trackBarTimeOfDay.LargeChange = 12;
            this.trackBarTimeOfDay.Location = new System.Drawing.Point(143, 45);
            this.trackBarTimeOfDay.Maximum = 287;
            this.trackBarTimeOfDay.Name = "trackBarTimeOfDay";
            this.trackBarTimeOfDay.Size = new System.Drawing.Size(120, 20);
            this.trackBarTimeOfDay.TabIndex = 8;
            this.trackBarTimeOfDay.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarTimeOfDay.Scroll += new System.EventHandler(this.trackBarTimeOfDay_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Local";
            // 
            // textBoxLon
            // 
            this.textBoxLon.Location = new System.Drawing.Point(65, 162);
            this.textBoxLon.Name = "textBoxLon";
            this.textBoxLon.Size = new System.Drawing.Size(147, 20);
            this.textBoxLon.TabIndex = 6;
            this.textBoxLon.Text = "130.84";
            this.textBoxLon.Leave += new System.EventHandler(this.simInputs_AnyChange);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Lon";
            // 
            // textBoxLat
            // 
            this.textBoxLat.Location = new System.Drawing.Point(65, 136);
            this.textBoxLat.Name = "textBoxLat";
            this.textBoxLat.Size = new System.Drawing.Size(147, 20);
            this.textBoxLat.TabIndex = 4;
            this.textBoxLat.Text = "-12.46";
            this.textBoxLat.Leave += new System.EventHandler(this.simInputs_AnyChange);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lat";
            // 
            // labelLocalTime
            // 
            this.labelLocalTime.AutoSize = true;
            this.labelLocalTime.Location = new System.Drawing.Point(62, 46);
            this.labelLocalTime.Name = "labelLocalTime";
            this.labelLocalTime.Size = new System.Drawing.Size(48, 13);
            this.labelLocalTime.TabIndex = 2;
            this.labelLocalTime.Text = "localtime";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "UTC";
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker.Location = new System.Drawing.Point(65, 19);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(170, 20);
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.Value = new System.DateTime(2011, 10, 24, 23, 0, 0, 0);
            this.dateTimePicker.ValueChanged += new System.EventHandler(this.simInputs_AnyChange);
            // 
            // tabPageOutputs
            // 
            this.tabPageOutputs.Controls.Add(this.outputStringFlattenedLabel);
            this.tabPageOutputs.Controls.Add(this.label17);
            this.tabPageOutputs.Controls.Add(this.outputStringPerfectMPPTLabel);
            this.tabPageOutputs.Controls.Add(this.label15);
            this.tabPageOutputs.Controls.Add(this.outputStringLabel);
            this.tabPageOutputs.Controls.Add(this.outputStringIVLinkLabel);
            this.tabPageOutputs.Controls.Add(this.outputStringShadedLabel);
            this.tabPageOutputs.Controls.Add(this.label13);
            this.tabPageOutputs.Controls.Add(this.outputStringAreaLabel);
            this.tabPageOutputs.Controls.Add(this.label16);
            this.tabPageOutputs.Controls.Add(this.outputStringPowerLabel);
            this.tabPageOutputs.Controls.Add(this.label14);
            this.tabPageOutputs.Controls.Add(this.outputStringInsolationLabel);
            this.tabPageOutputs.Controls.Add(this.label9);
            this.tabPageOutputs.Controls.Add(this.label6);
            this.tabPageOutputs.Controls.Add(this.outputStringsListBox);
            this.tabPageOutputs.Controls.Add(this.outputArrayLayoutControl);
            this.tabPageOutputs.Location = new System.Drawing.Point(4, 22);
            this.tabPageOutputs.Name = "tabPageOutputs";
            this.tabPageOutputs.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOutputs.Size = new System.Drawing.Size(406, 395);
            this.tabPageOutputs.TabIndex = 1;
            this.tabPageOutputs.Text = "Outputs";
            this.tabPageOutputs.UseVisualStyleBackColor = true;
            // 
            // outputStringFlattenedLabel
            // 
            this.outputStringFlattenedLabel.AutoSize = true;
            this.outputStringFlattenedLabel.Location = new System.Drawing.Point(305, 108);
            this.outputStringFlattenedLabel.Name = "outputStringFlattenedLabel";
            this.outputStringFlattenedLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringFlattenedLabel.TabIndex = 16;
            this.outputStringFlattenedLabel.Text = "0 W";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(239, 108);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(60, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "...flattened:";
            // 
            // outputStringPerfectMPPTLabel
            // 
            this.outputStringPerfectMPPTLabel.AutoSize = true;
            this.outputStringPerfectMPPTLabel.Location = new System.Drawing.Point(305, 87);
            this.outputStringPerfectMPPTLabel.Name = "outputStringPerfectMPPTLabel";
            this.outputStringPerfectMPPTLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringPerfectMPPTLabel.TabIndex = 14;
            this.outputStringPerfectMPPTLabel.Text = "0 W";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(214, 87);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(85, 13);
            this.label15.TabIndex = 13;
            this.label15.Text = "...perfect MPPT:";
            // 
            // outputStringLabel
            // 
            this.outputStringLabel.AutoSize = true;
            this.outputStringLabel.Location = new System.Drawing.Point(230, 22);
            this.outputStringLabel.Name = "outputStringLabel";
            this.outputStringLabel.Size = new System.Drawing.Size(101, 13);
            this.outputStringLabel.TabIndex = 12;
            this.outputStringLabel.Text = "(No string selected.)";
            // 
            // outputStringIVLinkLabel
            // 
            this.outputStringIVLinkLabel.AutoSize = true;
            this.outputStringIVLinkLabel.Location = new System.Drawing.Point(305, 178);
            this.outputStringIVLinkLabel.Name = "outputStringIVLinkLabel";
            this.outputStringIVLinkLabel.Size = new System.Drawing.Size(78, 13);
            this.outputStringIVLinkLabel.TabIndex = 11;
            this.outputStringIVLinkLabel.TabStop = true;
            this.outputStringIVLinkLabel.Text = "Show IV Trace";
            this.outputStringIVLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.outputStringIVLinkLabel_LinkClicked);
            // 
            // outputStringShadedLabel
            // 
            this.outputStringShadedLabel.AutoSize = true;
            this.outputStringShadedLabel.Location = new System.Drawing.Point(305, 152);
            this.outputStringShadedLabel.Name = "outputStringShadedLabel";
            this.outputStringShadedLabel.Size = new System.Drawing.Size(36, 13);
            this.outputStringShadedLabel.TabIndex = 10;
            this.outputStringShadedLabel.Text = "0 m^2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(245, 152);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "...shaded:";
            // 
            // outputStringAreaLabel
            // 
            this.outputStringAreaLabel.AutoSize = true;
            this.outputStringAreaLabel.Location = new System.Drawing.Point(305, 132);
            this.outputStringAreaLabel.Name = "outputStringAreaLabel";
            this.outputStringAreaLabel.Size = new System.Drawing.Size(36, 13);
            this.outputStringAreaLabel.TabIndex = 8;
            this.outputStringAreaLabel.Text = "0 m^2";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(267, 132);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Area:";
            // 
            // outputStringPowerLabel
            // 
            this.outputStringPowerLabel.AutoSize = true;
            this.outputStringPowerLabel.Location = new System.Drawing.Point(305, 67);
            this.outputStringPowerLabel.Name = "outputStringPowerLabel";
            this.outputStringPowerLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringPowerLabel.TabIndex = 6;
            this.outputStringPowerLabel.Text = "0 W";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(225, 67);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Output power:";
            // 
            // outputStringInsolationLabel
            // 
            this.outputStringInsolationLabel.AutoSize = true;
            this.outputStringInsolationLabel.Location = new System.Drawing.Point(305, 46);
            this.outputStringInsolationLabel.Name = "outputStringInsolationLabel";
            this.outputStringInsolationLabel.Size = new System.Drawing.Size(27, 13);
            this.outputStringInsolationLabel.TabIndex = 4;
            this.outputStringInsolationLabel.Text = "0 W";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(244, 46);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Insolation:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(134, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Performance of each string";
            // 
            // outputStringsListBox
            // 
            this.outputStringsListBox.FormattingEnabled = true;
            this.outputStringsListBox.Location = new System.Drawing.Point(3, 22);
            this.outputStringsListBox.Name = "outputStringsListBox";
            this.outputStringsListBox.Size = new System.Drawing.Size(209, 186);
            this.outputStringsListBox.TabIndex = 1;
            this.outputStringsListBox.SelectedIndexChanged += new System.EventHandler(this.outputStringsListBox_SelectedIndexChanged);
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
            this.label19.Size = new System.Drawing.Size(128, 13);
            this.label19.TabIndex = 23;
            this.label19.Text = "Local time. See timezone.";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(102, 41);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker2.TabIndex = 22;
            this.dateTimePicker2.Value = new System.DateTime(2013, 10, 6, 17, 30, 0, 0);
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
            this.dateTimePicker1.Location = new System.Drawing.Point(102, 15);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 20;
            this.dateTimePicker1.Value = new System.DateTime(2013, 10, 6, 8, 30, 0, 0);
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
            // outputArrayLayoutControl
            // 
            this.outputArrayLayoutControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputArrayLayoutControl.AnimatedSelection = false;
            this.outputArrayLayoutControl.Array = null;
            this.outputArrayLayoutControl.CellString = null;
            this.outputArrayLayoutControl.Editable = false;
            this.outputArrayLayoutControl.EditBypassDiodes = false;
            this.outputArrayLayoutControl.Location = new System.Drawing.Point(-11, 206);
            this.outputArrayLayoutControl.Name = "outputArrayLayoutControl";
            this.outputArrayLayoutControl.Size = new System.Drawing.Size(406, 180);
            this.outputArrayLayoutControl.TabIndex = 0;
            // 
            // glControl
            // 
            this.glControl.Array = null;
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(644, 421);
            this.glControl.Sprite = null;
            this.glControl.TabIndex = 0;
            this.glControl.VSync = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 467);
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
            this.groupBoxInstantaneous.ResumeLayout(false);
            this.groupBoxInstantaneous.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCarDirection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTimeOfDay)).EndInit();
            this.tabPageOutputs.ResumeLayout(false);
            this.tabPageOutputs.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBoxInstantaneous;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageRun;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.Label labelLocalTime;
        private System.Windows.Forms.TextBox textBoxLon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem openLayoutToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelArrayPower;
        private System.Windows.Forms.Label label5;
        private ArrayModelControl glControl;
        private System.Windows.Forms.TrackBar trackBarTimeOfDay;
        private System.Windows.Forms.Label labelAzimuthElevation;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelCarDirection;
        private System.Windows.Forms.OpenFileDialog openFileDialogModel;
        private System.Windows.Forms.Label labelArrPower;
        private System.Windows.Forms.Button btnRecalc;
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
        private System.Windows.Forms.TrackBar trackBarCarDirection;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox comboBoxTimezone;
        private System.Windows.Forms.Label labelTimezone;
    }
}