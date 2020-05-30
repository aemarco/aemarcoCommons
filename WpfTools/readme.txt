
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







Converter Usage:

xmlns:conv="clr-namespace:WpfTools.Converters;assembly=WpfTools"

<Window.Resources>
	<conv:DoubleToStringValueConverter x:Key="DoubleToStringValueConverter" />
	<conv:BoolToVisibilityValueConverter x:Key="BoolToVisibilityValueConverter" />
</Window.Resources>



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
