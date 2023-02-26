using aemarcoCommons.ToolboxAppOptions.Transformations;

namespace ToolboxAppOptionsTests.Bases;
public class AppOptionTestBase
{
    protected IConfigurationRoot Config
    {
        get
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }


    protected IServiceCollection Sc
    {
        get
        {
            var sc = new ServiceCollection()
                .AddConfigOptionsUtils(Config, x =>
                {
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



    protected ProtectedTransformer Pt
    {
        get
        {
            var r = new ProtectedTransformer("superNicePassword");
            return r;
        }
    }

}
