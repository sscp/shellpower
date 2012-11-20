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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Voc at STC:";
            // 
            // textBoxVoc
            // 
            this.textBoxVoc.Location = new System.Drawing.Point(193, 6);
            this.textBoxVoc.Name = "textBoxVoc";
            this.textBoxVoc.Size = new System.Drawing.Size(124, 20);
            this.textBoxVoc.TabIndex = 1;
            this.textBoxVoc.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // textBoxIsc
            // 
            this.textBoxIsc.Location = new System.Drawing.Point(193, 32);
            this.textBoxIsc.Name = "textBoxIsc";
            this.textBoxIsc.Size = new System.Drawing.Size(124, 20);
            this.textBoxIsc.TabIndex = 3;
            this.textBoxIsc.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Isc at STC:";
            // 
            // textBoxVocTemp
            // 
            this.textBoxVocTemp.Location = new System.Drawing.Point(193, 58);
            this.textBoxVocTemp.Name = "textBoxVocTemp";
            this.textBoxVocTemp.Size = new System.Drawing.Size(124, 20);
            this.textBoxVocTemp.TabIndex = 5;
            this.textBoxVocTemp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Voc temp sensitivity (volts/deg k):";
            // 
            // textBoxIscTemp
            // 
            this.textBoxIscTemp.Location = new System.Drawing.Point(193, 84);
            this.textBoxIscTemp.Name = "textBoxIscTemp";
            this.textBoxIscTemp.Size = new System.Drawing.Size(124, 20);
            this.textBoxIscTemp.TabIndex = 7;
            this.textBoxIscTemp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Isc temp sensitivity (amps/deg k):";
            // 
            // textBoxArea
            // 
            this.textBoxArea.Location = new System.Drawing.Point(193, 110);
            this.textBoxArea.Name = "textBoxArea";
            this.textBoxArea.Size = new System.Drawing.Size(124, 20);
            this.textBoxArea.TabIndex = 9;
            this.textBoxArea.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Area (m^2):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 231);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(270, 91);
            this.label6.TabIndex = 10;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // labelMaxPower
            // 
            this.labelMaxPower.AutoSize = true;
            this.labelMaxPower.Location = new System.Drawing.Point(339, 286);
            this.labelMaxPower.Name = "labelMaxPower";
            this.labelMaxPower.Size = new System.Drawing.Size(29, 13);
            this.labelMaxPower.TabIndex = 11;
            this.labelMaxPower.Text = "label";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(242, 334);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 13;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxTemp
            // 
            this.textBoxTemp.Location = new System.Drawing.Point(193, 136);
            this.textBoxTemp.Name = "textBoxTemp";
            this.textBoxTemp.Size = new System.Drawing.Size(124, 20);
            this.textBoxTemp.TabIndex = 15;
            this.textBoxTemp.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Temperature:";
            // 
            // textBoxNIdeal
            // 
            this.textBoxNIdeal.Location = new System.Drawing.Point(193, 162);
            this.textBoxNIdeal.Name = "textBoxNIdeal";
            this.textBoxNIdeal.Size = new System.Drawing.Size(124, 20);
            this.textBoxNIdeal.TabIndex = 18;
            this.textBoxNIdeal.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 165);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Ideality constant:";
            // 
            // textBoxSeriesR
            // 
            this.textBoxSeriesR.Location = new System.Drawing.Point(193, 188);
            this.textBoxSeriesR.Name = "textBoxSeriesR";
            this.textBoxSeriesR.Size = new System.Drawing.Size(124, 20);
            this.textBoxSeriesR.TabIndex = 20;
            this.textBoxSeriesR.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 191);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Series resistance:";
            // 
            // chartIV
            // 
            this.chartIV.Location = new System.Drawing.Point(342, 6);
            this.chartIV.Margins = new int[] {
        10,
        10,
        20,
        20};
            this.chartIV.Name = "chartIV";
            this.chartIV.Size = new System.Drawing.Size(388, 277);
            this.chartIV.TabIndex = 16;
            this.chartIV.X = null;
            this.chartIV.Y = null;
            // 
            // CellParamsForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 378);
            this.Controls.Add(this.textBoxSeriesR);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxNIdeal);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.chartIV);
            this.Controls.Add(this.textBoxTemp);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.labelMaxPower);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBoxArea);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxIscTemp);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxVocTemp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxIsc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxVoc);
            this.Controls.Add(this.label1);
            this.Name = "CellParamsForm";
            this.Text = "Solar Cell Parameters";
            this.Load += new System.EventHandler(this.CellParamsForm_Load);
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
    }
}