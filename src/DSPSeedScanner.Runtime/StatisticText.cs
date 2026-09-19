namespace DSPSeedScanner.Runtime
{
    public static class StatisticText
    {
        public static string Green(string text) => Color(text, "#80F294");
        public static string Red(string text) => Color(text, "#FF7A6E");
        private static string Color(string text, string color) =>
            text.Length == 0 ? text : "<color=" + color + ">" + text + "</color>";
    }
}
