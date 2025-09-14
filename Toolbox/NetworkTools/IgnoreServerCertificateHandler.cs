using System.Net.Http;

namespace aemarcoCommons.Toolbox.NetworkTools;

public class IgnoreServerCertificateHandler : HttpClientHandler
{
    public IgnoreServerCertificateHandler()
    {
        ServerCertificateCustomValidationCallback =
            (_, _, _, _) => true;
    }
}