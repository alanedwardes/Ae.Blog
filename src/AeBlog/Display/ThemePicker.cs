using System.Collections.Generic;

namespace AeBlog.Display
{
    public static class ThemePicker
    {
        private static IEnumerable<string> THEMES = new List<string> { "046380", "A20D1E", "827b00", "955918", "800463" };

        public static string PickRandomTheme()
        {
            return THEMES.PickRandom();
        }
    }
}
