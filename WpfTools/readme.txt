
Resource Usage:

//add reference to Resource File, so Converters can by used as Static resource with Typename
 <ResourceDictionary>
	<ResourceDictionary.MergedDictionaries>
		<ResourceDictionary Source="pack://application:,,,/WpfTools;component/Resources.xaml"/>
	</ResourceDictionary.MergedDictionaries>
</ResourceDictionary>

//in xaml
<TextBlock Visibility="{Binding IsVisible, Converter={StaticResource BoolToVisibilityConverter}}"/>



Commands Usage:

//in xaml
<MenuItem Header="Exit" Command="{Binding ExitApplicationCommand}" />
or
<Window Name="ThisWindow">
<Button Command="{Binding SignOutCommand}" CommandParameter="{Binding ElementName=ThisWindow}"/>

//in viewmodel
public ICommand ExitApplicationCommand
{
	get
	{
		return new DelegateCommand
		{
			CanExecuteFunc = () => true,
			CommandAction = () =>
			{
				Application.Current.Shutdown();
			},
			ObjectCommandAction = (p) =>
			{
				var window = p as MainWindow;
				window.Close();
			}
		};
	}
}


EnumConverter Usage:

//in xaml
<ComboBox SelectedItem="{Binding Path=ConfigurationHandler.Category}"/>

//in enum
[TypeConverter(typeof(EnumTypeConverter))]
public enum Category
{
	[Description("All")]
	All
}

//in viewmodel
public Category Category { get; set; }

//in code
TypeDescriptor.GetConverter(Category.All).ConvertTo(Category.All, typeof(string));






Markup Extension Usage:

xmlns:exts="clr-namespace:WpfTools.MarkupExtensions;assembly=WpfTools"
 <ComboBox ItemsSource="{Binding Source={exts:EnumBindingSource {x:Type help:Category}}}" />




 https://github.com/XamlAnimatedGif/WpfAnimatedGif
 https://github.com/charri/Font-Awesome-WPF/blob/master/README-WPF.md
 https://github.com/hardcodet/wpf-notifyicon
 https://github.com/Humanizr/Humanizer


