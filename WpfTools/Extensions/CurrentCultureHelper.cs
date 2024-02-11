using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace aemarcoCommons.WpfTools.Extensions;

//inspired by
//https://youtu.be/6rrgoH6phvE

public static class CurrentCultureHelper
{

    public static void SetDefaults()
    {
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));
        FrameworkElement.FlowDirectionProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight));
    }


}
