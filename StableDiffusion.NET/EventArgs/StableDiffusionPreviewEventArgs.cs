using System;
using HPPH;

namespace StableDiffusion.NET;

public sealed class StableDiffusionPreviewEventArgs(int step, bool isNoisy, Image<ColorRGB> image) : EventArgs
{
    #region Properties & Fields

    public int Step { get; } = step;
    public bool IsNoisy { get; } = isNoisy;
    public Image<ColorRGB> Image { get; } = image;

    #endregion
}