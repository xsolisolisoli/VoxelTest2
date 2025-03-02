using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VoxelTest2.Entity.ECS;
using VoxelTest2.Entity;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using VoxelTest2.Entity.Components;

namespace VoxelTest2.Rendering
{
    public class Renderer
    {
        private ComponentManager _componentManager;

        public Renderer(ComponentManager componentManager)
        {
            _componentManager = componentManager;
        }

        public void Render()
        {
            foreach (var entity in _componentManager.GetEntitiesWithComponent<RenderComponent>())
            {
                var renderComponent = _componentManager.GetComponent<RenderComponent>(entity);
                var shaderComponent = _componentManager.GetComponent<ShaderComponent>(entity);

                if (renderComponent != null && shaderComponent != null)
                {
                    shaderComponent.Shader.Use();

                    GL.BindVertexArray(renderComponent.VertexArrayObject);
                    GL.DrawElements(PrimitiveType.Triangles, renderComponent.ElementCount, DrawElementsType.UnsignedInt, 0);
                }
            }
        }
    }
}
