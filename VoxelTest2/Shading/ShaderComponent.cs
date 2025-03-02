using System;
using System.Collections.Generic;
using System.Linq;
using VoxelTest2.Shading;
using System.Text;
using System.Threading.Tasks;

namespace VoxelTest2.Entity.Components
{
    public class ShaderComponent
    {
        public Shader Shader { get; set; }

        public ShaderComponent(Shader shader)
        {
            Shader = shader;
        }
    }
}
