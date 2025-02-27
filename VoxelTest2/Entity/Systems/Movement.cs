using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VoxelTest2.Entity.ECS;

namespace VoxelTest2.Entity.Systems
{
    internal class Movement
    {
        private ComponentManager _componentManager;

        public Movement(ComponentManager componentManager)
        {
            _componentManager = componentManager;
        }

        public void Update(float deltaTime)
        {
            foreach (var entity in _componentManager.GetEntitiesWithComponent<PositionComponent>())
            {
                var position = _componentManager.GetComponent<PositionComponent>(entity);
                var velocity = _componentManager.GetComponent<VelocityComponent>(entity);

                if (position != null && velocity != null)
                {
                    position.Position += velocity.Velocity * deltaTime;
                }

            }
        }
