using aemarcoCommons.Extensions.AttributeExtensions;
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

namespace aemarcoCommons.WpfTools.MonitorTools;

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
        SetBackgroundImages(_currentMode);
    }
    private void Settings_SplitSettingsChanged(object sender, EventArgs e)
    {
        _wallpaperTargets = GetRealEstates().ToList();
        OnWallpaperTargetsChanged();
    }


    private IEnumerable<IWallpaperRealEstate> GetRealEstates()
    {
        //virtual
        foreach (var split in CreateSplits(SystemInformation.VirtualScreen, "Virtual"))
        {
            yield return new Monitor(
                GetAbsoluteBasedFromMonitorBased(split.rect),
                split.name,
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
            foreach (var split in CreateSplits(bounds, scr.DeviceName))
            {
                yield return new Monitor(
                    split.rect,
                    split.name,
                    _wallpaperSetterSettings.CombinedWallpaperFilePath,
                    _wallpaperSetterSettings,
                    RealEstateType.Monitor);
            }
        }

        //lock screen
        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240))
        {
            //lock screens ignore screen usage
            foreach (var split in CreateSplits(Screen.PrimaryScreen!.Bounds, nameof(LockScreen)))
            {
                yield return new LockScreen(
                    split.rect,
                    split.name,
                    _wallpaperSetterSettings.LockScreenFilePath,
                    _wallpaperSetterSettings);
            }
        }

        //var testBounds = new Rectangle(2400, 270, 960, 540);
        //foreach (var split in CreateSplits(testBounds, "Test"))
        //{
        //    yield return new Monitor(
        //        split.rect,
        //        split.name,
        //        _wallpaperSetterSettings.CombinedWallpaperFilePath,
        //        _wallpaperSetterSettings,
        //        RealEstateType.Monitor);
        //}


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
        foreach (var target in targets)
        {
            target.SetWallpaper(images[screens.IndexOf(target.DeviceName)]);
        }

        var mode = targets.Any(x => x.Type == RealEstateType.Virtual)
            ? RealEstateType.Virtual
            : RealEstateType.Monitor;
        SetBackgroundImages(mode);

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240))
        {
            await SetLockScreenImages()
                .ConfigureAwait(false);
        }
    }

    #endregion

    #region private

    protected async Task<Image> GetImage(string fileOrUrl)
    {
        if (fileOrUrl.StartsWith("http"))
        {

            using var client = _httpClientFactory.CreateClient(nameof(WallpaperSetter));
            var resp = await client.GetAsync(fileOrUrl, HttpCompletionOption.ResponseHeadersRead);
            resp.EnsureSuccessStatusCode();

            await using var stream = await resp.Content.ReadAsStreamAsync();
            return Image.FromStream(stream);

        }
        else
        {
            using var bmpTemp = new Bitmap(fileOrUrl);
            return new Bitmap(bmpTemp);
        }
    }


    private RealEstateType _currentMode = RealEstateType.Monitor;
    private void SetBackgroundImages(RealEstateType mode)
    {
        _currentMode = mode;

        var targets = _wallpaperTargets
            .Where(x => x.Type == _currentMode)
            .ToArray();
        if (!targets.Any(x => x.ChangedSinceDrawn))
            return;


        var filePath = _currentMode switch
        {
            RealEstateType.Virtual => _wallpaperSetterSettings.VirtualWallpaperFilePath,
            RealEstateType.Monitor => _wallpaperSetterSettings.CombinedWallpaperFilePath,
            _ => throw new NotImplementedException($"Mode {_currentMode} not supported")
        };

        targets.CreateImageFile(filePath);
        WallpaperHelper.SetWallpaper(filePath, WindowsWallpaperStyle.Tile);
    }

    [SupportedOSPlatform("windows10.0.10240")]
    private async Task SetLockScreenImages()
    {
        var lockTargets = _wallpaperTargets
            .Where(x => x.Type == RealEstateType.LockScreen)
            .ToArray();
        if (lockTargets.Any(x => x.ChangedSinceDrawn))
        {
            lockTargets.CreateImageFile(_wallpaperSetterSettings.LockScreenFilePath);
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