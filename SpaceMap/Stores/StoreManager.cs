using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class StoreManager : IStoreManager
    {
        private readonly Dictionary<Type, IStore> _stores = new Dictionary<Type, IStore>();
        
        public void RegisterStore<T>(T store) where T : class, IStore
        {
            var type = typeof(T);
            if (_stores.ContainsKey(type))
            {
                throw new Exception($"Store already registered for type {type}");
            }
            _stores.Add(type, store);
        }

        public T GetStore<T>() where T : class, IStore
        {
            IStore store;
            if (!_stores.TryGetValue(typeof(T), out store))
            {
                throw new Exception($"No store for type {typeof(T)} registered");
            }
            
            return store as T;
        }
    }
}