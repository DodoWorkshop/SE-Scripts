using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public class ItemContainer : IItemContainer
    {
        private readonly Dictionary<Type, IContainerItem> _data = new Dictionary<Type, IContainerItem>();

        public void RegisterItem<T>(T repository) where T : class, IContainerItem
        {
            var type = typeof(T);
            if (_data.ContainsKey(type))
            {
                throw new Exception($"Item already registered for type {type}");
            }

            _data.Add(type, repository);
        }

        public T GetItem<T>() where T : class, IContainerItem
        {
            var result = _data.FirstOrDefault(x => x.Value is T).Value as T;
            if (result == null)
            {
                throw new Exception($"No item of type {typeof(T)} registered");
            }
            return result;
        }

        public List<T> GetAllItems<T>() where T : class, IContainerItem
        {
            return _data.Where(entry => entry.Value is T)
                .Select(entry => entry.Value as T)
                .ToList();
        }
    }
}