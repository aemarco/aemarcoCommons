using System;
using System.Drawing;

namespace aemarcoCommons.Toolbox.MonitorTools
{
    public interface IWallpaperRealEstate
    {
        string DeviceName { get; }
        int Width { get; }
        int Height { get; }
        void SetWallpaper(Image image);
        void DrawToGraphics(Graphics g);

        DateTimeOffset Timestamp { get; }
        Image CurrentOriginal { get; }
    }
}