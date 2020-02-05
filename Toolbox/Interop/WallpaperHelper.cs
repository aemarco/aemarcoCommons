using Extensions.netExtensions;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Toolbox.Interop
{
    public static class WallpaperHelper
    {

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);


        private const int SetWallpaperOperation = 20; //0x0014
        private const int UpdateIniFile = 0x01;
        private const int SendWinIniChange = 0x02;

        public static void SetWallpaper(string filePath, WindowsWallpaperStyle style)
        {
            var attr = style.GetAttribute<WindowsWallpaperStyleValuesAttribute>();
            if (attr != null)
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
                {
                    key?.SetValue("WallpaperStyle", attr.WallpaperStyle);
                    key?.SetValue("TileWallpaper", attr.TileWallpaper);
                }
            }
            var success = SystemParametersInfo(SetWallpaperOperation, 0, filePath, UpdateIniFile | SendWinIniChange);
            if (success) return;
            
            
            var error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error);
        }
    }
    public enum WindowsWallpaperStyle
    {
        [WindowsWallpaperStyleValues(10, 0)]
        Fill, //resizes the image to fill both height and width --> keeps aspect ratio, cuts parts away
        [WindowsWallpaperStyleValues(6, 0)]
        Fit, //resizes the image to fit either height or width --> keeps aspect ratio, but black borders
        [WindowsWallpaperStyleValues(2, 0)]
        Stretch, //stretches the image to fill height and width --> aspect ratio over board
        [WindowsWallpaperStyleValues(0, 1)]
        Tile, // will tile the image without changing size --> picture shows multiple times
        [WindowsWallpaperStyleValues(0, 0)]
        Center, //will center the image without changing size --> borders or overflow
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
