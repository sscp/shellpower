using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace SSCP.ShellPower {
    public class ColorUtils {
        /// <summary>
        /// Returns true if the color is grayscale, ie R == G == B
        /// </summary>
        public static bool IsGrayscale(Color c) {
            return c.R == c.G && c.R == c.B;
        }
        /// <summary>
        /// Sets each pixel alpha to 255 (ie, opaque).
        /// </summary>
        public static void RemoveAlpha(Bitmap bmp) {
            int w = bmp.Width, h = bmp.Height;
            BitmapData dat = bmp.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            unsafe {
                uint* ptr = (uint*)dat.Scan0.ToPointer();
                for (int y = 0; y < h; y++) {
                    for (int x = 0; x < w; x++) {
                        ptr[y * w + x] |= 0xff000000;
                        Debug.Assert(Color.FromArgb(unchecked((int)ptr[y * w + x])).A == 255);
                    }
                }
            }
            bmp.UnlockBits(dat);
        }
    }
}
