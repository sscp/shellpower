using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
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
        /// This is a vector [x1, z1, x2, z2]. [x1, z1] is the top-left corner
        /// of the texture (in model coordinates, in meters) and [x2, z2]
        /// is the bottom-right corner.
        /// </summary>
        public Vector4d LayoutBoundsXZ { get; set; }
    }
}
