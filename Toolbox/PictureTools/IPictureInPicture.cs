using System;
using System.Drawing;

namespace aemarcoCommons.Toolbox.PictureTools
{
    public interface IPictureInPicture
    {
        Rectangle TargetArea { get; }
        DateTimeOffset Timestamp { get; }
        Image CurrentOriginal { get; }
        void DrawToImage(Image image);
    }
}