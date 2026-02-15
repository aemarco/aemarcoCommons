namespace aemarcoCommons.ToolboxVlc;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection SetupVlc(
        this IServiceCollection services) =>
        services.SetupVlc(_ => { });

    public static IServiceCollection SetupVlc(
        this IServiceCollection services,
        Action<VlcOptionsBuilder> options) =>
        services.SetupVlc(
            (_, opt) => options(opt));


    public static IServiceCollection SetupVlc(
        this IServiceCollection services,
        Action<IServiceProvider, VlcOptionsBuilder> options)
    {
        services.AddSingleton<VlcOptions>(sp =>
        {
            var builder = new VlcOptionsBuilder();
            options(sp, builder);
            return builder.Build();
        });

        services.AddTransient<VlcPlayer>();
        services.AddHttpClient<IVlcClient, VlcClient>();
        return services;
    }

}
