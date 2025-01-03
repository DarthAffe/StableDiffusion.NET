using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class PhotoMakerParameter
{
    /// <summary>
    /// path to PHOTOMAKER input id images dir
    /// </summary>
    public string InputIdImageDirectory { get; set; } = string.Empty;

    /// <summary>
    /// strength for keeping input identity (default: 20)
    /// </summary>
    public float StyleRatio { get; set; } = 20f;

    /// <summary>
    /// normalize PHOTOMAKER input id images
    /// </summary>
    public bool NormalizeInput { get; set; } = false;
}