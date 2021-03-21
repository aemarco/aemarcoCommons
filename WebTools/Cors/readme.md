
define cors policies in a static class
public static class CorsOrigins
{
    [CorsOrigin("https://example.com")]
    [CorsOrigin("https://example2.com")]
    [CorsMethod("post")]
    [CorsMethod("get")]
    [CorsPreflightMaxAge(1800)]
    public const string ExampleCom = "exampleCom";        
}


in Startup:ConfigureServices
services.AddCors(options => options.AddCorsPolicies(typeof(CorsOrigins))); //register class in services


in Startup:Configure
app.UseRouting();
app.UseCors(); //check cors after route
app.UseAuthentication();


Use in Controller as Attribute on Controller or Action
[EnableCors(CorsOrigins.ExampleCom)]

