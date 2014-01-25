namespace SSCP.ShellPower {
    partial class ArraySimInputControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelAzimuthElevation = new System.Windows.Forms.Label();
            this.labelTimezone = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBoxTimezone = new System.Windows.Forms.ComboBox();
            this.trackBarTimeOfDay = new System.Windows.Forms.TrackBar();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.labelLocalTime = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.labelCarDirection = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBarCarDirection = new System.Windows.Forms.TrackBar();
            this.textBoxLon = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxLat = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTimeOfDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCarDirection)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.labelAzimuthElevation);
            this.groupBox2.Controls.Add(this.labelTimezone);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.comboBoxTimezone);
            this.groupBox2.Controls.Add(this.trackBarTimeOfDay);
            this.groupBox2.Controls.Add(this.dateTimePicker);
            this.groupBox2.Controls.Add(this.labelLocalTime);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(348, 111);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "When";
            // 
            // labelAzimuthElevation
            // 
            this.labelAzimuthElevation.AutoSize = true;
            this.labelAzimuthElevation.Location = new System.Drawing.Point(6, 154);
            this.labelAzimuthElevation.Name = "labelAzimuthElevation";
            this.labelAzimuthElevation.Size = new System.Drawing.Size(0, 13);
            this.labelAzimuthElevation.TabIndex = 9;
            // 
            // labelTimezone
            // 
            this.labelTimezone.AutoSize = true;
            this.labelTimezone.Location = new System.Drawing.Point(240, 77);
            this.labelTimezone.Name = "labelTimezone";
            this.labelTimezone.Size = new System.Drawing.Size(41, 13);
            this.labelTimezone.TabIndex = 18;
            this.labelTimezone.Text = "tzoffset";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "UTC";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Local";
            // 
            // comboBoxTimezone
            // 
            this.comboBoxTimezone.DisplayMember = "Id";
            this.comboBoxTimezone.FormattingEnabled = true;
            this.comboBoxTimezone.Location = new System.Drawing.Point(65, 74);
            this.comboBoxTimezone.Name = "comboBoxTimezone";
            this.comboBoxTimezone.Size = new System.Drawing.Size(169, 21);
            this.comboBoxTimezone.TabIndex = 17;
            this.comboBoxTimezone.ValueMember = "Id";
            this.comboBoxTimezone.SelectedIndexChanged += new System.EventHandler(this.anyInput_changed);
            // 
            // trackBarTimeOfDay
            // 
            this.trackBarTimeOfDay.AutoSize = false;
            this.trackBarTimeOfDay.LargeChange = 12;
            this.trackBarTimeOfDay.Location = new System.Drawing.Point(143, 48);
            this.trackBarTimeOfDay.Maximum = 287;
            this.trackBarTimeOfDay.Name = "trackBarTimeOfDay";
            this.trackBarTimeOfDay.Size = new System.Drawing.Size(138, 20);
            this.trackBarTimeOfDay.TabIndex = 8;
            this.trackBarTimeOfDay.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarTimeOfDay.Scroll += new System.EventHandler(this.trackBarTimeOfDay_Scroll);
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker.Location = new System.Drawing.Point(65, 22);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(177, 20);
            this.dateTimePicker.TabIndex = 0;
            this.dateTimePicker.Value = new System.DateTime(2011, 10, 24, 23, 0, 0, 0);
            this.dateTimePicker.ValueChanged += new System.EventHandler(this.anyInput_changed);
            // 
            // labelLocalTime
            // 
            this.labelLocalTime.AutoSize = true;
            this.labelLocalTime.Location = new System.Drawing.Point(62, 49);
            this.labelLocalTime.Name = "labelLocalTime";
            this.labelLocalTime.Size = new System.Drawing.Size(48, 13);
            this.labelLocalTime.TabIndex = 2;
            this.labelLocalTime.Text = "localtime";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 77);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 13);
            this.label20.TabIndex = 15;
            this.label20.Text = "Timezone";
            // 
            // labelCarDirection
            // 
            this.labelCarDirection.AutoSize = true;
            this.labelCarDirection.Location = new System.Drawing.Point(218, 26);
            this.labelCarDirection.Name = "labelCarDirection";
            this.labelCarDirection.Size = new System.Drawing.Size(45, 13);
            this.labelCarDirection.TabIndex = 13;
            this.labelCarDirection.Text = "heading";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Heading";
            // 
            // trackBarCarDirection
            // 
            this.trackBarCarDirection.AutoSize = false;
            this.trackBarCarDirection.LargeChange = 6;
            this.trackBarCarDirection.Location = new System.Drawing.Point(65, 19);
            this.trackBarCarDirection.Maximum = 35;
            this.trackBarCarDirection.Name = "trackBarCarDirection";
            this.trackBarCarDirection.Size = new System.Drawing.Size(147, 20);
            this.trackBarCarDirection.TabIndex = 11;
            this.trackBarCarDirection.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarCarDirection.Scroll += new System.EventHandler(this.anyInput_changed);
            // 
            // textBoxLon
            // 
            this.textBoxLon.Location = new System.Drawing.Point(65, 71);
            this.textBoxLon.Name = "textBoxLon";
            this.textBoxLon.Size = new System.Drawing.Size(147, 20);
            this.textBoxLon.TabIndex = 6;
            this.textBoxLon.Leave += new System.EventHandler(this.anyInput_changed);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Lon";
            // 
            // textBoxLat
            // 
            this.textBoxLat.Location = new System.Drawing.Point(65, 45);
            this.textBoxLat.Name = "textBoxLat";
            this.textBoxLat.Size = new System.Drawing.Size(147, 20);
            this.textBoxLat.TabIndex = 4;
            this.textBoxLat.Leave += new System.EventHandler(this.anyInput_changed);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Lat";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxLon);
            this.groupBox1.Controls.Add(this.labelCarDirection);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.trackBarCarDirection);
            this.groupBox1.Controls.Add(this.textBoxLat);
            this.groupBox1.Location = new System.Drawing.Point(3, 120);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 117);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Where";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 9;
            // 
            // ArraySimInputControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "ArraySimInputControl";
            this.Size = new System.Drawing.Size(354, 243);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTimeOfDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCarDirection)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelAzimuthElevation;
        private System.Windows.Forms.Label labelTimezone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxTimezone;
        private System.Windows.Forms.TrackBar trackBarTimeOfDay;
        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.Label labelLocalTime;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label labelCarDirection;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar trackBarCarDirection;
        private System.Windows.Forms.TextBox textBoxLon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxLat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
    }
}
