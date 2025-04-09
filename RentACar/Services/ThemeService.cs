using System;
using System.Linq;
using System.Windows;

namespace RentACar.Services
{
    public class ThemeService
    {
        public enum ThemeType
        {
            Light,
            Dark,
            Sea
        }

        public static void ChangeTheme(ThemeType theme)
        {
            string themePath = $"Themes/{theme}Theme.xaml";
            var dict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };

            // Remove existing theme
            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Themes/"));
            if (oldDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
