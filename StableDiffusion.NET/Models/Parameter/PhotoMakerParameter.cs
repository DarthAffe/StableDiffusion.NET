using HPPH;

namespace StableDiffusion.NET;

public sealed class PhotoMakerParameter
{
    public IImage[]? IdImages { get; set; }

    public string IdEmbedPath { get; set; } = string.Empty;

    /// <summary>
    /// strength for keeping input identity (default: 20)
    /// </summary>
    public float StyleStrength { get; set; } = 20f;

    internal PhotoMakerParameter() { }
}