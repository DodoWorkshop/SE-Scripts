using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public class DockGroupRepository : IRepository
    {
        private readonly Dictionary<string, DockGroup> _data = new Dictionary<string, DockGroup>();

        public void RegisterGroup(DockGroup group)
        {
            _data.Add(group.Name, group);
        }

        public DockGroup GetGroup(string name)
        {
            return _data.GetValueOrDefault(name);
        }

        public DockGroup[] GetAll()
        {
            return _data.Values.ToArray();
        }

        public void DeleteGroup(string name)
        {
            _data.Remove(name);
        }
    }
}