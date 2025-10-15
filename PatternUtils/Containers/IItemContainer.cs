using System.Collections.Generic;

namespace IngameScript
{
    public interface IItemContainer
    {
        void RegisterItem<T>(T item) where T : class, IContainerItem;

        T GetItem<T>() where T : class, IContainerItem;

        List<T> GetAllItems<T>() where T : class, IContainerItem;
    }
}