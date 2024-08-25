using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class ControlNetParameter
{
    public bool IsEnabled => Image != null;

    public IImage? Image { get; set; } = null;
    public float Strength { get; set; } = 0.9f;
    public bool CannyPreprocess { get; set; } = false;
    public float CannyHighThreshold { get; set; } = 0.08f;
    public float CannyLowThreshold { get; set; } = 0.08f;
    public float CannyWeak { get; set; } = 0.8f;
    public float CannyStrong { get; set; } = 1.0f;
    public bool CannyInverse { get; set; } = false;
}