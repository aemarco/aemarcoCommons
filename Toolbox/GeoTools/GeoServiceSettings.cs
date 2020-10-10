using System;

namespace aemarcoCommons.Toolbox.GeoTools
{
    public interface IGeoServiceSettings
    {
        TimeSpan MinIntervalOfIpInfoUpdate { get; }
        int NumberOfCachedSunriseSunsetInfos { get; }
    }

    public class GeoServiceSettings : IGeoServiceSettings
    {
        public TimeSpan MinIntervalOfIpInfoUpdate { get; set; } = TimeSpan.FromMinutes(15);
        public int NumberOfCachedSunriseSunsetInfos { get; set; } = 10;
    }
}
