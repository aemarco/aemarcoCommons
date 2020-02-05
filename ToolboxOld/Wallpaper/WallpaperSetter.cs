using Extensions.netExtensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Interop;
using Toolbox.MonitorTools;

namespace ToolboxOld.Wallpaper
{
    public class WallpaperSetter
    {
        #region ctor

        public const string VIRTUALSCREEN_NAME = "Virtual";
        private readonly string _defaultBackgroundFile;
        private WallpaperMode _wallMode;
        private readonly List<Monitor> _monitors;
        protected readonly HttpClient _client;

        /// <summary>
        /// Use this Instance to handle setting Wallpapers
        /// </summary>
        /// <param name="mode">
        ///  Fit: Places the Wallpaper as big as possible without cutting (black bars)
        ///  Fill: Cuts as much needed to fill the screen
        ///  AllowFill: Decides automatically between Fill and Fit based on allowed cutting
        ///  AllowFillForceCut (default): Like AllowFill, otherwise Fit with allowed cutting 
        /// </param>
        public WallpaperSetter(WallpaperMode wallMode)
        {
            _defaultBackgroundFile = new FileInfo("CurrentWallpaper.jpg").FullName;
            _wallMode = wallMode;

            _monitors = GetMonitors().ToList();
            _client = new HttpClient(new WebRequestHandler());
        }

        private IEnumerable<Monitor> GetMonitors()
        {
            foreach (var scr in Screen.AllScreens)
            {
                yield return CreateMonitor(scr);
            }

            yield return CreateVirtualMonitor();
        }

        private Monitor CreateMonitor(Screen screen)
        {
            var targetRectangle = new Rectangle(
                -Screen.AllScreens.Min(x => x.Bounds.X) + screen.Bounds.Left,
                -Screen.AllScreens.Min(x => x.Bounds.Y) + screen.Bounds.Top,
                screen.Bounds.Width,
                screen.Bounds.Height);

            return new Monitor(targetRectangle, screen.DeviceName, _defaultBackgroundFile, _wallMode);
        }

        private Monitor CreateVirtualMonitor()
        {
            var allScreenRect = new Rectangle(
                Screen.AllScreens.Min(x => x.Bounds.X),
                Screen.AllScreens.Min(x => x.Bounds.Y),
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            var virtualMon = new Monitor(allScreenRect, VIRTUALSCREEN_NAME, _defaultBackgroundFile, _wallMode);
            return virtualMon;
        }




        #endregion

        #region props

        public WallpaperMode WallpaperMode
        {
            get { return _wallMode; }
            set
            {
                if (value != _wallMode)
                {
                    _wallMode = value;
                    foreach (var mon in _monitors)
                        mon.WallpaperMode = value;
                }
            }
        }

        #endregion

        #region private

        private void SetBackgroundImage(bool virtualScreen)
        {
            using (Image virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap))
                {
                    if (virtualScreen)
                    {
                        var mon = _monitors.First(x => x.DeviceName == VIRTUALSCREEN_NAME);
                        mon.DrawToGraphics(virtualScreenGraphic);
                    }
                    else
                    {
                        foreach (var mon in _monitors
                                    .Where(x => x.DeviceName != VIRTUALSCREEN_NAME))
                        {
                            mon.DrawToGraphics(virtualScreenGraphic);
                        }
                    }
                }
                virtualScreenBitmap.Save(_defaultBackgroundFile, ImageFormat.Jpeg);
            }
            WallpaperHelper.SetWallpaper(_defaultBackgroundFile, WindowsWallpaperStyle.Tile);
        }

        protected virtual async Task<Image> GetImage(string file)
        {
            if (file.StartsWith("http"))
            {
                var resp = await _client.GetAsync(file, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();

                using (Stream stream = await resp.Content.ReadAsStreamAsync())
                {
                    Image img = Image.FromStream(stream);
                    return img;
                }
            }
            else
            {
                using (var bmpTemp = new Bitmap(file))
                    return new Bitmap(bmpTemp);
            }
        }

        #endregion

        #region Setting Wall

        //eins auf allen Monitoren --> direct to Windows API
        /// <summary>
        /// Sets given Wallpaper foreach Screen
        /// </summary>
        /// <param name="file">Wallpaper to set</param>
        public void SetSameWallOnEveryScreen(string file)
        {
            var attr = _wallMode.GetAttribute<WallpaperModeWindowsMappingAttribute>();
            WallpaperHelper.SetWallpaper(file, attr.WindowsWallpaperStyle);
        }

        //einzelne
        /// <summary>
        /// Sets given Bitmap on given Screen
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="file">Wallpaper to set</param>
        public async Task SetWallForScreen(string screen, string file)
        {
            if (String.IsNullOrWhiteSpace(screen) || String.IsNullOrWhiteSpace(file))
            {
                throw new ArgumentException("Screen or Wallpaper not provided correctly.");
            }
            Image image = await GetImage(file);
            SetWallForScreen(screen, image);
        }
        /// <summary>
        /// Sets given Bitmap on given Screen
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="image">Image to set</param>
        public void SetWallForScreen(string screen, Image image)
        {
            if (String.IsNullOrWhiteSpace(screen) || image == null)
            {
                throw new ArgumentException("Screen or Wallpaper not provided correctly.");
            }
            SetWallsForScreens(new List<string> { screen }, new List<Image> { image });
        }

        //mehrere
        /// <summary>
        /// Sets given Wallpapers to given Screens
        /// </summary>
        /// <param name="screens">Screen Device names</param>
        /// <param name="bitmaps">Wallpapers to set</param>
        public async Task SetWallsForScreens(List<string> screens, List<string> files)
        {
            if (screens == null) throw new ArgumentNullException(nameof(screens));
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (screens.Contains(null)) throw new ArgumentNullException($"inside {nameof(screens)}");
            if (files.Contains(null)) throw new ArgumentNullException($"inside {nameof(files)}");
            if (screens.Count == 0) throw new ArgumentException(nameof(screens));
            if (screens.Count != files.Count) throw new ArgumentException(nameof(files));


            List<Image> images = new List<Image>();
            foreach (var file in files)
            {
                images.Add(await GetImage(file));
            }
            SetWallsForScreens(screens, images);
        }

        /// <summary>
        /// Sets given Wallpapers to given Screens
        /// </summary>
        /// <param name="screens">Screen Device names</param>
        /// <param name="images">Image to set on those screens</param>
        public void SetWallsForScreens(List<string> screens, List<Image> images)
        {
            if (screens == null) throw new ArgumentNullException(nameof(screens));
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (screens.Contains(null)) throw new ArgumentNullException($"inside {nameof(screens)}");
            if (images.Contains(null)) throw new ArgumentNullException($"inside {nameof(images)}");
            if (screens.Count == 0) throw new ArgumentException(nameof(screens));
            if (screens.Count != images.Count) throw new ArgumentException(nameof(images));


            for (int i = 0; i < screens.Count; i++)
            {
                Monitor mon = _monitors.FirstOrDefault(x => x.DeviceName == screens[i]);
                if (mon == null)
                {
                    Screen scr = Screen.AllScreens.FirstOrDefault(x => x.DeviceName == screens[i]);
                    if (scr == null) continue;
                    mon = new Monitor(scr.Bounds, scr.DeviceName, _defaultBackgroundFile, _wallMode);
                    _monitors.Add(mon);
                }
                mon.SetWallpaper(images[i]);
            }
            SetBackgroundImage(screens.Contains(VIRTUALSCREEN_NAME));
        }

        #endregion

    }
}
