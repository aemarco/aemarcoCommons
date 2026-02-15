namespace aemarcoCommons.ToolboxTypeStore;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection SetupTypeStore(
        this IServiceCollection services) =>
        services.SetupTypeStore(_ => { });

    public static IServiceCollection SetupTypeStore(
        this IServiceCollection services,
        Action<TypeToFileSettings> options) =>
        services.SetupTypeStore(
            (_, opt) => options(opt));

    public static IServiceCollection SetupTypeStore(
        this IServiceCollection services,
        Action<IServiceProvider, TypeToFileSettings> options)
    {
        services.AddSingleton<TypeToFileSettings>(sp =>
        {
            var settings = new TypeToFileSettings();
            options(sp, settings);
            return settings;
        });

        services.AddSingleton(typeof(ITypeToFileStore<>), typeof(TypeToFileStore<>));

        return services;
    }

}
