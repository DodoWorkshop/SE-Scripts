namespace IngameScript
{
    /*
     * Custom regex to prevent import error
     */
    public class MyRegex : System.Text.RegularExpressions.Regex
    {
        public MyRegex(string pattern) : base(pattern)
        {
        }
    }
}