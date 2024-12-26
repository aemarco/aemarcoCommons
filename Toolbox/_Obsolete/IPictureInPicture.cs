using System;
using System.Drawing;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.PictureTools
{

    [Obsolete("Use IImageInImage instead.")]
    public interface IPictureInPicture
    {
        Rectangle TargetArea { get; }

        Image Current { get; }
        DateTimeOffset Timestamp { get; }
        bool ChangedSinceDrawn { get; }

        void DrawToImage(Image image);
    }
}