using OpenTK;

namespace SSCP.ShellPower {
    /// <summary>
    /// Represents a 3x3 matrix of 32bit floats.
    /// </summary>
    public struct Matrix3 {
        public static readonly Matrix3 Identity = new Matrix3(
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1));

        private float[,] elems;
        public Matrix3(Vector3 col1, Vector3 col2, Vector3 col3) {
            elems = new float[3, 3];
            elems[0, 0] = col1.X;
            elems[0, 1] = col2.X;
            elems[0, 2] = col3.X;
            elems[1, 0] = col1.Y;
            elems[1, 1] = col2.Y;
            elems[1, 2] = col3.Y;
            elems[2, 0] = col1.Z;
            elems[2, 1] = col2.Z;
            elems[2, 2] = col3.Z;
        }
        public static Matrix3 Slice(Matrix4 rhs) {
            return new Matrix3(
                new Vector3(rhs.M11, rhs.M21, rhs.M31),
                new Vector3(rhs.M12, rhs.M22, rhs.M32),
                new Vector3(rhs.M13, rhs.M23, rhs.M33));
        }
        private void Swap(int i1, int j1, int i2, int j2) {
            float temp = elems[i1, j1];
            elems[i1, j1] = elems[i2, j2];
            elems[i2, j2] = temp;
        }
        public void Transpose() {
            for (int i = 1; i < 3; i++)
                for (int j = 1; j < 3; j++)
                    Swap(i, j, j, i);
        }
        public float this[int i, int j] {
            get {
                return elems[i, j];
            }
            set {
                elems[i, j] = value;
            }
        }
        public Matrix3 Inverse {
            get {
                Matrix3 inverse = new Matrix3();
                float invdet = 1.0f / Determinant;
                for (int i = 0; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        inverse[i, j] =
                            invdet *
                            ((i + j) % 2 == 0 ? 1 : -1) *
                            (elems[(i + 1) % 3, (j + 1) % 3] * elems[(i + 2) % 3, (j + 2) % 3] -
                            elems[(i + 2) % 3, (j + 1) % 3] * elems[(i + 1) % 3, (j + 2) % 3]);
                    }
                }
                return inverse;
            }
        }
        public float Determinant {
            get {
                float det = 0;
                for (int j = 0; j < 3; j++) {
                    det += (j % 2 == 0 ? 1 : -1) *
                        (elems[1, (j + 1) % 3] * elems[2, (j + 2) % 3] -
                        elems[2, (j + 1) % 3] * elems[1, (j + 2) % 3]);
                }
                return det;
            }
        }
        public static Vector3 operator *(Matrix3 a, Vector3 b) {
            Vector3 product = new Vector3(
                a.elems[0, 0] * b.X +
                a.elems[1, 0] * b.Y +
                a.elems[2, 0] * b.Z,

                a.elems[0, 1] * b.X +
                a.elems[1, 1] * b.Y +
                a.elems[2, 1] * b.Z,

                a.elems[0, 2] * b.X +
                a.elems[1, 2] * b.Y +
                a.elems[2, 2] * b.Z);
            return product;
        }
    }
}
