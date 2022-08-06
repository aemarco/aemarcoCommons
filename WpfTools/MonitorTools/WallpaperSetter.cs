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

namespace aemarcoCommons.WpfTools.MonitorTools
{
    public class WallpaperSetter : ISingleton, IDisposable
    {
        #region ctor

        private readonly IWallpaperSetterSettings _wallpaperSetterSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private List<IWallpaperRealEstate> _wallpaperTargets;


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
            _wallpaperTargets = GetRealEstates()
                .ToList();

            foreach (var re in _wallpaperTargets
                .Where(x =>
                    oldMons.Any(om =>
                        om.DeviceName == x.DeviceName &&
                        om.CurrentOriginal is not null)))
            {
                var oldMon = oldMons.First(x => x.DeviceName == re.DeviceName);
                re.SetWallpaper(oldMon.CurrentOriginal);
            }


            switch (_lastMode)
            {
                case RealEstateType.Monitor:
                    SetBackgroundImage(_wallpaperTargets
                        .Where(x => x.Type == RealEstateType.Monitor));
                    break;
                case RealEstateType.Virtual:
                    SetBackgroundImage(_wallpaperTargets
                        .Where(x => x.Type == RealEstateType.Virtual));
                    break;
            }
        }
        private void Settings_SplitSettingsChanged(object sender, EventArgs e)
        {
            _wallpaperTargets = GetRealEstates().ToList();
        }


        private IEnumerable<IWallpaperRealEstate> GetRealEstates()
        {
            foreach (var scr in Screen.AllScreens)
            {
                foreach (var split in CreateMonitor(scr))
                {
                    yield return split;
                }
            }

            yield return CreateVirtualMonitor();
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240))
                yield return CreateLockScreen();
        }
        private IEnumerable<IWallpaperRealEstate> CreateMonitor(Screen screen)
        {
            var horDiv = _wallpaperSetterSettings.HorizontalSplit;
            var verDiv = _wallpaperSetterSettings.VerticalSplit;

            var count = 0;
            var bounds = _wallpaperSetterSettings.ScreenUsage switch
            {
                ScreenUsage.WorkingArea => screen.WorkingArea,
                _ => screen.Bounds
            };

            var minX = Screen.AllScreens.Min(x => x.Bounds.X);
            var minY = Screen.AllScreens.Min(x => x.Bounds.Y);


            var width = bounds.Width / horDiv;
            var height = bounds.Height / verDiv;
            for (var j = 0; j < verDiv; j++)
            {
                for (var i = 0; i < horDiv; i++)
                {

                    var x = _wallpaperSetterSettings.ScreenUsage switch
                    {
                        ScreenUsage.WorkingArea => -minX + screen.WorkingArea.Left + (i * width),
                        _ => -minX + screen.Bounds.Left + (i * width)

                    };
                    var y = _wallpaperSetterSettings.ScreenUsage switch
                    {
                        ScreenUsage.WorkingArea => -minY + screen.WorkingArea.Top + (j * height),
                        _ => -minY + screen.Bounds.Top + (j * height)

                    };

                    var name = screen.DeviceName;
                    if (horDiv > 1 || verDiv > 1)
                    {
                        name += $"_{++count}";
                    }
                    yield return new Monitor(
                        new Rectangle(x, y, width, height),
                        name,
                        _wallpaperSetterSettings.CombinedWallpaperFilePath,
                        _wallpaperSetterSettings,
                        RealEstateType.Monitor);
                }
            }

            //var targetRectangle = new Rectangle(
            //    -Screen.AllScreens.Min(x => x.Bounds.X) + screen.Bounds.Left,
            //    -Screen.AllScreens.Min(x => x.Bounds.Y) + screen.Bounds.Top,
            //    screen.Bounds.Width,
            //    screen.Bounds.Height);
            //yield return new Monitor(targetRectangle, screen.DeviceName, _wallpaperSetterSettings.CombinedWallpaperFilePath, _wallpaperSetterSettings);
        }
        private IWallpaperRealEstate CreateVirtualMonitor()
        {
            var allScreenRect = new Rectangle(
                0,
                0,
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            var virtualMon = new Monitor(
                allScreenRect,
                "Virtual",
                _wallpaperSetterSettings.VirtualWallpaperFilePath,
                _wallpaperSetterSettings,
                RealEstateType.Virtual);
            return virtualMon;
        }
        private IWallpaperRealEstate CreateLockScreen()
        {
            var targetRectangle = new Rectangle(
                0,
                0,
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);

            return new LockScreen(targetRectangle, _wallpaperSetterSettings);
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



            for (var i = 0; i < screens.Count; i++)
            {
                _wallpaperTargets
                    .First(x => x.DeviceName == screens[i])
                    .SetWallpaper(images[i]);
            }

            SetBackgroundImage(
                screens.Any(x => _wallpaperTargets.First(wt => wt.Type == RealEstateType.Virtual).DeviceName == x)
                    ? WallpaperTargets.Where(x => x.Type == RealEstateType.Virtual)
                    : WallpaperTargets.Where(x => x.Type == RealEstateType.Monitor));

            if (screens.Any(x => _wallpaperTargets.First(wt => wt.Type == RealEstateType.LockScreen).DeviceName == x)
                && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240))
            {
                await SetLockScreenImage()
                    .ConfigureAwait(false);
            }
        }

        #endregion

        #region private

        protected async Task<Image> GetImage(string fileOrUrl)
        {
            if (fileOrUrl.StartsWith("http"))
            {
                var resp = await _httpClientFactory.CreateClient(nameof(WallpaperSetter))
                    .GetAsync(fileOrUrl, HttpCompletionOption.ResponseHeadersRead);
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


        private RealEstateType _lastMode = RealEstateType.Monitor;
        private void SetBackgroundImage(IEnumerable<IWallpaperRealEstate> targets)
        {
            IWallpaperRealEstate[] sources = targets.ToArray();
            _lastMode = sources.Any(x => x.Type == RealEstateType.Virtual)
                ? RealEstateType.Virtual
                : RealEstateType.Monitor;

            var filePath = _lastMode switch
            {
                RealEstateType.Monitor => _wallpaperSetterSettings.CombinedWallpaperFilePath,
                RealEstateType.Virtual => _wallpaperSetterSettings.VirtualWallpaperFilePath,
                _ => throw new InvalidOperationException($"Mode {_lastMode} not supported.")
            };

            sources.CreateImageFile(filePath);
            WallpaperHelper.SetWallpaper(filePath, WindowsWallpaperStyle.Tile);
        }


        [SupportedOSPlatform("windows10.0.10240")]
        private async Task SetLockScreenImage()
        {
            //get the right lock screen object, and draw a image for it
            _wallpaperTargets
                 .First(x => x.Type == RealEstateType.LockScreen)
                 .CreateImageFile(_wallpaperSetterSettings.LockScreenFilePath);

            //use win api to set the lock screen
            await using FileStream stream = File.OpenRead(_wallpaperSetterSettings.LockScreenFilePath);
            await Windows.System.UserProfile.LockScreen.SetImageStreamAsync(stream.AsRandomAccessStream());
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
}
