using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class DiffusionParameter
{
    #region Properties & Fields

    public static DiffusionParameter SD1Default => new() { Width = 512, Height = 512, CfgScale = 7.5f, Guidance = 1f, SampleSteps = 25, SampleMethod = Sampler.Euler_A };
    public static DiffusionParameter SDXLDefault => new() { Width = 1024, Height = 1024, CfgScale = 7f, Guidance = 1f, SampleSteps = 30, SampleMethod = Sampler.Euler_A };
    public static DiffusionParameter SD3_5Default => new() { Width = 1024, Height = 1024, CfgScale = 4.5f, Guidance = 1f, SampleSteps = 20, SampleMethod = Sampler.Euler };
    public static DiffusionParameter FluxDefault => new() { Width = 1024, Height = 1024, CfgScale = 1, Guidance = 3.5f, SampleSteps = 20, SampleMethod = Sampler.Euler };

    public string NegativePrompt { get; set; } = string.Empty;
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
    public Sampler SampleMethod { get; set; } = Sampler.Euler_A;
    public int SampleSteps { get; set; } = 25;
    public long Seed { get; set; } = -1;
    public float Strength { get; set; } = 0.7f;
    public int ClipSkip { get; set; } = -1;

    public ControlNetParameter ControlNet { get; } = new();

    // Stable Diffusion only
    public float CfgScale { get; set; } = 7.5f;
    public PhotoMakerParameter PhotoMaker { get; } = new();

    // Flux only
    public float Guidance { get; set; } = 3.5f;

    #endregion
}