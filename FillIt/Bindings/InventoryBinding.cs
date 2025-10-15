using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class InventoryBinding
    {
        public IMyCargoContainer Block { get; }
        
        public IMyInventory Inventory { get; }
        
        public List<Expectation> Expectations { get; }
        
        public InventoryBinding(IMyCargoContainer block, IMyInventory inventory)
        {
            Block = block;
            Inventory = inventory;
            Expectations = new List<Expectation>();
        }
    }
}