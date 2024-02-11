# `aemarcoToolboxTopService`

<a href=https://www.nuget.org/packages/aemarcoToolboxTopService><img src="https://buildstats.info/nuget/aemarcoToolboxTopService"></a><br/>

## Overview
This package injects some commands into the Application, to make it easier to use.
The behaviour of it is somewhat inspired by "Topshelf".

- HostedService as WindowsService
- Use Commands directly on the app (see Commands section)

## Get Started
```
{
   HostApplicationBuilder app = Host.CreateApplicationBuilder(args);
   app.Services.AddHostedService<SampleBackgroundService>();
   await app.RunAsTopService(x => x
    .ServiceName("SomeService")
    .DisplayName("Some Service")
    .Description("SampleService installed with TopService"));
}
```

## Commands
- "install" to install as a windows service
- "uninstall" to uninstall the service
- "start" to start the service
- "stop" to stop the service

## Startup Type
```
{
    x.StartupType(StartupType.AutoDelayed));
}
```
