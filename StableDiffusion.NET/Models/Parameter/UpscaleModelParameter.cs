using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class UpscaleModelParameter : IQuantizedModelParameter
{
    public string ModelPath { get; set; } = string.Empty;
    public int ThreadCount { get; set; } = 1;

    public Quantization Quantization { get; set; } = Quantization.F16;
}