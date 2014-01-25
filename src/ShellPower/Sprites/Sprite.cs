using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSCP.ShellPower {
    public abstract class Sprite {
        public Vector4 Position {
            get {
                return Transform.Column3;
            }
            set {
                transform.M14 = value.X;
                transform.M24 = value.Y;
                transform.M34 = value.Z;
                transform.M44 = value.W;
            }
        }
        Matrix4 transform;
        public Matrix4 Transform {
            get { return transform; }
            set { transform = value; }
        }
        public Vector3 Up {
            get {
                return Vector3.Transform(new Vector3(0, 1, 0), Transform);
            }
        }
        public Vector3 Right {
            get {
                return Vector3.Transform(new Vector3(1, 0, 0), Transform);
            }
        }
        public Vector3 Forward {
            get {
                return Vector3.Transform(new Vector3(0, 0, 1), Transform);
            }
        }
        public Sprite() {
            Transform = Matrix4.Identity;
        }
        public abstract void Render();
        public virtual void Initialize() { }
        public virtual void Dispose() {

        }
        public void PushTransform() {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.MultTransposeMatrix(ref transform);
        }
        public void PopTransform() {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }
    }
}

