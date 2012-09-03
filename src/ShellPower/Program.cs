using System;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    static class Program {
        [STAThread]
        static void Main() {
            Logger.info("starting shellpower");
            var form = new MainForm();
            Application.Run(form);
        }
    }
}
