using System;

namespace IngameScript
{
    public enum ExpectationRule
    {
        Exact,
        Minimum,
        Maximum
    }

    public static class ExpectationRuleExtensions
    {
        public static ExpectationRule FromString(string value)
        {
            if (string.Equals(value, "min", StringComparison.InvariantCultureIgnoreCase))
            {
                return ExpectationRule.Minimum;
            }
            
            if (string.Equals(value, "max", StringComparison.InvariantCultureIgnoreCase))
            {
                return ExpectationRule.Maximum;
            }

            return ExpectationRule.Exact;
        }
    }
}