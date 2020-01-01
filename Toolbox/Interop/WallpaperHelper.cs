using Extensions.netExtensions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace Toolbox.Interop
{
    public static class WallpaperHelper
    {

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);


        const int SETWALLPAPER = 20; //0x0014
        const int UPDATEINIFILE = 0x01;
        const int SENDWININICHANGE = 0x02;

        public static void SetWallpaper(string filePath, WindowsWallpaperStyle style)
        {
            var attr = style.GetAttribute<WindowsWallpaperStyleValuesAttribute>();
            if (attr != null)
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    key.SetValue("WallpaperStyle", attr.WallpaperStyle);
                    key.SetValue("TileWallpaper", attr.TileWallpaper);
                }
            }
            var success = SystemParametersInfo(SETWALLPAPER, 0, filePath, UPDATEINIFILE | SENDWININICHANGE);
            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }
    }
    public enum WindowsWallpaperStyle
    {
        [WindowsWallpaperStyleValues(10, 0)]
        Fill, //resizes the image to fill both height and width --> keeps aspect ratio, cuts parts away
        [WindowsWallpaperStyleValues(6, 0)]
        Fit, //resizes the imgage to fit either height or width --> keeps aspect ratio, but black borders
        [WindowsWallpaperStyleValues(2, 0)]
        Stretch, //stretches the image to fill height and width --> aspect ratio over board
        [WindowsWallpaperStyleValues(0, 1)]
        Tile, // will tile the imgage without changing size --> picture shows multiple times
        [WindowsWallpaperStyleValues(0, 0)]
        Center, //will center the image without changing size --> borders or overlow
        [WindowsWallpaperStyleValues(22, 0)]
        Span
    }


    internal sealed class WindowsWallpaperStyleValuesAttribute : Attribute
    {
        public WindowsWallpaperStyleValuesAttribute(int wallpaperStyle, int tileWallpaper)
        {
            WallpaperStyle = wallpaperStyle.ToString();
            TileWallpaper = tileWallpaper.ToString();
        }

        public string WallpaperStyle { get; }
        public string TileWallpaper { get; }
    }

}
