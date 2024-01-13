# `aemarcoToolboxAppOptions`

<a href=https://www.nuget.org/packages/aemarcoToolboxAppOptions><img src="https://buildstats.info/nuget/aemarcoToolboxAppOptions"></a><br/>



## Overview


This package provides a opinionated approach to the Options pattern described in the [Microsoft docs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-8.0).
With minimal setup, all the Option classes will be registered in the IOC container and mapped to the IConfiguration source.

- Automatic mapping of sections to classes (convention driven, but can be adjusted)
- Beside registering the interfaces from Microsoft, registers the class itself as well (singleton)
- Registers also interfaces which you put on the settings class
- Allows text transformations between IConfiguration and the settings class
- Option to use fluent validation on settings
- Option to validate settings during startup (on by default, opt-out in options)


## Get Started

During startup of the app
```
{
  services.AddConfigOptionsUtils(config);
}
```
Create your class representing your options
```
{
  public class MySettings : ISettingsBase
  {
    public string? Text { get; set; }
    public bool Enable { get; set; }
  }
}
```

In appsetting.json (or other IConfiguration source)
```
{
    "MySettings": {
        
        "Text": "SomeText",
        "Enable": true
    }
}
```



## StringTransformation
String transformations will be executed in the order defined in the setup process.
You could define your own, but PlaceholderTransformation is built in already.
```
{
    services.AddConfigOptionsUtils(
        config,
        x => x
            .AddStringTransformation(new PlaceholderTransformation());
}
```

PlaceholderTransformation does resolve placeholders {{{...}}} through the entire IConfguration by Key.
That maybe usefull when piecing together file or url path.
```
{
    "Message": "Hello world!",
    "MySettings": {
        
        "Text": "The message is {{{Message}}}",
        "Enable": true
    }
}
```



## Fluent Validation
Just define your Validators in the assemblies where setting classes are defined.
The tool will register them, and use them if present. By default, Validation takes place at startup.
```
{
    services.AddConfigOptionsUtils(
        config,
        x => x
            .EnableValidationOnStartup(false); //on by default
}
```



## Setting classes in multiple assemblies
The tool relies on assembly scanning during the setup. If your option classes are in other assemblies,
you may pass one of the types as assembly marker, and/or use the assemblies itself.
```
{
    services.AddConfigOptionsUtils(
        config,
        x => x
            .AddAssemblyMarker(typeof(MySettings))
            // and/or
            .AddAssemblies(someAssemblies));
}
```


## Mapping Path
By default, the class name is exactly matched to the root level of the configuration.
You may define your own path in the settings class

```
{
  [SettingsPath("Settings")]
  public class MySettings : ISettingsBase
  {
    public string? Text { get; set; }
    public bool Enable { get; set; }
  }
}
```
would map to
```
{
    "Settings": {
        
        "Text": "SomeText",
        "Enable": true
    }
}
```

You could even do it nested, just use the colon seperated path as you would with GetSection().

```
{
  [SettingsPath("Settings:Nested")]
  public class MySettings : ISettingsBase
  {
    public string? Text { get; set; }
    public bool Enable { get; set; }
  }
}
```
would map to
```
{
    "Settings": {
        "Nested":{
            "Text": "SomeText",
            "Enable": true
        }
    }
}
```









