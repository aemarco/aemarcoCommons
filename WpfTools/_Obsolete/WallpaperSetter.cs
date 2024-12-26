using aemarcoCommons.Extensions;
using aemarcoCommons.Toolbox.Interop;
using aemarcoCommons.Toolbox.MonitorTools;
using aemarcoCommons.Toolbox.PictureTools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.WpfTools.MonitorTools;

[Obsolete]
public class WallpaperSetter : ISingleton, IDisposable
{
    #region ctor

    private readonly IWallpaperSetterSettings _wallpaperSetterSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private List<IWallpaperRealEstate> _wallpaperTargets;

    public event EventHandler WallpaperTargetsChanged;
    protected virtual void OnWallpaperTargetsChanged()
    {
        WallpaperTargetsChanged?.Invoke(this, EventArgs.Empty);
    }


    /// <summary>
    /// Use this Instance to handle setting Wallpapers
    /// </summary>
    /// <param name="wallpaperSetterSettings"></param>
    /// <param name="httpClientFactory"></param>
    // ReSharper disable once MemberCanBeProtected.Global
    public WallpaperSetter(
        IWallpaperSetterSettings wallpaperSetterSettings,
        IHttpClientFactory httpClientFactory)
    {
        _wallpaperSetterSettings = wallpaperSetterSettings;
        _httpClientFactory = httpClientFactory;
        _wallpaperTargets = GetRealEstates().ToList();

        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        _wallpaperSetterSettings.SplitSettingsChanged += Settings_SplitSettingsChanged;
    }

    private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
    {
        var oldMons = _wallpaperTargets;
        _wallpaperTargets = GetRealEstates().ToList();
        OnWallpaperTargetsChanged();

        foreach (var re in _wallpaperTargets
                     .Where(x =>
                         oldMons.Any(om =>
                             om.DeviceName == x.DeviceName &&
                             om.Current is not null)))
        {
            var oldMon = oldMons.First(x => x.DeviceName == re.DeviceName);
            re.SetWallpaper(oldMon.Current);
        }
        SetBackgroundImages(UpdateFiles());
    }
    private void Settings_SplitSettingsChanged(object sender, EventArgs e)
    {
        _wallpaperTargets = GetRealEstates().ToList();
        OnWallpaperTargetsChanged();
    }


    private IEnumerable<IWallpaperRealEstate> GetRealEstates()
    {
        //virtual
        foreach ((Rectangle rect, var name) in CreateSplits(SystemInformation.VirtualScreen, "Virtual"))
        {

            yield return new Monitor(
                GetAbsoluteBasedFromMonitorBased(rect),
                name,
                _wallpaperSetterSettings.VirtualWallpaperFilePath,
                _wallpaperSetterSettings,
                RealEstateType.Virtual);
        }

        //monitors
        foreach (var scr in Screen.AllScreens)
        {
            var bounds = _wallpaperSetterSettings.ScreenUsage switch
            {
                ScreenUsage.WorkingArea => scr.WorkingArea,
                _ => scr.Bounds
            };
            bounds = GetAbsoluteBasedFromMonitorBased(bounds);
            foreach ((Rectangle rect, var name) in CreateSplits(bounds, scr.DeviceName))
            {
                yield return new Monitor(
                    rect,
                    name,
                    _wallpaperSetterSettings.CombinedWallpaperFilePath,
                    _wallpaperSetterSettings,
                    RealEstateType.Monitor);
            }
        }

        //lock screen
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240))
        {
            //lock screens ignore screen usage
            foreach ((Rectangle rect, var name) in CreateSplits(Screen.PrimaryScreen!.Bounds, nameof(LockScreen)))
            {
                yield return new LockScreen(
                    rect,
                    name,
                    _wallpaperSetterSettings.LockScreenFilePath,
                    _wallpaperSetterSettings);
            }
        }

        //file images
        foreach (var fileImageSettings in _wallpaperSetterSettings.FileImages)
        {
            yield return new FileImage(fileImageSettings);
        }
    }

    private IEnumerable<(Rectangle rect, string name)> CreateSplits(
        Rectangle bounds,
        string baseName)
    {
        var horDiv = _wallpaperSetterSettings.HorizontalSplit;
        var verDiv = _wallpaperSetterSettings.VerticalSplit;

        var count = 0;
        //var minX = Screen.AllScreens.Min(x => x.Bounds.X);
        //var minY = Screen.AllScreens.Min(x => x.Bounds.Y);

        var width = bounds.Width / horDiv;
        var height = bounds.Height / verDiv;
        var name = baseName;
        for (var j = 0; j < verDiv; j++)
        {
            for (var i = 0; i < horDiv; i++)
            {
                var x = bounds.Left + (i * width);
                var y = bounds.Top + (j * height);
                if (horDiv > 1 || verDiv > 1)
                    name = $"{baseName}_{++count}";

                var rect = new Rectangle(x, y, width, height);

                yield return (rect, name);
            }
        }
    }

    private Rectangle GetAbsoluteBasedFromMonitorBased(Rectangle rect)
    {
        var minX = Screen.AllScreens.Min(x => x.Bounds.X);
        var minY = Screen.AllScreens.Min(x => x.Bounds.Y);
        return rect with
        {
            X = -minX + rect.X,
            Y = -minY + rect.Y
        };
    }



    public IEnumerable<IWallpaperRealEstate> WallpaperTargets => _wallpaperTargets;

    #endregion

    #region Setting Wall

    //one for each screen --> direct to Windows API
    /// <summary>
    /// Sets given Wallpaper foreach Screen
    /// </summary>
    /// <param name="file">Wallpaper to set</param>
    public void SetSameWallOnEveryScreen(string file)
    {
        var attr = _wallpaperSetterSettings.WallpaperMode.GetAttribute<WallpaperModeWindowsMappingAttribute>();
        WallpaperHelper.SetWallpaper(file, attr.WindowsWallpaperStyle);
    }
    //single
    /// <summary>
    /// Sets given Bitmap on given Screen
    /// </summary>
    /// <param name="screen">Screen Device name</param>
    /// <param name="file">Wallpaper to set</param>
    public async Task SetWallForScreen(string screen, string file)
    {
        if (string.IsNullOrWhiteSpace(screen) || string.IsNullOrWhiteSpace(file))
        {
            throw new ArgumentException("Screen or Wallpaper not provided correctly.");
        }
        var image = await GetImage(file)
            .ConfigureAwait(false);
        await SetWallForScreen(screen, image)
            .ConfigureAwait(false);
    }
    /// <summary>
    /// Sets given Bitmap on given Screen
    /// </summary>
    /// <param name="screen">Screen Device name</param>
    /// <param name="image">Image to set</param>
    // ReSharper disable once MemberCanBePrivate.Global
    public Task SetWallForScreen(string screen, Image image)
    {
        if (string.IsNullOrWhiteSpace(screen) || image == null)
        {
            throw new ArgumentException("Screen or Wallpaper not provided correctly.");
        }
        return SetWallsForScreens(new List<string> { screen }, new List<Image> { image });
    }

    //multiple
    /// <summary>
    /// Sets given Wallpapers to given Screens
    /// </summary>
    /// <param name="screens">Screen Device names</param>
    /// <param name="files">Wallpapers to set</param>
    public async Task SetWallsForScreens(List<string> screens, List<string> files)
    {
        if (screens == null) throw new ArgumentNullException(nameof(screens));
        if (files == null) throw new ArgumentNullException(nameof(files));
        if (screens.Contains(null)) throw new ArgumentNullException(nameof(screens));
        if (files.Contains(null)) throw new ArgumentNullException(nameof(files));
        if (screens.Count == 0) throw new ArgumentException("no screens to set", nameof(screens));
        if (screens.Count != files.Count) throw new ArgumentException("screen count does not match files count", nameof(files));


        var images = new List<Image>();
        foreach (var file in files)
        {
            images.Add(await GetImage(file)
                .ConfigureAwait(false));
        }
        await SetWallsForScreens(screens, images).ConfigureAwait(false);
    }

    /// <summary>
    /// Sets given Wallpapers to given Screens.
    /// Combination of individual screens and Virtual screen is not possible.
    /// </summary>
    /// <param name="screens">Screen Device names</param>
    /// <param name="images">Image to set on those screens</param>
    public async Task SetWallsForScreens(List<string> screens, List<Image> images)
    {
        if (screens == null) throw new ArgumentNullException(nameof(screens));
        if (images == null) throw new ArgumentNullException(nameof(images));
        if (screens.Contains(null)) throw new ArgumentNullException(nameof(screens));
        if (images.Contains(null)) throw new ArgumentNullException(nameof(images));
        if (screens.Count == 0) throw new ArgumentException("no screens to set", nameof(screens));
        if (screens.Count != images.Count) throw new ArgumentException("screen count does not match images count", nameof(images));


        var targets = _wallpaperTargets
            .Where(x => screens.Contains(x.DeviceName))
            .ToArray();

        var dict = new Dictionary<string, Image>();
        foreach (var target in targets)
        {
            dict.Add(target.DeviceName, images[screens.IndexOf(target.DeviceName)]);
        }
        await HandleUpdates(dict);
    }

    public async Task HandleUpdates(Dictionary<string, Image> updates)
    {
        foreach (var (mon, image) in updates)
        {
            if (_wallpaperTargets.FirstOrDefault(x => x.DeviceName == mon) is { } target)
            {
                target.SetWallpaper(image);
            }
        }
        await HandleUpdates();
    }

    #endregion

    #region private


    private async Task HandleUpdates()
    {
        //update files
        var changes = UpdateFiles();

        //virtual and monitors
        SetBackgroundImages(changes);

        //lock screen
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240))
            await SetLockScreenImages(changes)
                .ConfigureAwait(false);

    }

    protected async Task<Image> GetImage(string fileOrUrl)
    {
        var stream = await GetImageStream(fileOrUrl);
        var result = Image.FromStream(stream);
        return result;
    }
    protected async Task<Stream> GetImageStream(string fileOrUrl)
    {
        Stream result;
        if (fileOrUrl.StartsWith("http"))
        {
            using var client = _httpClientFactory.CreateClient(nameof(WallpaperSetter));
            var resp = await client.GetAsync(fileOrUrl, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();
            result = await resp.Content.ReadAsStreamAsync();
        }
        else
        {
            result = File.OpenRead(fileOrUrl);
        }
        return result;
    }


    private string[] UpdateFiles()

    {
        List<string> result = [];
        var fileImages = _wallpaperTargets
            .GroupBy(x => x.TargetFilePath)
            .ToDictionary(x => x.Key, x => x.ToArray());
        foreach ((var file, IWallpaperRealEstate[] estates) in fileImages)
        {
            if (!estates.Any(x => x.ChangedSinceDrawn))
                continue;

            estates.CreateImageFile(file);
            result.Add(file);
        }
        return [.. result];
    }

    private void SetBackgroundImages(string[] changes)
    {
        var toSet = changes.Contains(_wallpaperSetterSettings.VirtualWallpaperFilePath)
            ? _wallpaperSetterSettings.VirtualWallpaperFilePath
            : changes.Contains(_wallpaperSetterSettings.CombinedWallpaperFilePath)
                ? _wallpaperSetterSettings.CombinedWallpaperFilePath
                : null;
        if (toSet is not null)
            WallpaperHelper.SetWallpaper(toSet, WindowsWallpaperStyle.Tile);
    }

    [SupportedOSPlatform("windows10.0.10240")]
    private async Task SetLockScreenImages(string[] changes)
    {
        if (changes.Any(x => x == _wallpaperSetterSettings.LockScreenFilePath))
        {
            //use win api to set the lock screen
            await using FileStream stream = File.OpenRead(_wallpaperSetterSettings.LockScreenFilePath);
            await Windows.System.UserProfile.LockScreen.SetImageStreamAsync(stream.AsRandomAccessStream());
        }
    }

    #endregion

    #region IDisposable

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // To detect redundant calls
    private bool _disposed;

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            // Dispose managed state (managed objects).
            SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            _disposed = true;
        }
    }

    #endregion

}