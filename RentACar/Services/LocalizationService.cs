using System;
using System.Linq;
using System.Windows;

namespace RentACar.Services
{
    public class LocalizationService
    {
        public enum Language
        {
            English,
            Serbian,
            German
        }

        public static void ChangeLanguage(Language language)
        {
            string langCode = language switch
            {
                Language.English => "en",
                Language.Serbian => "sr",
                Language.German => "de",
                _ => "en"
            };

            string langPath = $"Languages/Strings.{langCode}.xaml";
            var dict = new ResourceDictionary { Source = new Uri(langPath, UriKind.Relative) };

            var oldDict = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Languages/"));
            if (oldDict != null)
                Application.Current.Resources.MergedDictionaries.Remove(oldDict);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}