using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace SSCP.ShellPower {
    public interface IArea {
        bool Contains(Vector2 vec);
    }

    /// <summary>
    /// Represents OR and NOR operators.
    /// 
    /// If a point is in *any* of the include shapes, but *not in any* of the exclude shapes, 
    /// then it is in the compound shape.
    /// </summary>
    public class CompoundShape2 : IArea {
        public List<IArea> include = new List<IArea>();
        public List<IArea> exclude = new List<IArea>();
        public bool Contains(Vector2 vec) {
            bool contains = false;
            foreach(IArea shape in include){
                if(shape.Contains(vec)){
                    contains = true;
                    break;
                }
            }
            if (contains) {
                foreach (IArea shape in exclude) {
                    if (shape.Contains(vec)) {
                        contains = false;
                        break;
                    }
                }
            }
            return contains;
        }
    }
}
