using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;


namespace WinTools.WindTools
{
    public class WallpaperSetter
    {
        #region fields

        public const string VIRTUALSCREEN_NAME = "Virtual";
        private readonly string _defaultBackgroundFile;
        private WallpaperMode _wallMode;
        
        private readonly Dictionary<Monitor, List<string>> _monitorDictionary;
        protected readonly HttpClient _client;

        #endregion

        #region ctor

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

            _monitorDictionary = new Dictionary<Monitor, List<string>>();
            foreach (Screen scr in Screen.AllScreens)
            {
                var mon = new Monitor(scr.Bounds, scr.DeviceName, _defaultBackgroundFile, _wallMode);
                _monitorDictionary.Add(mon, null);
            }
            //adding Virtual Screen as well.
            var allScreenRect = new Rectangle(
                Screen.AllScreens.Min(x => x.Bounds.X),
                Screen.AllScreens.Min(x => x.Bounds.Y),
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height);
            var virtualMon = new Monitor(allScreenRect, VIRTUALSCREEN_NAME, _defaultBackgroundFile, _wallMode);
            _monitorDictionary.Add(virtualMon, null);

            _client = new HttpClient(new WebRequestHandler());
        }

        #endregion

        #region props

        public WallpaperMode Wallpapermode
        {
            get { return _wallMode; }
            set
            {
                if (value != _wallMode)
                {
                    _wallMode = value;
                    foreach (var key in _monitorDictionary.Keys)
                        key.WallpaperMode = value;
                }
            }
        }

        public static int PercentLeftRightCutAllowed { get; internal set; }
        public static int PercentTopBottomCutAllowed { get; internal set; }


        #endregion

        #region private

        private void SetBackgroundImage(WallpaperSetMode mode)
        {
            using (Image virtualScreenBitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height))
            {
                using (Graphics virtualScreenGraphic = Graphics.FromImage(virtualScreenBitmap))
                {
                    switch (mode)
                    {
                        case WallpaperSetMode.VirtualScreen:
                            {
                                var mon = _monitorDictionary.Keys.Where(x => x.DeviceName == VIRTUALSCREEN_NAME).First();
                                mon.DrawToGraphics(virtualScreenGraphic);
                                break;
                            }
                        case WallpaperSetMode.IndividualScreens:
                            {
                                foreach (var mon in _monitorDictionary.Keys
                                    .Where(x => x.DeviceName != VIRTUALSCREEN_NAME))
                                {
                                    mon.DrawToGraphics(virtualScreenGraphic);
                                }
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
                virtualScreenBitmap.Save(_defaultBackgroundFile, ImageFormat.Jpeg);
            }
            WinWallpaper.SetWallpaper(_defaultBackgroundFile, WindowsWallpaperStyle.Tile);
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

        //eins auf allen Monitoren
        /// <summary>
        /// Sets given Wallpaper foreach Screen
        /// </summary>
        /// <param name="file">Wallpaper to set</param>
        public async Task SetSameWallOnEveryScreen(string file)
        {

            switch (_wallMode)
            {
                case WallpaperMode.Fit:
                    WinWallpaper.SetWallpaper(file, WindowsWallpaperStyle.Fit);
                    break;
                case WallpaperMode.AllowFill:
                case WallpaperMode.AllowFillForceCut:
                    WinWallpaper.SetWallpaper(file, WindowsWallpaperStyle.Fill);
                    break;
                case WallpaperMode.Fill:
                    WinWallpaper.SetWallpaper(file, WindowsWallpaperStyle.Fill);
                    break;
                default:
                    throw new NotImplementedException();
            }
            await Task.CompletedTask;

            //foreach (Monitor mon in _monitorDictionary.Keys
            //    .Where(x => x.DeviceName != Constants.VIRTUALSCREEN_NAME))
            //{
            //    mon.SetWallpaper(await GetImage(file));
            //}
            //SetBackgroundImage(WallpaperSetMode.IndividualScreens);
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
                Monitor mon = _monitorDictionary.Keys.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                if (mon == null)
                {
                    Screen scr = Screen.AllScreens.Where(x => x.DeviceName == screens[i]).FirstOrDefault();
                    if (scr == null) continue;
                    mon = new Monitor(scr.Bounds, scr.DeviceName, _defaultBackgroundFile, _wallMode);
                    _monitorDictionary.Add(mon, null);
                }
                mon.SetWallpaper(images[i]);
            }


            if (screens.Contains(VIRTUALSCREEN_NAME))
            {
                SetBackgroundImage(WallpaperSetMode.VirtualScreen);
            }
            else
            {
                SetBackgroundImage(WallpaperSetMode.IndividualScreens);
            }
        }


        //random stuff
        /// <summary>
        /// Sets a list for Random Wallpaper Function
        /// </summary>
        /// <param name="screen">Screen Device name</param>
        /// <param name="files">Wallpapers to set</param>
        public void SetWallpaperSourceList(string screen, IEnumerable<string> files)
        {
            if (String.IsNullOrWhiteSpace(screen) || files == null ||
                !files.Any() || files.Contains(null))
            {
                throw new ArgumentException("Screens or Wallpapers not provided correctly.");
            }

            Monitor mon = _monitorDictionary.Keys.Where(x => x.DeviceName == screen).FirstOrDefault();
            if (mon == null)
            {
                throw new ArgumentException($"Monitor {screen} not found.");
            }

            _monitorDictionary[mon] = files.ToList();
        }

        #endregion

        #region statics

        /// <summary>
        /// Returns true if the Image can be snapped to the desired Monitor.
        /// Tolerated cutting amount can be adjusted in the AppSettings
        /// </summary>
        /// <param name="imageWidth">Width of Image</param>
        /// <param name="imageHeight">Height of Image</param>
        /// <param name="monitorWidth">Width of Monitor</param>
        /// <param name="monitorHeight">Height of Monitor</param>
        /// <returns></returns>
        public static bool CanBeSnapped(int imageWidth, int imageHeight, int monitorWidth, int monitorHeight)
        {
            double imageRatio = 1.0 * imageWidth / imageHeight;
            var (minRatio, maxRatio) = GetRatioRange(monitorWidth, monitorHeight);

            return (imageRatio <= maxRatio && imageRatio >= minRatio);
        }

        /// <summary>
        /// returns the min and max Ratio for which Pictures can be Snapped based on allowed % cuts
        /// </summary>
        /// <param name="monitorWidth"></param>
        /// <param name="monitorHeight"></param>
        /// <param name="percentLeftRightCutAllowed"></param>
        /// <param name="percentTopBottomCutAllowed"></param>
        /// <returns>minRatio and maxRatio</returns>
        public static (double minRatio, double maxRatio) GetRatioRange(int monitorWidth, int monitorHeight,
            int percentLeftRightCutAllowed = -1,
            int percentTopBottomCutAllowed = -1)
        {
            if (percentLeftRightCutAllowed == -1) percentLeftRightCutAllowed = PercentLeftRightCutAllowed;
            if (percentTopBottomCutAllowed == -1) percentTopBottomCutAllowed = PercentTopBottomCutAllowed;

            double maxWidth = 100.0 * monitorWidth / (100 - percentLeftRightCutAllowed);
            double maxHeight = 100.0 * monitorHeight / (100 - percentTopBottomCutAllowed);

            var minratio = monitorWidth / maxHeight;
            var maxratio = maxWidth / monitorHeight;
            if (maxratio == double.PositiveInfinity)
                maxratio = double.MaxValue;

            return (Math.Round(minratio, 2), Math.Round(maxratio, 2));
        }


        public static (double minRatio, double maxRatio) GetRatioRange(int monitorWidth, int monitorHeight,
            int cuttingTolerance)
        {
            return GetRatioRange(monitorWidth, monitorHeight, cuttingTolerance, cuttingTolerance / 2);
        }


        #endregion

    }
    public enum WallpaperMode
    {
        Fit,
        AllowFill,
        AllowFillForceCut,
        Fill
    }

    internal enum WindowsWallpaperStyle
    {
        Fit,
        Fill,
        Tile
    }
    

    internal enum WallpaperSetMode
    {
        VirtualScreen,
        IndividualScreens
    }
    
    internal class Monitor
    {

        #region fields

        private Rectangle _rectangle;
        private Image _wallpaper;

        #endregion

        #region ctor

        internal Monitor(Rectangle rect, string name, string backgroundFile, WallpaperMode mode)
        {
            if (String.IsNullOrWhiteSpace(backgroundFile) || String.IsNullOrWhiteSpace(name))
            {
                throw new NullReferenceException("Monitor kann nicht initialisiert werden");
            }


            //set mandatory fields 
            _rectangle = new Rectangle(
                GetRectangleX(rect),
                GetRectangleY(rect),
                rect.Width,
                rect.Height);


            _wallpaper = GetPreviousImage(backgroundFile, _rectangle);
            DeviceName = name;
            WallpaperMode = mode;
        }
        
        
        #endregion

        #region props

        internal string DeviceName { get; }
        public WallpaperMode WallpaperMode { get; internal set; }
        
        #endregion

        #region private

        private int GetRectangleX(Rectangle rect)
        {
            var minX = Screen.AllScreens.Min(x => x.Bounds.X);
            int result = -minX + rect.Left;
            return result;
        }
        private int GetRectangleY(Rectangle rect)
        {
            var minY = Screen.AllScreens.Min(x => x.Bounds.Y);
            int result = -minY + rect.Top;
            return result;
        }
        private Bitmap GetPreviousImage(string backgroundFile, Rectangle rectangle)
        {
            Bitmap result = null;

            if (File.Exists(backgroundFile))
            {
                try
                {
                    using (Bitmap old = new Bitmap(backgroundFile))
                    {
                        if (old.Width >= (rectangle.X + rectangle.Width) &&
                            old.Height >= (rectangle.Y + rectangle.Height))
                        {
                            result = new Bitmap(old.Clone(rectangle, old.PixelFormat));
                        }
                        else
                        {
                            throw new FileLoadException("Image size not compatible.");
                        }
                    }
                }
                catch
                {
                    File.Delete(backgroundFile);
                    result = new Bitmap(_rectangle.Width, _rectangle.Height);
                }
            }
            return result;
        }


        /// <summary>
        /// Sets the Picture as big as possible with Black bars if needed
        /// </summary>
        /// <param name="readyToUsePicture"></param>
        private void SetDirectWallpaper(Image readyToUsePicture)
        {
            float heightRatio = (float)_rectangle.Height / (float)readyToUsePicture.Height;
            float widthRatio = (float)_rectangle.Width / (float)readyToUsePicture.Width;

            int height, width;
            int x = 0;
            int y = 0;

            if (heightRatio < widthRatio) //Bild schmaler als Monitor
            {
                width = (int)((float)readyToUsePicture.Width * heightRatio);
                height = (int)((float)readyToUsePicture.Height * heightRatio);
                x = (int)((float)(_rectangle.Width - width) / 2f);
            }
            else //Bild breiter als Monitor
            {
                width = (int)((float)readyToUsePicture.Width * widthRatio);
                height = (int)((float)readyToUsePicture.Height * widthRatio);
                y = (int)((float)(_rectangle.Height - height) / 2f);
            }

            Rectangle drawTo = new Rectangle(x, y, width, height);

            Bitmap targetImg = new Bitmap(_rectangle.Width, _rectangle.Height);
            Graphics g = Graphics.FromImage(targetImg);
            g.DrawImage(readyToUsePicture, drawTo);
            _wallpaper = targetImg;
        }
        /// <summary>
        /// Sets the Picture and fills the screen by cutting the Picture
        /// </summary>
        /// <param name="pictureToBeCutted"></param>
        private void SetSnappedWallpaper(Image pictureToBeCutted)
        {
            Rectangle rect;
            double targetRatio = 1.0 * _rectangle.Width / _rectangle.Height;

            if (targetRatio < (1.0 * pictureToBeCutted.Width / pictureToBeCutted.Height))
            {   // ratio zu groß
                int targetWidth = (int)(targetRatio * pictureToBeCutted.Height);
                rect = new Rectangle(0, 0, targetWidth, pictureToBeCutted.Height);
                rect.X = (pictureToBeCutted.Width - rect.Width) / 2;
            }
            else
            {
                // ratio zu klein
                int targetHeight = (int)(pictureToBeCutted.Width / targetRatio);
                rect = new Rectangle(0, 0, pictureToBeCutted.Width, targetHeight);
                rect.Y = (pictureToBeCutted.Height - rect.Height) / 2;
            }

            if (rect.X == 0 && rect.Y == 0)
            {   // ratio stimmt überein
                SetDirectWallpaper(pictureToBeCutted);
            }
            else
            {
                SetDirectWallpaper(((Bitmap)pictureToBeCutted).Clone(rect, pictureToBeCutted.PixelFormat));
            }
        }
        /// <summary>
        /// Cuts the Picture by the allowed amount and sets it as big as possible with black bars.
        /// Should be called only if it can´t be "Snapped"
        /// </summary>
        /// <param name="pictureToBeCutted"></param>
        private void SetCuttedWallpaper(Image pictureToBeCutted)
        {
            Rectangle rect;
            double targetRatio = 1.0 * _rectangle.Width / _rectangle.Height;

            if (targetRatio < 1.0 * pictureToBeCutted.Width / pictureToBeCutted.Height)
            {   // ratio zu groß
                double pixelsToCut = 1.0 * pictureToBeCutted.Width / 100 * WallpaperSetter.PercentLeftRightCutAllowed;
                rect = new Rectangle(0, 0, pictureToBeCutted.Width - (int)pixelsToCut, pictureToBeCutted.Height);
                rect.X = (pictureToBeCutted.Width - rect.Width) / 2;
            }
            else
            {   // ratio zu klein
                double pixelsToCut = 1.0 * pictureToBeCutted.Height / 100 * WallpaperSetter.PercentTopBottomCutAllowed;
                rect = new Rectangle(0, 0, pictureToBeCutted.Width, pictureToBeCutted.Height - (int)pixelsToCut);
                rect.Y = (pictureToBeCutted.Height - rect.Height) / 2;
            }

            SetDirectWallpaper(((Bitmap)pictureToBeCutted).Clone(rect, pictureToBeCutted.PixelFormat));
        }

        #endregion

        internal void SetWallpaper(Image wall)
        {
            if (wall == null)
            {
                throw new NullReferenceException("Wallpaper can´t be null");
            }

            switch (WallpaperMode)
            {
                case WallpaperMode.AllowFill:
                    {
                        if (WallpaperSetter.CanBeSnapped(wall.Width, wall.Height, _rectangle.Width, _rectangle.Height))
                        {
                            SetSnappedWallpaper(wall);
                        }
                        else
                        {
                            SetDirectWallpaper(wall);
                        }
                        break;
                    }
                case WallpaperMode.AllowFillForceCut:
                    {
                        if (WallpaperSetter.CanBeSnapped(wall.Width, wall.Height, _rectangle.Width, _rectangle.Height))
                        {
                            SetSnappedWallpaper(wall);
                        }
                        else
                        {
                            SetCuttedWallpaper(wall);
                        }
                        break;
                    }
                case WallpaperMode.Fit:
                    {
                        SetDirectWallpaper(wall);
                        break;
                    }
                case WallpaperMode.Fill:
                    {
                        SetSnappedWallpaper(wall);
                        break;
                    }
            }
        }

        internal void DrawToGraphics(Graphics g)
        {
            g.DrawImage(_wallpaper, _rectangle);
        }


    }

    internal static class WinWallpaper
    {
        internal static void SetWallpaper(string wallpaper, WindowsWallpaperStyle style)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                switch (style)
                {
                    case WindowsWallpaperStyle.Fit:
                        key.SetValue(@"WallpaperStyle", 6.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case WindowsWallpaperStyle.Fill:
                        key.SetValue(@"WallpaperStyle", 10.ToString());
                        key.SetValue(@"TileWallpaper", 0.ToString());
                        break;
                    case WindowsWallpaperStyle.Tile:
                        key.SetValue(@"WallpaperStyle", 0.ToString());
                        key.SetValue(@"TileWallpaper", 1.ToString());
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            SystemParametersInfo(SETWALLPAPER, 0, wallpaper, UPDATEINIFILE | SENDWININICHANGE);
        }

        #region winApi

        const int SETWALLPAPER = 20; //0x0014
        const int UPDATEINIFILE = 0x01;
        const int SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        #endregion
    }


}
