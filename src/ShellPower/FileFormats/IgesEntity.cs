using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    public abstract class IgesEntity {
        public abstract int EntityTypeNumber { get; }
    }
    public class IgesRationalBSplineSurfaceEntity : IgesEntity {
        public override int EntityTypeNumber {
            get {
                return 128;
            }
        }
        public bool[] Props { get; internal set; }
        public int UControlPoints { get; internal set; }
        public int VControlPoints { get; internal set; }
        public int UDegree { get; internal set; }
        public int VDegree { get; internal set; }
        public Vector3d[,] ControlPoints { get; internal set; }
        public double[,] Weights { get; internal set; }
        public double[] UKnots { get; internal set; }
        public double[] VKnots { get; internal set; }
        public double UMin { get; internal set; }
        public double UMax { get; internal set; }
        public double VMin { get; internal set; }
        public double VMax { get; internal set; }

        public void Render(int ures, int vres) {
            Vector3d[,] grid = new Vector3d[ures, vres];
            for (int i = 0; i < ures; i++) {
                for (int j = 0; j < vres; j++) {
                    double u = UMin + (UMax - UMin) * i / (ures - 1);
                    double v = VMin + (VMax - VMin) * j / (vres - 1);
                    grid[i, j] = Evaluate(u, v);
                }
            }

            GL.Begin(BeginMode.Triangles);
            for (int i = 0; i < ures - 1; i++) {
                for (int j = 0; j < vres - 1; j++) {
                    /* TODO: real normals calculated from nurb */
                    Vector3d norm1 = Vector3d.Cross(
                        grid[i + 1, j] - grid[i, j],
                        grid[i, j + 1] - grid[i, j]);
                    norm1.NormalizeFast();
                    Vector3d norm2 = Vector3d.Cross(
                        grid[i, j + 1] - grid[i + 1, j + 1],
                        grid[i + 1, j] - grid[i + 1, j + 1]);
                    norm2.NormalizeFast();

                    GL.Normal3(norm1);
                    GL.Vertex3(grid[i, j]);
                    GL.Normal3(norm2);
                    GL.Vertex3(grid[i + 1, j]);
                    GL.Normal3(norm1);
                    GL.Vertex3(grid[i, j + 1]);

                    GL.Normal3(norm1);
                    GL.Vertex3(grid[i, j + 1]);
                    GL.Normal3(norm2);
                    GL.Vertex3(grid[i + 1, j]);
                    GL.Normal3(norm2);
                    GL.Vertex3(grid[i + 1, j + 1]);
                }
            }
            GL.End();
        }
        /* this function is a crime against computers everywhere,
         * and it would make gene golum cry if he were still alive.
         */
        public Vector3d Evaluate(double u, double v) {
            double[,] uBasis = new double[UControlPoints + UDegree, UDegree];
            double[,] vBasis = new double[VControlPoints + VDegree, VDegree];
            CalculateBasis(uBasis, UKnots, u);
            CalculateBasis(vBasis, VKnots, v);

            double sumWeights = 0.0;
            Vector3d val = new Vector3d();
            for (int i = 0; i < UControlPoints; i++) {
                for (int j = 0; j < VControlPoints; j++) {
                    double weight =
                        uBasis[i, UDegree - 1]
                        * vBasis[j, VDegree - 1]
                        * Weights[i, j];
                    sumWeights += weight;
                    val += weight * ControlPoints[i, j];
                }
            }
            sumWeights += 0.00001;
            val /= sumWeights;
            return val;
        }
        void CalculateBasis(double[,] basis, double[] knots, double t) {
            int k = basis.GetLength(0);
            int degree = basis.GetLength(1);
            for (int i = 1; i < k; i++) {
                if (t <= knots[i]) {
                    basis[i - 1, 0] = 1;
                    break;
                }
            }
            for (int j = 1; j < degree; j++) {
                for (int i = 0; i < k - j; i++) {
                    double f = (t - knots[i]) / (knots[i + j] - knots[i]);
                    if (f > 0 && f <= 1)
                        basis[i, j] += f * basis[i, j - 1];
                    double g1 = (knots[i + 1] - t) / (knots[i + j + 1] - knots[i + 1]);
                    if (g1 > 1)
                        break;
                    else if (g1 > 0)
                        basis[i, j] += g1 * basis[i + 1, j - 1];
                }
            }
        }
    }
}
