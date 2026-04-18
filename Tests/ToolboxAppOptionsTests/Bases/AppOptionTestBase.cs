using aemarcoCommons.ToolboxAppOptions.Transformations;

namespace ToolboxAppOptionsTests.Bases;

public class AppOptionTestBase
{

    protected IConfigurationRoot Config =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

    private IServiceCollection Sc
    {
        get
        {
            var sc = new ServiceCollection()
                .AddConfigOptionsUtils(Config, x =>
                {
                    x.AddAssemblyMarker(typeof(AppOptionTestBase));
                    x.AddStringTransformation(new PlaceholderTransformation());
                });
            return sc;
        }
    }

    protected IServiceProvider Sp
    {
        get
        {
            var sc = Sc;
            var sp = sc.BuildServiceProvider();
            return sp;
        }
    }



    protected static ProtectedTransformer Pt
    {
        get
        {
            var r = new ProtectedTransformer("superNicePassword");
            return r;
        }
    }

}
