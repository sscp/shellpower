namespace SSCP.ShellPower {
    partial class CellParamsForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CellParamsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxVoc = new System.Windows.Forms.TextBox();
            this.textBoxIsc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxVocTemp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxIscTemp = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxArea = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelMaxPower = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxTemp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxNIdeal = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxSeriesR = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chartIV = new SSCP.ShellPower.SimpleGraph();
            this.textBoxInsolation = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Voc at STC:";
            // 
            // textBoxVoc
            // 
            this.textBoxVoc.Location = new System.Drawing.Point(187, 14);
            this.textBoxVoc.Name = "textBoxVoc";
            this.textBoxVoc.Size = new System.Drawing.Size(124, 20);
            this.textBoxVoc.TabIndex = 1;
            this.textBoxVoc.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxIsc
            // 
            this.textBoxIsc.Location = new System.Drawing.Point(187, 40);
            this.textBoxIsc.Name = "textBoxIsc";
            this.textBoxIsc.Size = new System.Drawing.Size(124, 20);
            this.textBoxIsc.TabIndex = 3;
            this.textBoxIsc.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Isc at STC:";
            // 
            // textBoxVocTemp
            // 
            this.textBoxVocTemp.Location = new System.Drawing.Point(187, 66);
            this.textBoxVocTemp.Name = "textBoxVocTemp";
            this.textBoxVocTemp.Size = new System.Drawing.Size(124, 20);
            this.textBoxVocTemp.TabIndex = 5;
            this.textBoxVocTemp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Voc temp sensitivity (volts/deg C):";
            // 
            // textBoxIscTemp
            // 
            this.textBoxIscTemp.Location = new System.Drawing.Point(187, 92);
            this.textBoxIscTemp.Name = "textBoxIscTemp";
            this.textBoxIscTemp.Size = new System.Drawing.Size(124, 20);
            this.textBoxIscTemp.TabIndex = 7;
            this.textBoxIscTemp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(165, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Isc temp sensitivity (amps/deg C):";
            // 
            // textBoxArea
            // 
            this.textBoxArea.Location = new System.Drawing.Point(187, 118);
            this.textBoxArea.Name = "textBoxArea";
            this.textBoxArea.Size = new System.Drawing.Size(124, 20);
            this.textBoxArea.TabIndex = 9;
            this.textBoxArea.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 121);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Area (m^2):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 279);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(270, 91);
            this.label6.TabIndex = 10;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // labelMaxPower
            // 
            this.labelMaxPower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMaxPower.AutoSize = true;
            this.labelMaxPower.Location = new System.Drawing.Point(409, 326);
            this.labelMaxPower.Name = "labelMaxPower";
            this.labelMaxPower.Size = new System.Drawing.Size(29, 13);
            this.labelMaxPower.TabIndex = 11;
            this.labelMaxPower.Text = "label";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(167, 387);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 13;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxTemp
            // 
            this.textBoxTemp.Location = new System.Drawing.Point(187, 13);
            this.textBoxTemp.Name = "textBoxTemp";
            this.textBoxTemp.Size = new System.Drawing.Size(124, 20);
            this.textBoxTemp.TabIndex = 15;
            this.textBoxTemp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Temperature (deg C):";
            // 
            // textBoxNIdeal
            // 
            this.textBoxNIdeal.Location = new System.Drawing.Point(187, 144);
            this.textBoxNIdeal.Name = "textBoxNIdeal";
            this.textBoxNIdeal.Size = new System.Drawing.Size(124, 20);
            this.textBoxNIdeal.TabIndex = 18;
            this.textBoxNIdeal.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 147);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Ideality constant:";
            // 
            // textBoxSeriesR
            // 
            this.textBoxSeriesR.Location = new System.Drawing.Point(187, 170);
            this.textBoxSeriesR.Name = "textBoxSeriesR";
            this.textBoxSeriesR.Size = new System.Drawing.Size(124, 20);
            this.textBoxSeriesR.TabIndex = 20;
            this.textBoxSeriesR.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 173);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Series resistance:";
            // 
            // chartIV
            // 
            this.chartIV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartIV.Location = new System.Drawing.Point(342, 6);
            this.chartIV.Margins = new int[] {
        10,
        10,
        20,
        20};
            this.chartIV.Name = "chartIV";
            this.chartIV.Size = new System.Drawing.Size(458, 317);
            this.chartIV.TabIndex = 16;
            this.chartIV.X = null;
            this.chartIV.Y = null;
            // 
            // textBoxInsolation
            // 
            this.textBoxInsolation.Location = new System.Drawing.Point(187, 39);
            this.textBoxInsolation.Name = "textBoxInsolation";
            this.textBoxInsolation.Size = new System.Drawing.Size(124, 20);
            this.textBoxInsolation.TabIndex = 23;
            this.textBoxInsolation.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 42);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(100, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Insolation (W/m^2):";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxArea);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxVoc);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxSeriesR);
            this.groupBox1.Controls.Add(this.textBoxIsc);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxNIdeal);
            this.groupBox1.Controls.Add(this.textBoxVocTemp);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBoxIscTemp);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(324, 195);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cells";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxTemp);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.textBoxInsolation);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Location = new System.Drawing.Point(12, 201);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(323, 71);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Conditions";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(248, 387);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 26;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // CellParamsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(804, 422);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chartIV);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelMaxPower);
            this.Controls.Add(this.label6);
            this.MinimumSize = new System.Drawing.Size(820, 460);
            this.Name = "CellParamsForm";
            this.Text = "Solar Cell Parameters";
            this.Load += new System.EventHandler(this.CellParamsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxVoc;
        private System.Windows.Forms.TextBox textBoxIsc;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxVocTemp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxIscTemp;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxArea;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelMaxPower;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxTemp;
        private System.Windows.Forms.Label label7;
        private SimpleGraph chartIV;
        private System.Windows.Forms.TextBox textBoxNIdeal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxSeriesR;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxInsolation;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonCancel;
    }
}