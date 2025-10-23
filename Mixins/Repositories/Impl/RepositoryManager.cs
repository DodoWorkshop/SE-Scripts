using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly IItemContainer _itemContainer;
        private readonly MyIni _ini = new MyIni();

        public RepositoryManager(IItemContainer itemContainer)
        {
            _itemContainer = itemContainer;
        }

        private void SaveRepositories(MyIni ini)
        {
            foreach (var repository in _itemContainer.GetAllItems<ISavableRepository>())
            {
                repository.Save(ini);
            }
        }

        private void LoadRepositories(MyIni ini)
        {
            foreach (var repository in _itemContainer.GetAllItems<ISavableRepository>())
            {
                repository.Load(ini);
            }
        }

        public void LoadStorage(string storage, Action onSuccess = null, Action<Exception> onError = null)
        {
            if (string.IsNullOrEmpty(storage)) return;

            try
            {
                if (_ini.TryParse(storage))
                {
                    LoadRepositories(_ini);
                }
                else
                {
                    throw new Exception("Could not parse storage settings.");
                }

                onSuccess?.Invoke();
            }
            catch (Exception e)
            {
                onError?.Invoke(e);
            }
        }

        public string SaveToString()
        {
            _ini.Clear();
            SaveRepositories(_ini);
            return _ini.ToString();
        }
    }
}