using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class ControlNetParameter
{
    public bool IsEnabled => Image != null;

    /// <summary>
    /// image condition, control net
    /// </summary>
    public IImage? Image { get; set; } = null;

    /// <summary>
    /// strength to apply Control Net (default: 0.9)
    /// 1.0 corresponds to full destruction of information in init image
    /// </summary>
    public float Strength { get; set; } = 0.9f;

    /// <summary>
    /// apply canny preprocessor (edge detection)
    /// </summary>
    public bool CannyPreprocess { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public float CannyHighThreshold { get; set; } = 0.08f;

    /// <summary>
    /// 
    /// </summary>
    public float CannyLowThreshold { get; set; } = 0.08f;

    /// <summary>
    /// 
    /// </summary>
    public float CannyWeak { get; set; } = 0.8f;

    /// <summary>
    /// 
    /// </summary>
    public float CannyStrong { get; set; } = 1.0f;

    /// <summary>
    /// 
    /// </summary>
    public bool CannyInverse { get; set; } = false;
}