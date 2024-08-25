using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class PhotoMakerParameter
{
    public string InputIdImageDirectory { get; set; } = string.Empty;
    public float StyleRatio { get; set; } = 20f;
    public bool NormalizeInput { get; set; } = false;
}