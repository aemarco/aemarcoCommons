# `aemarcoToolboxTopService`

<a href=https://www.nuget.org/packages/aemarcoToolboxTopService><img src="https://buildstats.info/nuget/aemarcoToolboxTopService"></a><br/>

## Overview

- HostedService as WindowsService

## Get Started
```
{
   HostApplicationBuilder app = Host.CreateApplicationBuilder(args);
   app.Services.AddHostedService<SampleBackgroundService>();
   await app.RunAsTopService(x => x
    .SetServiceName("SomeService")
    .SetDisplayName("SomeService")
    .SetDescription("SomeService says hello"));
}
```


## Commands
- "install" to install as a windows service
- "uninstall" to uninstall the service
- "start" to start the service
- "stop" to stop the service
