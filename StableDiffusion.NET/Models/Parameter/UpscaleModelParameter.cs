using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class UpscaleModelParameter
{
    /// <summary>
    /// path to esrgan model. Upscale images after generate, just RealESRGAN_x4plus_anime_6B supported by now
    /// </summary>
    public string ModelPath { get; set; } = string.Empty;

    public bool OffloadParamsToCPU { get; set; } = false;

    /// <summary>
    /// number of threads to use during computation (default: -1)
    /// If threads = -1, then threads will be set to the number of CPU physical cores
    /// </summary>
    public int ThreadCount { get; set; } = 1;

    /// <summary>
    /// use Conv2d direct in the diffusion model
    /// This might crash if it is not supported by the backend.
    /// </summary>
    public bool ConvDirect { get; set; } = false;

    public static UpscaleModelParameter Create() => new();
}