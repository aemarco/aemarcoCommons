using aemarcoCommons.Extensions.TimeExtensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aemarcoCommons.Toolbox.GeoTools;

public partial interface IGeoServiceSettings
{
    TimeSpan MinIntervalOfIpInfoUpdate { get; }
}

internal partial class GeoServiceSettings
{
    public TimeSpan MinIntervalOfIpInfoUpdate { get; set; } = TimeSpan.FromMinutes(15);
}

public partial class GeoService
{

    private string _lastIpInfo;
    private DateTimeOffset? _lastInInfoTimestamp;
    public async Task<string> GetIpInfo(
        bool throwExceptions = false,
        TimeSpan? maximumInfoAge = null)
    {
        maximumInfoAge = maximumInfoAge ?? _geoServiceSettings.MinIntervalOfIpInfoUpdate;
        if (!string.IsNullOrWhiteSpace(_lastIpInfo) &&
            _lastInInfoTimestamp.HasValue &&
            _lastInInfoTimestamp.Value.IsYoungerThan(maximumInfoAge.Value))
        {
            return _lastIpInfo;
        }
        string result = null;


        var errors = new List<Exception>(2);
        try
        {
            using (var response = await _geoClient
                       .GetAsync("https://api.ipify.org/")
                       .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                var ipText = await response.Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false);
                if (IPAddress.TryParse(ipText, out IPAddress test))
                    result = test.ToString();
            }
        }
        catch (Exception ex)
        {
            errors.Add(ex);
        }
        if (string.IsNullOrWhiteSpace(result))
        {
            try
            {
                using var response = await _geoClient.GetAsync("http://checkip.dyndns.org")
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var info = await response.Content.ReadAsStringAsync();
                var match = Regex.Match(info, @"(?:\d{1,3}.){3}.\d{1,3}");
                if (match.Success && IPAddress.TryParse(match.Value, out IPAddress test))
                    result = test.ToString();
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                if (throwExceptions)
                    throw new AggregateException(errors);
            }
        }



        if (result != null)
        {
            _lastIpInfo = result;
            _lastInInfoTimestamp = DateTimeOffset.Now;
        }
        return result;
    }

}