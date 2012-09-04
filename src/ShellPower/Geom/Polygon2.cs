using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    public class Polygon2 : IArea {
        public Vector2[] vertices;
        /// <summary>
        /// Determines whether the 2D polygon contains a given point.
        /// Treats the polygon as a closed set, ie it contains each point on its boundary.
        /// </summary>
        public Boolean Contains(Vector2 v) {
            // project a ray from v in the positive Y dir. count the # intersections 
            // with the polygon. if it is an odd number, we're inside the polygon
            int intersections = 0, onTheLine = 0;
            for (int i = 0; i < vertices.Length; i++) {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i+1)%vertices.Length];
                if (v.X < Math.Min(a.X, b.X)) continue;
                if (v.X >= Math.Max(a.X, b.X)) continue;
                float y = (v.X - a.X) * (b.Y - a.Y) / (b.X - a.X + float.Epsilon) + a.Y;
                if (y == v.Y) {
                    onTheLine++;
                } else if (y > v.Y) {
                    intersections++;
                }
            }
            // all boundary points are contained
            return (intersections % 2) == 1 || ((intersections + onTheLine) % 2) == 1;
        }
    }
}
