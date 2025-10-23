using System;
using System.Collections.Generic;

namespace IngameScript
{
    public interface IItemContainer
    {
        void RegisterItem(object item, Type type);

        void RegisterItem(object item);

        void RegisterItem<T>(T item);

        T GetItem<T>();

        List<T> GetAllItems<T>();
    }
}