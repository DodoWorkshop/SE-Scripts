using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    public struct Expectation
    {
        public MyItemType ItemType;

        public int Amount;
        
        public ExpectationRule Rule;

        public Expectation(MyItemType itemType, int amount, ExpectationRule rule)
        {
            ItemType = itemType;
            Amount = amount;
            Rule = rule;
        }
    }
}