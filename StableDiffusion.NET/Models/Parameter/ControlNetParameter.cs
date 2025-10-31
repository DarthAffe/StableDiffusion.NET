using HPPH;

namespace StableDiffusion.NET;

public sealed class ControlNetParameter
{
    /// <summary>
    /// image condition, control net
    /// </summary>
    public IImage? Image { get; set; } = null;

    /// <summary>
    /// strength to apply Control Net (default: 0.9)
    /// 1.0 corresponds to full destruction of information in init image
    /// </summary>
    public float Strength { get; set; } = 0.9f;

    internal ControlNetParameter() { }
}