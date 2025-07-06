using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingdom_of_Creation.Dtos
{
    public class CollisionManifold
    {
        public bool HasCollision;
        public Vector_2 Normal;
        public float PenetrationDepth;
    }
}
