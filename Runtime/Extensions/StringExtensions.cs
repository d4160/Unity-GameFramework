namespace d4160.Core
{
    public static class StringExtensions
    {
        public static string ToZeroFormattedString(this int current)
        {
            return current < 10 ? $"0{current}" : current.ToString();
        }
    }
}