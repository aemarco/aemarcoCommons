using System;
using System.Net.Http;

namespace aemarcoCommons.Toolbox.GeoTools
{

    public partial interface IGeoServiceSettings { }
    internal partial class GeoServiceSettings : IGeoServiceSettings { }

    public partial class GeoService
    {

        private readonly IGeoServiceSettings _geoServiceSettings;
        private readonly HttpClient _geoClient;
        private GeoService(
            IGeoServiceSettings geoServiceSettings,
            HttpClient client)
        {
            _geoServiceSettings = geoServiceSettings;
            _geoClient = client;

            if (_geoServiceSettings.NumberOfCachedSunriseSunsetInfos < 0)
                throw new ArgumentException("NumberOfCachedSunriseSunsetInfos must be >= 0", nameof(geoServiceSettings));
        }

        public GeoService(IGeoServiceSettings geoServiceSettings)
            : this(
                geoServiceSettings,
                new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (r, c, chain, errors) => true
                }))
        { }

        public GeoService()
            : this(new GeoServiceSettings())
        { }

        public GeoService(IHttpClientFactory httpClientFactory)
            : this(new GeoServiceSettings(), httpClientFactory)
        { }


        public GeoService(IGeoServiceSettings geoServiceSettings, IHttpClientFactory httpClientFactory)
            : this(geoServiceSettings, httpClientFactory.CreateClient(nameof(GeoService)))
        { }

    }

}

