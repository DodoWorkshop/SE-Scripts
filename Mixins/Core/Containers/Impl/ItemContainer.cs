using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public class ItemContainer : IItemContainer
    {
        private readonly Dictionary<Type, object> _data = new Dictionary<Type, object>();

        public void RegisterItem<T>(T item)
        {
            RegisterItem(item, typeof(T));
        }

        public void RegisterItem(object item, Type type)
        {
            if (_data.ContainsKey(type))
            {
                throw new Exception($"Item already registered for type {type}");
            }

            _data.Add(type, item);
        }

        public void RegisterItem(object item)
        {
            RegisterItem(item, item.GetType());
        }

        public T GetItem<T>()
        {
            var result = (T)_data.FirstOrDefault(x => x.Value is T).Value;
            if (result == null)
            {
                throw new Exception($"No item of type {typeof(T)} registered");
            }

            return result;
        }

        public List<T> GetAllItems<T>()
        {
            return _data.Where(entry => entry.Value is T)
                .Select(entry => (T)entry.Value)
                .ToList();
        }
    }
}