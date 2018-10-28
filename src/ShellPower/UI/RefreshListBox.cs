using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SSCP.ShellPower {
    public class RefreshListBox : ListBox {
        public new void RefreshItem(int index) {
            base.RefreshItem(index);
        }
        public new void RefreshItems() {
            base.RefreshItems();
        }
    }
}
