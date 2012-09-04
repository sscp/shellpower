using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    public interface IVolume {
        bool Contains(Vector3 vec);
    }
    public class ExtrudedVolume : IVolume {
        public enum Plane { XY, YX, XZ, ZX, YZ, ZY };
        public Plane plane;
        public IArea area;

        public bool Contains(Vector3 vec) {
            Vector2 v2;
            switch (plane) {
                case Plane.XY:
                    v2 = new Vector2(vec.X, vec.Y);
                    break;
                case Plane.YX:
                    v2 = new Vector2(vec.Y, vec.X);
                    break;
                case Plane.XZ:
                    v2 = new Vector2(vec.X, vec.Z);
                    break;
                case Plane.ZX:
                    v2 = new Vector2(vec.Z, vec.X);
                    break;
                case Plane.YZ:
                    v2 = new Vector2(vec.Y, vec.Z);
                    break;
                case Plane.ZY:
                    v2 = new Vector2(vec.Z, vec.Y);
                    break;
                default:
                    throw new InvalidOperationException("must specify a plane");
            }
            return area.Contains(v2);
        }
    }
}
