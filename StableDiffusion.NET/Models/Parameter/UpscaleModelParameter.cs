using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class UpscaleModelParameter : IQuantizedModelParameter
{
    /// <summary>
    /// path to esrgan model. Upscale images after generate, just RealESRGAN_x4plus_anime_6B supported by now
    /// </summary>
    public string ModelPath { get; set; } = string.Empty;

    /// <summary>
    /// number of threads to use during computation (default: -1)
    /// If threads = -1, then threads will be set to the number of CPU physical cores
    /// </summary>
    public int ThreadCount { get; set; } = 1;

    /// <summary>
    /// 
    /// </summary>
    public Quantization Quantization { get; set; } = Quantization.F16;
}