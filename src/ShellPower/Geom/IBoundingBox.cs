using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSCP.ShellPower {
    public interface IBoundingBox {
        Quad3 BoundingBox { get; }
    }
}
