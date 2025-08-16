using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class CannyParameter
{
    /// <summary>
    /// the image to process
    /// </summary>
    public IImage? Image { get; set; } = null;

    /// <summary>
    /// 
    /// </summary>
    public float HighThreshold { get; set; } = 0.08f;

    /// <summary>
    /// 
    /// </summary>
    public float LowThreshold { get; set; } = 0.08f;

    /// <summary>
    /// 
    /// </summary>
    public float Weak { get; set; } = 0.8f;

    /// <summary>
    /// 
    /// </summary>
    public float Strong { get; set; } = 1.0f;

    /// <summary>
    /// 
    /// </summary>
    public bool Inverse { get; set; } = false;

    public static CannyParameter Create() => new();
}