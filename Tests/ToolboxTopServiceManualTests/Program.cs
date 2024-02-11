_ = await Host.CreateApplicationBuilder(args)
    .SetupSampleService()
    .RunAsTopService(x => x
        .ServiceName("SomeService")
        .DisplayName("Some Service")
        .Description("SampleService installed with TopService")
        .StartupType(StartupType.AutoDelayed));