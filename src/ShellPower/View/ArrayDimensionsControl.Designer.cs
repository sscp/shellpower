namespace SSCP.ShellPower {
    partial class ArrayDimensionsControl {
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
            this.label1 = new System.Windows.Forms.Label();
            this.numX0 = new System.Windows.Forms.NumericUpDown();
            this.numX1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numZ0 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numZ1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numX0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZ0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZ1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min X (in meters):";
            // 
            // numX0
            // 
            this.numX0.DecimalPlaces = 3;
            this.numX0.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numX0.Location = new System.Drawing.Point(154, 22);
            this.numX0.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numX0.Name = "numX0";
            this.numX0.Size = new System.Drawing.Size(120, 20);
            this.numX0.TabIndex = 1;
            this.numX0.ValueChanged += new System.EventHandler(this.numX0_ValueChanged);
            // 
            // numX1
            // 
            this.numX1.DecimalPlaces = 3;
            this.numX1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numX1.Location = new System.Drawing.Point(154, 48);
            this.numX1.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numX1.Name = "numX1";
            this.numX1.Size = new System.Drawing.Size(120, 20);
            this.numX1.TabIndex = 3;
            this.numX1.ValueChanged += new System.EventHandler(this.numX1_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Width (in meters):";
            // 
            // numZ0
            // 
            this.numZ0.DecimalPlaces = 3;
            this.numZ0.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numZ0.Location = new System.Drawing.Point(154, 74);
            this.numZ0.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numZ0.Name = "numZ0";
            this.numZ0.Size = new System.Drawing.Size(120, 20);
            this.numZ0.TabIndex = 5;
            this.numZ0.ValueChanged += new System.EventHandler(this.numZ0_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Min Z (in meters):";
            // 
            // numZ1
            // 
            this.numZ1.DecimalPlaces = 3;
            this.numZ1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numZ1.Location = new System.Drawing.Point(154, 100);
            this.numZ1.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numZ1.Name = "numZ1";
            this.numZ1.Size = new System.Drawing.Size(120, 20);
            this.numZ1.TabIndex = 7;
            this.numZ1.ValueChanged += new System.EventHandler(this.numZ1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Height (in meters):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(219, 39);
            this.label5.TabIndex = 8;
            this.label5.Text = "The array will be textured from the top down. \r\n+Y is up, +X is right, and +Z is " +
    "back.\r\nThe array view will update automatically.";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(199, 207);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(118, 207);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // ArrayDimensionsControl
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(306, 253);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numZ1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numZ0);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numX1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numX0);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ArrayDimensionsControl";
            this.Text = "Array Texture Dimensions";
            ((System.ComponentModel.ISupportInitialize)(this.numX0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZ0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZ1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numX0;
        private System.Windows.Forms.NumericUpDown numX1;
        private System.Windows.Forms.NumericUpDown numZ0;
        private System.Windows.Forms.NumericUpDown numZ1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
    }
}