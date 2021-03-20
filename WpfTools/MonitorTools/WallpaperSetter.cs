using aemarcoCommons.Extensions.AttributeExtensions;
using aemarcoCommons.Toolbox.Interop;
using aemarcoCommons.Toolbox.MonitorTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aemarcoCommons.WpfTools.MonitorTools
{
    public class WallpaperSetter : ISingleton
    {
        #region ctor

        public const string VirtualScreenName = "Virtual";
        public const string LockScreenName = nameof(LockScreen);
        private readonly IWallpaperSetterSettings _wallpaperSetterSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly List<IWallpaperRealEstate> _monitors;


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
            _monitors = GetMonitors().ToList();
        }

        private IEnumerable<IWallpaperRealEstate> GetMonitors()
        {
            foreach (var scr in Screen.AllScreens)
            {
                yield return CreateMonitor(scr);
            }

            yield return CreateVirtualMonitor();
            yield return CreateLockScreen();
        }
        private IWallpaperRealEstate CreateMonitor(Screen screen)
        {
            var targetRectangle = new Rectangle(
                -Screen.AllScreens.Min(x => x.Bounds.X) + screen.Bounds.Left,
                -Screen.AllScreens.Min(x => x.Bounds.Y) + screen.Bounds.Top,
                screen.Bounds.Width,
                screen.Bounds.Height);

            return new Monitor(targetRectangle, screen.DeviceName, _wallpaperSetterSettings.CombinedWallpaperFilePath, _wallpaperSetterSettings);
        }
        private IWallpaperRealEstate CreateVirtualMonitor()
        {
            var allScreenRect = new Rectangle(
                0,
                0,
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            var virtualMon = new Monitor(allScreenRect, VirtualScreenName, _wallpaperSetterSettings.VirtualWallpaperFilePath, _wallpaperSetterSettings);
            return virtualMon;
        }
        private IWallpaperRealEstate CreateLockScreen()
        {
            var targetRectangle = new Rectangle(
                0,
                0,
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);

            return new LockScreen(targetRectangle, LockScreenName, _wallpaperSetterSettings);
        }



        public IEnumerable<IWallpaperRealEstate> WallpaperTargets => _monitors.ToList();
        public IWallpaperSetterSettings Settings => _wallpaperSetterSettings;


        #endregion

        #region private

        private void SetCombinedBackgroundImage()
        {

            //get the right true screen objects, and draw a image for it
            var mons = _monitors.Where(x =>
                x.DeviceName != VirtualScreenName &&
                x.DeviceName != LockScreenName);
            using Image virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            using var virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap);
            foreach (var mon in mons)
            {
                mon.DrawToGraphics(virtualScreenGraphic);
            }


            //save the image to hd and set it
            virtualScreenBitmap.Save(_wallpaperSetterSettings.CombinedWallpaperFilePath, ImageFormat.Jpeg);
            WallpaperHelper.SetWallpaper(_wallpaperSetterSettings.CombinedWallpaperFilePath, WindowsWallpaperStyle.Tile);
        }
        private void SetVirtualBackgroundImage()
        {
            //get the right virtual screen object, and draw a image for it
            var mon = _monitors.First(x => x.DeviceName == VirtualScreenName);
            using Image virtualScreenBitmap = new Bitmap(mon.Width, mon.Height);
            using var virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap);
            mon.DrawToGraphics(virtualScreenGraphic);

            //save the image to hd and set it
            virtualScreenBitmap.Save(_wallpaperSetterSettings.VirtualWallpaperFilePath, ImageFormat.Jpeg);
            WallpaperHelper.SetWallpaper(_wallpaperSetterSettings.VirtualWallpaperFilePath, WindowsWallpaperStyle.Tile);
        }

#if NET5_0
        [SupportedOSPlatform("windows10.0.10240")]
#endif
        private async Task SetLockScreenBackgroundImage()
        {
            //get the right lock screen object, and draw a image for it
            var mon = _monitors.First(x => x.DeviceName == LockScreenName);
            using Image virtualScreenBitmap = new Bitmap(mon.Width, mon.Height);
            using var virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap);
            mon.DrawToGraphics(virtualScreenGraphic);

            //save the image to hd
            virtualScreenBitmap.Save(_wallpaperSetterSettings.LockScreenFilePath, ImageFormat.Jpeg);

            //use win api to set the lock screen
            await using var stream = File.OpenRead(_wallpaperSetterSettings.LockScreenFilePath);
            await Windows.System.UserProfile.LockScreen.SetImageStreamAsync(stream.AsRandomAccessStream());
        }

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
            if (screens.Contains(null)) throw new ArgumentNullException($"inside {nameof(screens)}");
            if (files.Contains(null)) throw new ArgumentNullException($"inside {nameof(files)}");
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

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once MemberCanBeProtected.Global
        public async Task SetWallsForScreens(List<string> screens, List<Image> images)
        {
            if (screens == null) throw new ArgumentNullException(nameof(screens));
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (screens.Contains(null)) throw new ArgumentNullException($"inside {nameof(screens)}");
            if (images.Contains(null)) throw new ArgumentNullException($"inside {nameof(images)}");
            if (screens.Count == 0) throw new ArgumentException("no screens to set", nameof(screens));
            if (screens.Count != images.Count) throw new ArgumentException("screen count does not match images count", nameof(images));

            for (var i = 0; i < screens.Count; i++)
            {
                var mon = _monitors.First(x => x.DeviceName == screens[i]);
                mon.SetWallpaper(images[i]);
            }

            if (screens.Any(x => x == VirtualScreenName))
                SetVirtualBackgroundImage();
            else if (Screen.AllScreens.Any(screen => screens.Contains(screen.DeviceName)))
                SetCombinedBackgroundImage();
            if (screens.Any(x => x == LockScreenName)
#if NET5_0

                && OperatingSystem.IsWindowsVersionAtLeast(10, 0, 10240)
#endif
            )

                await SetLockScreenBackgroundImage()
                        .ConfigureAwait(false);
        }

        #endregion

    }
}
