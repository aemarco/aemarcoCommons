using System;
using System.Drawing;

namespace aemarcoCommons.Toolbox.PictureTools
{
    public interface IPictureInPicture
    {
        Rectangle TargetArea { get; }

        Image Current { get; }
        DateTimeOffset Timestamp { get; }
        bool ChangedSinceDrawn { get; }

        void DrawToImage(Image image);
    }
}