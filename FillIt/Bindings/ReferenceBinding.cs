using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public struct ReferenceBinding
    {
        private readonly IMyGridTerminalSystem _gridTerminalSystem;

        public List<InventoryBinding> ContainerDatas { get; }

        public ReferenceBinding(Program program)
        {
            _gridTerminalSystem = program.GridTerminalSystem;
            ContainerDatas = new List<InventoryBinding>();

            ComputeContainerDatas();
        }

        public void ComputeContainerDatas()
        {
            // Compute containers to stop handling
            var dataToRemove = ContainerDatas
                .Where(containerData => !containerData.Block.CustomName.Contains(Program.AutoFilledContainerTag))
                .ToList();

            foreach (var containerData in dataToRemove)
            {
                // Clear custom data
                containerData.Block.CustomData = "";

                ContainerDatas.Remove(containerData);
            }

            // Collect containers
            var containersToHandle = _gridTerminalSystem
                .GetBlockWithTag<IMyCargoContainer>(Program.AutoFilledContainerTag);

            foreach (var container in containersToHandle)
            {
                if (ContainerDatas.Any(cd => cd.Block.EntityId.Equals(container.EntityId)))
                {
                    continue;
                }

                var containerData = new InventoryBinding(container, container.GetInventory());

                ContainerDatas.Add(containerData);
            }
        }
    }
}