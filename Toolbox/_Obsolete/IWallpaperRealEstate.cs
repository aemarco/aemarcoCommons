using aemarcoCommons.Toolbox.PictureTools;
using System;
using System.Drawing;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.MonitorTools
{


    [Obsolete]
    public interface IWallpaperRealEstate : IPictureInPicture
    {
        string DeviceName { get; }
        RealEstateType Type { get; }
        string FriendlyName { get; }
        string TargetFilePath { get; }

        void SetWallpaper(Image image, Color? background = null);
    }


}