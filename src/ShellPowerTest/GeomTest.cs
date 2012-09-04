using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;

namespace SSCP.ShellPower.Test {
    [TestClass]
    public class GeomTest {
        [TestMethod]
        public void TestPolygon() {
            Polygon2 poly = new Polygon2() {
                vertices = new Vector2[]{
                    new Vector2(1, 1),
                    new Vector2(2, 1),
                    new Vector2(1, 2)
                }
            };
            Assert.IsTrue(poly.Contains(new Vector2(1.1f, 1.1f)));
            Assert.IsTrue(poly.Contains(new Vector2(1.1f, 1.8f)));
            Assert.IsTrue(poly.Contains(new Vector2(1.49f, 1.49f)));
            Assert.IsFalse(poly.Contains(new Vector2(1.51f, 1.51f)));
            Assert.IsFalse(poly.Contains(new Vector2(0f, 0f)));
            Assert.IsTrue(poly.Contains(new Vector2(1f, 1f)));
        }
    }
}
