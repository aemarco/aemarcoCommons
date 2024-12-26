﻿using aemarcoCommons.Toolbox.Interop;
using System;

// ReSharper disable once CheckNamespace
namespace aemarcoCommons.Toolbox.MonitorTools
{
    [Obsolete]
    public enum WallpaperMode
    {
        /// <summary>
        /// as big as possible, with black bars
        /// </summary>
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fit)]
        Fit,
        /// <summary>
        /// if allowed cutting leads to full picture, it will cut and fill
        /// else black bars
        /// </summary>
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fill)]
        AllowFill,
        /// <summary>
        /// if allowed cutting leads to full picture, it will cut and fill
        /// else it will cut as much as allowed, and black bar as the rest
        /// </summary>
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fill)]
        AllowFillForceCut,
        /// <summary>
        /// cut as much necessary, to fill the area completely
        /// </summary>
        [WallpaperModeWindowsMapping(WindowsWallpaperStyle.Fill)]
        Fill
    }

    [Obsolete]
    public enum RealEstateType
    {
        Monitor,
        Virtual,
        LockScreen,
        FileImage
    }

    [Obsolete]
    public enum ScreenUsage
    {
        All,
        WorkingArea
    }

}