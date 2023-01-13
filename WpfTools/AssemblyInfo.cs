using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly
//where the generic resource dictionary is located
//(used if a resource is not found in the page,
// app, or any theme specific resource dictionaries)
)]



[assembly: SupportedOSPlatform("windows")]

[assembly: XmlnsPrefix("http://schemas.aemarco/wpf", "aem")]
[assembly: XmlnsDefinition("http://schemas.aemarco/xaml", "aemarcoCommons.WpfTools.Controls")]
[assembly: XmlnsDefinition("http://schemas.aemarco/xaml", "aemarcoCommons.WpfTools.Converters")]
[assembly: XmlnsDefinition("http://schemas.aemarco/xaml", "aemarcoCommons.WpfTools.Dialogs")]
[assembly: XmlnsDefinition("http://schemas.aemarco/xaml", "aemarcoCommons.WpfTools.Helpers")]
[assembly: XmlnsDefinition("http://schemas.aemarco/xaml", "aemarcoCommons.WpfTools.MarkupExtensions")]

[assembly: InternalsVisibleTo("aemarcoCommons.WpfToolsTests")]
[assembly: InternalsVisibleTo("aemarcoCommons.WpfTools.Tests")]
[assembly: InternalsVisibleTo("WpfToolsTests")]
[assembly: InternalsVisibleTo("WpfTools.Tests")]