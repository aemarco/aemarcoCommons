using System.Net.Http;

namespace aemarcoCommons.Toolbox.NetworkTools
{
    public class IgnoreServerCertificateHandler : HttpClientHandler
    {
        public IgnoreServerCertificateHandler()
        {
            ServerCertificateCustomValidationCallback =
                (r, c, ch, e) => true;
        }
    }
}
