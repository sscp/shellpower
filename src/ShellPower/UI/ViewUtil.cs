using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    public class ViewUtil {
        public static bool ValidateEntry(TextBox textBox, out double val, double min, double max) {
            if (!double.TryParse(textBox.Text, out val) || val < min || val > max) {
                textBox.BackColor = Color.FromArgb(0xff, 0xff, 0xbb, 0xaa);
                return false;
            } else {
                textBox.BackColor = Color.White;
                return true;
            }
        }
    }
}
