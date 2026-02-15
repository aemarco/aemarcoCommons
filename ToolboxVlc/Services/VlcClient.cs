namespace aemarcoCommons.ToolboxVlc.Services;

internal class VlcClient : IVlcClient
{

    private readonly VlcOptions _vlcOptions;
    private readonly HttpClient _httpClient;
    private readonly ILogger<VlcClient> _logger;
    public VlcClient(
        VlcOptions vlcOptions,
        HttpClient httpClient,
        ILogger<VlcClient> logger)
    {
        _vlcOptions = vlcOptions;
        _httpClient = httpClient;
        _logger = logger;

        var auth64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($":{vlcOptions.HttpPassword}"));
        _httpClient.BaseAddress = new Uri("http://127.0.0.1:8080");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth64);
    }

    //https://wiki.videolan.org/Control_VLC_via_a_browser/
    public string HttpPassword => _vlcOptions.HttpPassword;

    public async Task<VlcStatus?> GetStatus(CancellationToken cancellationToken = default)
    {
        using var resp = await _httpClient.GetAsync(
                "/requests/status.json",
                cancellationToken)
            .ConfigureAwait(false);

        if (!resp.IsSuccessStatusCode)
            return null;

        var status = await resp.Content.ReadFromJsonAsync<StatusResponse>(cancellationToken)
            .ConfigureAwait(false);

        if (status is null)
            return null;

        var result = status.ToVlcStatus();
        _logger.LogDebug("Received Status {@status}", result);
        return result;
    }

    internal record StatusResponse(
        [property: JsonPropertyName("time")] int Time,
        [property: JsonPropertyName("volume")] int Volume,
        [property: JsonPropertyName("length")] int Length,
        [property: JsonPropertyName("state")] string State);
}





// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

//private const string StatusSchema = """
//    {
//      "fullscreen":0,
//      "stats":{
//        "inputbitrate":0,
//        "sentbytes":0,
//        "lostabuffers":0,
//        "averagedemuxbitrate":0,
//        "readpackets":0,
//        "demuxreadpackets":0,
//        "lostpictures":0,
//        "displayedpictures":0,
//        "sentpackets":0,
//        "demuxreadbytes":0,
//        "demuxbitrate":0,
//        "playedabuffers":0,
//        "demuxdiscontinuity":0,
//        "decodedaudio":0,
//        "sendbitrate":0,
//        "readbytes":0,
//        "averageinputbitrate":0,
//        "demuxcorrupted":0,
//        "decodedvideo":0
//      },
//      "seek_sec":10,
//      "apiversion":3,
//      "currentplid":3,
//      "time":0,
//      "volume":118,
//      "length":132,
//      "random":false,
//      "audiofilters":{
//"filter_0":""
//      },
//      "information":{
//"chapter":0,
//        "chapters":[],
//        "title":0,
//        "category":{
//    "Stream 0":{
//        "Ausrichtung":"Oben links",
//            "Bildwiederholrate":"23.976024",
//            "Pufferabmessungen":"428x240",
//            "Typ":"Video",
//            "Videoauflösung":"428x240",
//            "Codec":"H264 - MPEG-4 AVC (part 10) (avc1)"
//          },
//          "Stream 1":{
//        "Bitrate":"128 kB/s",
//            "Codec":"MPEG AAC Audio (mp4a)",
//            "Bits_pro_Sample":"16",
//            "Abtastrate":"44100 Hz",
//            "Typ":"Audio"
//          },
//          "meta":{
//        "filename":"20998_VID-20180528-WA0001.mp4"
//          }
//},
//        "titles":[]
//      },
//      "rate":1,
//      "videoeffects":{
//"hue":0,
//        "saturation":1,
//        "contrast":1,
//        "brightness":1,
//        "gamma":1
//      },
//      "state":"playing",
//      "loop":false,
//      "version":"3.0.20 Vetinari",
//      "position":0,
//      "audiodelay":0,
//      "repeat":false,
//      "subtitledelay":0,
//      "equalizer":[]
//    }
//    """;







//public record VlcStatus(
//[property: JsonProperty("fullscreen")] bool Fullscreen,
//[property: JsonProperty("stats")] VlcStats Stats,
//[property: JsonProperty("aspectratio")] string AspectRatio,
//[property: JsonProperty("seek_sec")] int SeekSeconds,
//[property: JsonProperty("apiversion")] int ApiVersion,
//[property: JsonProperty("currentplid")] int CurrentPlId,
//[property: JsonProperty("time")] int Time,
//[property: JsonProperty("volume")] int Volume,
//[property: JsonProperty("length")] int Length,
//[property: JsonProperty("random")] bool Random,
//[property: JsonProperty("audiofilters")] VlcAudioFilters AudioFilters,
//[property: JsonProperty("information")] VlcInformation Information,
//[property: JsonProperty("rate")] double Rate,
//[property: JsonProperty("videoeffects")] VlcVideoEffects VideoEffects,
//[property: JsonProperty("state")] string State
//[property: JsonProperty("loop")] bool Loop,
//[property: JsonProperty("version")] string Version,
//[property: JsonProperty("position")] double Position,
//[property: JsonProperty("audiodelay")] int AudioDelay,
//[property: JsonProperty("repeat")] bool Repeat,
//[property: JsonProperty("subtitledelay")] int SubtitleDelay,
//[property: JsonProperty("equalizer")] List<string> Equalizer
//);

//public record VlcStats(
//    [property: JsonProperty("inputbitrate")] double InputBitrate,
//    [property: JsonProperty("sentbytes")] int SentBytes,
//    [property: JsonProperty("lostabuffers")] int LostAudioBuffers,
//    [property: JsonProperty("averagedemuxbitrate")] double AverageDemuxBitrate,
//    [property: JsonProperty("readpackets")] int ReadPackets,
//    [property: JsonProperty("demuxreadpackets")] int DemuxReadPackets,
//    [property: JsonProperty("lostpictures")] int LostPictures,
//    [property: JsonProperty("displayedpictures")] int DisplayedPictures,
//    [property: JsonProperty("sentpackets")] int SentPackets,
//    [property: JsonProperty("demuxreadbytes")] int DemuxReadBytes,
//    [property: JsonProperty("demuxbitrate")] double DemuxBitrate,
//    [property: JsonProperty("playedabuffers")] int PlayedAudioBuffers,
//    [property: JsonProperty("demuxdiscontinuity")] int DemuxDiscontinuity,
//    [property: JsonProperty("decodedaudio")] int DecodedAudio,
//    [property: JsonProperty("sendbitrate")] double SendBitrate,
//    [property: JsonProperty("readbytes")] int ReadBytes,
//    [property: JsonProperty("averageinputbitrate")] double AverageInputBitrate,
//    [property: JsonProperty("demuxcorrupted")] int DemuxCorrupted,
//    [property: JsonProperty("decodedvideo")] int DecodedVideo
//);

//public record VlcAudioFilters(
//    [property: JsonProperty("filter_0")] string Filter0
//);

//public record VlcInformation(
//[property: JsonProperty("chapter")] int Chapter,
//[property: JsonProperty("chapters")] List<object> Chapters,
//[property: JsonProperty("title")] int Title,
//[property: JsonProperty("category")] VlcCategory Category,
//[property: JsonProperty("titles")] List<object> Titles
//);

//public record VlcCategory(
//[property: JsonProperty("Stream 0")] VlcStream Stream0,
//[property: JsonProperty("Stream 1")] VlcStream Stream1,
//    [property: JsonProperty("meta")] VlcMeta Meta
//);

//public record VlcStream(
//    [property: JsonProperty("Ausrichtung")] string Orientation,
//    [property: JsonProperty("Bildwiederholrate")] double FrameRate,
//    [property: JsonProperty("Pufferabmessungen")] string BufferDimensions,
//    [property: JsonProperty("Typ")] string Type,
//    [property: JsonProperty("Videoauflösung")] string VideoResolution,
//    [property: JsonProperty("Codec")] string Codec,
//    [property: JsonProperty("Bitrate")] string? Bitrate = null,
//    [property: JsonProperty("Bits_pro_Sample")] string? BitsPerSample = null,
//    [property: JsonProperty("Abtastrate")] string? SampleRate = null
//);

//public record VlcMeta(
//    [property: JsonProperty("filename")] string Filename,
//    [property: JsonProperty("url")] string Url,
//    [property: JsonProperty("genre")] string Genre,
//    [property: JsonProperty("title")] string Title
//);

//public record VlcVideoEffects(
//    [property: JsonProperty("hue")] int Hue,
//    [property: JsonProperty("saturation")] double Saturation,
//    [property: JsonProperty("contrast")] double Contrast,
//    [property: JsonProperty("brightness")] double Brightness,
//    [property: JsonProperty("gamma")] double Gamma
//);

// ReSharper restore StringLiteralTypo
// ReSharper restore CommentTypo
