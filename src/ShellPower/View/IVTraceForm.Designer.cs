namespace SSCP.ShellPower {
    partial class IVTraceForm {
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
            this.simpleGraphIV = new SSCP.ShellPower.SimpleGraph();
            this.labelName = new System.Windows.Forms.Label();
            this.labelMaxPower = new System.Windows.Forms.Label();
            this.labelFillFactor = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // simpleGraphIV
            // 
            this.simpleGraphIV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleGraphIV.Location = new System.Drawing.Point(12, 71);
            this.simpleGraphIV.Margins = new int[] {
        10,
        10,
        20,
        20};
            this.simpleGraphIV.Name = "simpleGraphIV";
            this.simpleGraphIV.Size = new System.Drawing.Size(333, 230);
            this.simpleGraphIV.TabIndex = 0;
            this.simpleGraphIV.X = null;
            this.simpleGraphIV.Y = null;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(57, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "labelName";
            // 
            // labelMaxPower
            // 
            this.labelMaxPower.AutoSize = true;
            this.labelMaxPower.Location = new System.Drawing.Point(12, 28);
            this.labelMaxPower.Name = "labelMaxPower";
            this.labelMaxPower.Size = new System.Drawing.Size(79, 13);
            this.labelMaxPower.TabIndex = 2;
            this.labelMaxPower.Text = "labelMaxPower";
            // 
            // labelFillFactor
            // 
            this.labelFillFactor.AutoSize = true;
            this.labelFillFactor.Location = new System.Drawing.Point(12, 47);
            this.labelFillFactor.Name = "labelFillFactor";
            this.labelFillFactor.Size = new System.Drawing.Size(35, 13);
            this.labelFillFactor.TabIndex = 3;
            this.labelFillFactor.Text = "label1";
            // 
            // IVTraceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 313);
            this.Controls.Add(this.labelFillFactor);
            this.Controls.Add(this.labelMaxPower);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.simpleGraphIV);
            this.Name = "IVTraceForm";
            this.Text = "IV Trace";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SimpleGraph simpleGraphIV;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelMaxPower;
        private System.Windows.Forms.Label labelFillFactor;
    }
}