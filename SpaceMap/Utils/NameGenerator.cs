using System.Text;

namespace IngameScript
{
    public static class NameGenerator
    {
        private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-+*/\\";
        private static readonly int Base = Alphabet.Length;

        private static ulong MixBits(long input)
        {
            var x = (ulong)input;
            x = (x ^ (x >> 30)) * 0xbf58476d1ce4e5b9UL;
            x = (x ^ (x >> 27)) * 0x94d049bb133111ebUL;
            x = x ^ (x >> 31);
            return x;
        }

        public static string Generate(long value, int codeLength = 7)
        {
            var mixed = MixBits(value);

            var sb = new StringBuilder();
            for (var i = 0; i < codeLength; i++)
            {
                var index = (int)(mixed % (ulong)Base);
                sb.Append(Alphabet[index]);
                mixed /= (ulong)Base;
            }

            return sb.ToString();
        }
    }
}