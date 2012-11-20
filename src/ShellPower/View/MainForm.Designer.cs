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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSimParamsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageRun = new System.Windows.Forms.TabPage();
            this.labelArrPower = new System.Windows.Forms.Label();
            this.labelArrayPower = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxInstantaneous = new System.Windows.Forms.GroupBox();
            this.btnRecalc = new System.Windows.Forms.Button();
            this.labelCarDirection = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBarCarDirection = new System.Windows.Forms.TrackBar();
            this.labelTimezone = new System.Windows.Forms.Label();
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
            this.groupBoxSimulation = new System.Windows.Forms.GroupBox();
            this.labelSimTotalEnergy = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelSimAvgEfficiency = new System.Windows.Forms.Label();
            this.buttonAnimate = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.labelSimAvgPower = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.glControl = new SSCP.ShellPower.ArrayModelControl();
            this.openFileDialogModel = new System.Windows.Forms.OpenFileDialog();
            this.cellParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.openSimParamsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openModelToolStripMenuItem
            // 
            this.openModelToolStripMenuItem.Name = "openModelToolStripMenuItem";
            this.openModelToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openModelToolStripMenuItem.Text = "Open model";
            this.openModelToolStripMenuItem.Click += new System.EventHandler(this.openModelToolStripMenuItem_Click);
            // 
            // openLayoutToolStripMenuItem
            // 
            this.openLayoutToolStripMenuItem.Name = "openLayoutToolStripMenuItem";
            this.openLayoutToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openLayoutToolStripMenuItem.Text = "Open layout ";
            this.openLayoutToolStripMenuItem.Click += new System.EventHandler(this.openLayoutToolStripMenuItem_Click);
            // 
            // openSimParamsToolStripMenuItem
            // 
            this.openSimParamsToolStripMenuItem.Name = "openSimParamsToolStripMenuItem";
            this.openSimParamsToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.openSimParamsToolStripMenuItem.Text = "Open sim params";
            this.openSimParamsToolStripMenuItem.Click += new System.EventHandler(this.openSimParamsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.layoutToolStripMenuItem,
            this.cellParametersToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // layoutToolStripMenuItem
            // 
            this.layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
            this.layoutToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.layoutToolStripMenuItem.Text = "Layout";
            this.layoutToolStripMenuItem.Click += new System.EventHandler(this.layoutToolStripMenuItem_Click);
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
            this.tabPageRun.Controls.Add(this.groupBoxSimulation);
            this.tabPageRun.Location = new System.Drawing.Point(4, 22);
            this.tabPageRun.Name = "tabPageRun";
            this.tabPageRun.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRun.Size = new System.Drawing.Size(406, 395);
            this.tabPageRun.TabIndex = 0;
            this.tabPageRun.Text = "Run";
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
            this.groupBoxInstantaneous.Controls.Add(this.btnRecalc);
            this.groupBoxInstantaneous.Controls.Add(this.labelCarDirection);
            this.groupBoxInstantaneous.Controls.Add(this.label7);
            this.groupBoxInstantaneous.Controls.Add(this.trackBarCarDirection);
            this.groupBoxInstantaneous.Controls.Add(this.labelTimezone);
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
            // btnRecalc
            // 
            this.btnRecalc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecalc.Location = new System.Drawing.Point(319, 170);
            this.btnRecalc.Name = "btnRecalc";
            this.btnRecalc.Size = new System.Drawing.Size(75, 23);
            this.btnRecalc.TabIndex = 14;
            this.btnRecalc.Text = "Calculate";
            this.btnRecalc.UseVisualStyleBackColor = true;
            this.btnRecalc.Click += new System.EventHandler(this.btnRecalc_Click);
            // 
            // labelCarDirection
            // 
            this.labelCarDirection.AutoSize = true;
            this.labelCarDirection.Location = new System.Drawing.Point(175, 78);
            this.labelCarDirection.Name = "labelCarDirection";
            this.labelCarDirection.Size = new System.Drawing.Size(0, 13);
            this.labelCarDirection.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Car dir";
            // 
            // trackBarCarDirection
            // 
            this.trackBarCarDirection.AutoSize = false;
            this.trackBarCarDirection.Location = new System.Drawing.Point(65, 71);
            this.trackBarCarDirection.Maximum = 15;
            this.trackBarCarDirection.Name = "trackBarCarDirection";
            this.trackBarCarDirection.Size = new System.Drawing.Size(104, 20);
            this.trackBarCarDirection.TabIndex = 11;
            this.trackBarCarDirection.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarCarDirection.Scroll += new System.EventHandler(this.GuiSimStepInputs);
            // 
            // labelTimezone
            // 
            this.labelTimezone.AutoSize = true;
            this.labelTimezone.Location = new System.Drawing.Point(6, 174);
            this.labelTimezone.Name = "labelTimezone";
            this.labelTimezone.Size = new System.Drawing.Size(35, 13);
            this.labelTimezone.TabIndex = 10;
            this.labelTimezone.Text = "label9";
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
            this.trackBarTimeOfDay.Location = new System.Drawing.Point(143, 45);
            this.trackBarTimeOfDay.Maximum = 47;
            this.trackBarTimeOfDay.Name = "trackBarTimeOfDay";
            this.trackBarTimeOfDay.Size = new System.Drawing.Size(104, 20);
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
            this.textBoxLon.Location = new System.Drawing.Point(65, 123);
            this.textBoxLon.Name = "textBoxLon";
            this.textBoxLon.Size = new System.Drawing.Size(147, 20);
            this.textBoxLon.TabIndex = 6;
            this.textBoxLon.Text = "130.84";
            this.textBoxLon.Leave += new System.EventHandler(this.GuiSimStepInputs);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Lon";
            // 
            // textBoxLat
            // 
            this.textBoxLat.Location = new System.Drawing.Point(65, 97);
            this.textBoxLat.Name = "textBoxLat";
            this.textBoxLat.Size = new System.Drawing.Size(147, 20);
            this.textBoxLat.TabIndex = 4;
            this.textBoxLat.Text = "-12.46";
            this.textBoxLat.Leave += new System.EventHandler(this.GuiSimStepInputs);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 100);
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
            this.labelLocalTime.Size = new System.Drawing.Size(0, 13);
            this.labelLocalTime.TabIndex = 2;
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
            this.dateTimePicker.Size = new System.Drawing.Size(182, 20);
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.Value = new System.DateTime(2011, 10, 24, 23, 0, 0, 0);
            this.dateTimePicker.ValueChanged += new System.EventHandler(this.GuiSimStepInputs);
            // 
            // groupBoxSimulation
            // 
            this.groupBoxSimulation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSimulation.Controls.Add(this.labelSimTotalEnergy);
            this.groupBoxSimulation.Controls.Add(this.label12);
            this.groupBoxSimulation.Controls.Add(this.labelSimAvgEfficiency);
            this.groupBoxSimulation.Controls.Add(this.buttonAnimate);
            this.groupBoxSimulation.Controls.Add(this.label8);
            this.groupBoxSimulation.Controls.Add(this.buttonRun);
            this.groupBoxSimulation.Controls.Add(this.labelSimAvgPower);
            this.groupBoxSimulation.Controls.Add(this.label10);
            this.groupBoxSimulation.Location = new System.Drawing.Point(6, 211);
            this.groupBoxSimulation.Name = "groupBoxSimulation";
            this.groupBoxSimulation.Size = new System.Drawing.Size(400, 107);
            this.groupBoxSimulation.TabIndex = 2;
            this.groupBoxSimulation.TabStop = false;
            this.groupBoxSimulation.Text = "Simulation";
            this.groupBoxSimulation.Visible = false;
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
            this.label12.Location = new System.Drawing.Point(6, 77);
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
            // buttonAnimate
            // 
            this.buttonAnimate.Location = new System.Drawing.Point(86, 19);
            this.buttonAnimate.Name = "buttonAnimate";
            this.buttonAnimate.Size = new System.Drawing.Size(75, 23);
            this.buttonAnimate.TabIndex = 10;
            this.buttonAnimate.Text = "Animate";
            this.buttonAnimate.UseVisualStyleBackColor = true;
            this.buttonAnimate.Click += new System.EventHandler(this.buttonAnimate_Click);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(127, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "efficiency:";
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(5, 19);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 9;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
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
            this.label10.Location = new System.Drawing.Point(6, 54);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "Avg power:";
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(644, 421);
            this.glControl.Sprite = null;
            this.glControl.SunDirection = ((OpenTK.Vector3)(resources.GetObject("glControl.SunDirection")));
            this.glControl.TabIndex = 0;
            this.glControl.VSync = true;
            // 
            // openFileDialogModel
            // 
            this.openFileDialogModel.Filter = "3DXML files|*.3dxml|STL Files (ascii)|*.stl|All files|*.*";
            this.openFileDialogModel.Title = "Load solar car model";
            // 
            // cellParametersToolStripMenuItem
            // 
            this.cellParametersToolStripMenuItem.Name = "cellParametersToolStripMenuItem";
            this.cellParametersToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.cellParametersToolStripMenuItem.Text = "Cell Parameters";
            this.cellParametersToolStripMenuItem.Click += new System.EventHandler(this.cellParametersToolStripMenuItem_Click);
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
        private System.Windows.Forms.GroupBox groupBoxSimulation;
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
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonAnimate;
        private System.Windows.Forms.ToolStripMenuItem openLayoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSimParamsToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelArrayPower;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelSimAvgEfficiency;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelSimAvgPower;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelSimTotalEnergy;
        private System.Windows.Forms.Label label12;
        private ArrayModelControl glControl;
        private System.Windows.Forms.TrackBar trackBarTimeOfDay;
        private System.Windows.Forms.Label labelTimezone;
        private System.Windows.Forms.Label labelAzimuthElevation;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar trackBarCarDirection;
        private System.Windows.Forms.Label labelCarDirection;
        private System.Windows.Forms.OpenFileDialog openFileDialogModel;
        private System.Windows.Forms.Label labelArrPower;
        private System.Windows.Forms.Button btnRecalc;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cellParametersToolStripMenuItem;
    }
}