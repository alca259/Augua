using System.Globalization;

namespace Microsoft.Extensions.Localization;

public static class LocalizeExtensions
{
    public static LocalizedString GetLocalizedString(this IStringLocalizer localizer, string key, string culture, string defaultValue = null)
    {
        var currCulture = Thread.CurrentThread.CurrentCulture.Name;
        var currCultureUI = Thread.CurrentThread.CurrentUICulture.Name;

        Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

        LocalizedString result = localizer[key];

        if (result.ResourceNotFound)
        {
            result = new LocalizedString(result.Name, defaultValue ?? key, result.ResourceNotFound, result.SearchedLocation);
        }

        Thread.CurrentThread.CurrentCulture = new CultureInfo(currCulture);
        Thread.CurrentThread.CurrentUICulture = new CultureInfo(currCultureUI);

        return result;
    }
}
