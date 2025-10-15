using System.Collections.Generic;

namespace IngameScript
{
    public interface IAsyncSystem : ISystem
    {
        IEnumerator<bool> Run();
    }
}