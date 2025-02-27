using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTest2.Entity
{
    public class ECS
    {
        public struct Entity
        {
            public int Id { get; private set; }

            public Entity(int id)
            {
                Id = id;
            }
        }
        public class PositionComponent
        {
            public Vector3 Position { get; set; }
        }

        public class VelocityComponent
        {
            public Vector3 Velocity { get; set; }
        }

        public class RenderComponent
        {
            public int VertexArrayObject { get; set; }
            public int VertexBufferObject { get; set; }
            public int ElementBufferObject { get; set; }
        }

    }
}
