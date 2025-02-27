using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VoxelTest2.Entity.ECS;

namespace VoxelTest2.Entity
{
    public class ComponentManager
    {
        private Dictionary<Type, Dictionary<int, object>> _components = new Dictionary<Type, Dictionary<int, object>>();

        public void AddComponent<T>(ECS.Entity entity, T component)
        {
            var type = typeof(T);
            if (!_components.ContainsKey(type))
            {
                _components[type] = new Dictionary<int, object>();
            }
            _components[type][entity.Id] = component;
        }

        public T GetComponent<T>(ECS.Entity entity)
        {
            var type = typeof(T);
            if (_components.ContainsKey(type) && _components[type].ContainsKey(entity.Id))
            {
                return (T)_components[type][entity.Id];
            }
            return default(T);
        }

        public void RemoveComponent<T>(ECS.Entity entity)
        {
            var type = typeof(T);
            if (_components.ContainsKey(type))
            {
                _components[type].Remove(entity.Id);
            }
        }
        public List<ECS.Entity> GetEntitiesWithComponent<T>()
        {
            var type = typeof(T);
            var entities = new List<ECS.Entity>();

            if (_components.ContainsKey(type))
            {
                foreach (var entityId in _components[type].Keys)
                {
                    entities.Add(new ECS.Entity(entityId));
                }
            }

            return entities;
        }
    }
}
