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
        /// </summary>
        public Boolean Contains(Vector2 v) {
            int intersections = 0;
            for (int i = 0; i < vertices.Length; i++) {
                Vector2 a = vertices[i];
                Vector2 b = vertices[(i+1)%vertices.Length];
                if (v.X < a.X) continue;
                if (v.X >= b.X) continue;
                float y = (v.X - a.X) * (b.Y - a.Y) / (b.X - a.X + float.Epsilon);
                if (y < v.Y) continue;
                intersections++;
            }
            return (intersections % 2) == 1;
        }
    }
}
