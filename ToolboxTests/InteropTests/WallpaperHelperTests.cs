using NUnit.Framework;
using System;
using System.Diagnostics;
using aemarcoCommons.Toolbox.Interop;

namespace ToolboxTests.InteropTests
{
    public class WallpaperHelperTests
    {
        static string WideImage => $"{AppDomain.CurrentDomain.BaseDirectory}Resources\\1680_525.jpg";
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once ArrangeTypeMemberModifiers
        static string HighImage => $"{AppDomain.CurrentDomain.BaseDirectory}Resources\\840_1050.jpg";

        [Test]
        public void SetWallpaper()
        {
            if (Debugger.IsAttached)
            {
                //WallpaperHelper.SetWallpaper(WideImage, WindowsWallpaperStyle.Fit);
                //WallpaperHelper.SetWallpaper(WideImage, WindowsWallpaperStyle.Fill);
                //WallpaperHelper.SetWallpaper(WideImage, WindowsWallpaperStyle.Center);
                //WallpaperHelper.SetWallpaper(WideImage, WindowsWallpaperStyle.Span);
                //WallpaperHelper.SetWallpaper(WideImage, WindowsWallpaperStyle.Stretch);
                WallpaperHelper.SetWallpaper(WideImage, WindowsWallpaperStyle.Tile);

                //WallpaperHelper.SetWallpaper(HighImage, WindowsWallpaperStyle.Fit);
                //WallpaperHelper.SetWallpaper(HighImage, WindowsWallpaperStyle.Fill);
                //WallpaperHelper.SetWallpaper(HighImage, WindowsWallpaperStyle.Center);
                //WallpaperHelper.SetWallpaper(HighImage, WindowsWallpaperStyle.Span);
                //WallpaperHelper.SetWallpaper(HighImage, WindowsWallpaperStyle.Stretch);
                //WallpaperHelper.SetWallpaper(HighImage, WindowsWallpaperStyle.Tile);
            }
            else
            {
                Assert.Pass();
            }
        }
    }
}
