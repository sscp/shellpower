using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using OpenTK;

namespace SSCP.ShellPower {
    /// <summary>
    /// Represents a solar array.
    /// 
    /// This comprises:
    /// * A 3D mesh that defines the shape. 
    ///   Mesh dimensions should be in meters.
    /// * A texture that places the individual cells.
    /// * (TODO) A mapping that names all the cells, and defines 
    ///   which texture color each one corresponds to.
    /// * (TODO) A set of links that joins cells into strings
    /// * (TODO) An optional set of bypass diodes
    /// </summary>
    public class ArraySpec {
        public ArraySpec() {
            Strings = new List<CellString>();
            CellSpec = new CellSpec();
        }
        /// <summary>
        /// The shape of the array.
        /// Dimensions should be in meters. The positive Y direction is up.
        /// </summary>
        public Mesh Mesh { get; set; }
        /// <summary>
        /// The location of cells on the array. Each cell should have a unique, flat color.
        /// </summary>
        public Bitmap LayoutTexture { get; set; }
        /// <summary>
        /// Aligns the layout texture. 
        /// The texture is expected to be a top-down ortho projection.
        /// 
        /// This is a rectangle. [x0, z0] is the top-left corner
        /// of the texture (in model coordinates, in meters) and [x1, z1]
        /// is the bottom-right corner.
        /// </summary>
        public RectangleF LayoutBoundsXZ { get; set; }
        /// <summary>
        /// Assigns cells to strings (a group of cells in series).
        /// </summary>
        public List<CellString> Strings { get; private set; }
        public class CellString {
            public CellString() {
                Cells = new List<Cell>();
                Name = "NewString";
            }
            public List<Cell> Cells { get; private set; }
            public String Name { get; set; }
            //TODO: bypass diodes
            public override string ToString() {
                return Name + " ("+Cells.Count+" cells)";
            }
        }
        public class Cell {
            public Color Color { get; set; }
            public List<Pair<int>> Pixels { get; private set; } //must be sorted, scanline order
            public Cell() {
                Pixels = new List<Pair<int>>();
            }
            public override int GetHashCode() {
                if (Pixels.Count == 0) return 0;
                return Pixels[0].GetHashCode();
            }
            public override bool Equals(Object other) {
                Cell a = this;
                Cell b = other as Cell;
                if (b == null) return false;
                bool equal;
                if (a.Pixels.Count == 0 || b.Pixels.Count == 0) {
                    equal = a.Pixels.Count == b.Pixels.Count; // both empty
                } else {
                    equal = a.Pixels[0].Equals(b.Pixels[0]); // remember, sorted
                }
                // validate...
                if(equal){
                    Debug.Assert(a.Pixels.Count == b.Pixels.Count);
                    for (int i = 0; i < a.Pixels.Count; i++) {
                        Debug.Assert(a.Pixels[i].Equals(b.Pixels[i]));
                    }
                    Debug.Assert(a.Color == b.Color);
                }
                return equal;
            }
        }

        public CellSpec CellSpec { get; private set; }
    }
}
