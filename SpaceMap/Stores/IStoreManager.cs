namespace IngameScript
{
    public interface IStoreManager
    {
        void RegisterStore<T>(T store) where T : class, IStore;
    
        T GetStore<T>() where T : class, IStore;
    }
}