using System;

namespace IngameScript
{
    public interface IRepositoryManager
    {
        void LoadStorage(string storage, Action onSuccess = null, Action<Exception> onError = null);

        string SaveToString();
    }
}