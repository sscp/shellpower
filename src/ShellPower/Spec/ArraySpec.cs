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
            BypassDiodeSpec = new DiodeSpec();
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
        public BoundsSpec LayoutBounds { get; set; }
        /// <summary>
        /// Defines individual-cell properties, such as area and efficiency.
        /// </summary>
        public CellSpec CellSpec { get; private set; }
        /// <summary>
        /// Specifies the properties of the bypass diodes
        /// (if there are any). 
        /// Mixing diode types not supported.
        /// </summary>
        public DiodeSpec BypassDiodeSpec { get; private set; }
        /// <summary>
        /// Assigns cells to strings (a group of cells in series).
        /// </summary>
        public List<CellString> Strings { get; private set; }
        /// <summary>
        /// Encapsulation loss. 0.03 = 3% loss
        /// </summary>
        public double EncapsulationLoss { get; set; }

        public class CellString {
            public CellString() {
                Cells = new List<Cell>();
                BypassDiodes = new List<BypassDiode>();
                Name = "NewString";
            }
            public List<Cell> Cells { get; private set; }
            public List<BypassDiode> BypassDiodes { get; private set; }
            public String Name { get; set; }
            public override string ToString() {
                string str = Name + " (" + Cells.Count + " cells";
                if (BypassDiodes.Count > 0) {
                    str += ", " + BypassDiodes.Count + " diodes";
                }
                str += ")";
                return str;
            }
        }

        public class BypassDiode {
            /// <summary>
            /// Bypass diodes connects these two cells in the string.
            /// Must be in order.
            /// </summary>
            public Pair<int> CellIxs { get; set; }
            public override int GetHashCode() {
                return CellIxs.GetHashCode();
            }
            public override bool Equals(object obj) {
                BypassDiode other = obj as BypassDiode;
                if (other == null) return false;
                return CellIxs.Equals(other.CellIxs);
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

        public void Recolor() {
            Debug.Assert(LayoutTexture != null);
            if(Strings.Count >= 256){
                throw new InvalidOperationException("Cannot create a layout texture with more than 255 strings.");
            }

            // first, change the cell colors...
            int nstrings = Strings.Count;
            int nsteps = (int)Math.Ceiling(Math.Sqrt(nstrings));
            int colorIx = 0;
            for (int i = 0; i < nstrings; i++) {
                List<Cell> cells = Strings[i].Cells;
                if (cells.Count == 0) continue;
                if ((colorIx / nsteps) == (colorIx % nsteps)) colorIx++;
                int red = 255 * (colorIx / nsteps) / nsteps;
                int green = 255 * (colorIx % nsteps) / nsteps;
                colorIx++;
                int ncells = cells.Count;
                for (int j = 0; j < ncells; j++) {
                    int blue = 255 * j / ncells;
                    cells[j].Color = Color.FromArgb(255, red, green, blue);
                }
            }

            // then, mod the texture
            int texW = LayoutTexture.Width, texH =LayoutTexture.Height;
            BitmapData data = LayoutTexture.LockBits(
                new Rectangle(0, 0, texW, texH),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe {
                int* ptr = (int*)data.Scan0.ToPointer();
                byte* bptr = (byte*)data.Scan0.ToPointer();
                // first, clear
                for (int y = 0; y < texH; y++) {
                    for (int x = 0; x < texW; x++) {
                        int ix = y*texW+x;
                        byte r = bptr[4 * ix + 1];
                        byte g = bptr[4 * ix + 2];
                        byte b = bptr[4 * ix + 3];
                        if (r == g && g == b) {
                            // grayscale
                            int gray = r > 200 ? 200 : r;
                            ptr[ix] = Color.FromArgb(255, gray, gray, gray).ToArgb();
                        } else {
                            // all colors become white
                            ptr[ix] = unchecked((int)0xffffffff);
                        }
                    }
                }
                // then, color
                foreach (CellString cellStr in Strings) {
                    foreach (Cell cell in cellStr.Cells) {
                        foreach (Pair<int> pixel in cell.Pixels) {
                            int x = pixel.First, y = pixel.Second;
                            ptr[y * texW + x] = cell.Color.ToArgb();
                        }
                    }
                }
            }
            LayoutTexture.UnlockBits(data);
        }

        public void ReadStringsFromColors() {
            // first, read out all the strings
            Strings.Clear();
            var cellMap = new Dictionary<Color, Cell>();
            var stringMap = new Dictionary<Color, CellString>();

            int texW = LayoutTexture.Width, texH = LayoutTexture.Height;
            BitmapData data = LayoutTexture.LockBits(
                new Rectangle(0, 0, texW, texH),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try {
                unsafe {
                    int* ptr = (int*)data.Scan0.ToPointer();
                    for (int y = 0; y < texH; y++) {
                        for (int x = 0; x < texW; x++) {
                            Color color = Color.FromArgb(ptr[y * texW + x]);
                            if (color.A != 255) {
                                throw new ArgumentException("Layout texture cannot be transparent.");
                            }
                            if (ColorUtils.IsGrayscale(color)) continue;
                            Color stringColor = Color.FromArgb(255, color.R, color.G, 0);

                            if (!stringMap.ContainsKey(stringColor)) {
                                CellString newString = new CellString();
                                newString.Name = "String " + Strings.Count;
                                Strings.Add(newString);
                                stringMap.Add(stringColor, newString);
                            }

                            if (!cellMap.ContainsKey(color)) {
                                Cell newCell = new Cell();
                                newCell.Color = color;
                                cellMap.Add(color, newCell);
                                stringMap[stringColor].Cells.Add(newCell);
                            }
                            cellMap[color].Pixels.Add(new Pair<int>(x, y));

                        }
                    }
                }
            } finally {
                LayoutTexture.UnlockBits(data);
            }

            // second, go through and sort the cells in each string in the order they're wired
            foreach (CellString cellStr in Strings) {
                cellStr.Cells.Sort((a, b) => {
                    if (a.Color.B < b.Color.B) return -1;
                    if (a.Color.B > b.Color.B) return 1;
                    return 0;
                });
            }
        }
    }
}
