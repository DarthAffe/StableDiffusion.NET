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

    /// <summary>
    /// the negative prompt (default: "");
    /// </summary>
    public string NegativePrompt { get; set; } = string.Empty;

    /// <summary>
    /// image width, in pixel space (default: 512)
    /// </summary>
    public int Width { get; set; } = 512;

    /// <summary>
    /// image height, in pixel space (default: 512)
    /// </summary>
    public int Height { get; set; } = 512;

    /// <summary>
    /// sampling method (default: Euler_A)
    /// </summary>
    public Sampler SampleMethod { get; set; } = Sampler.Euler_A;

    /// <summary>
    /// number of sample steps (default: 25)
    /// </summary>
    public int SampleSteps { get; set; } = 25;

    /// <summary>
    /// RNG seed. use -1 for a random seed (default: -1)
    /// </summary>
    public long Seed { get; set; } = -1;

    /// <summary>
    /// strength for noising/unnoising (default: 0.7)
    /// </summary>
    public float Strength { get; set; } = 0.7f;

    /// <summary>
    /// ignore last layers of CLIP network; 1 ignores none, 2 ignores one layer (default: -1)
    /// -1 represents unspecified, will be 1 for SD1.x, 2 for SD2.x
    /// </summary>
    public int ClipSkip { get; set; } = -1;

    /// <summary>
    /// skip layer guidance (SLG) scale, only for DiT models: (default: 0)
    /// 0 means disabled, a value of 2.5 is nice for sd3.5 medium
    /// </summary>
    public float SlgScale { get; set; } = 0f;

    /// <summary>
    /// Layers to skip for SLG steps: (default: [7,8,9])
    /// </summary>
    public int[] SkipLayers { get; set; } = [7, 8, 9];

    /// <summary>
    /// SLG enabling point: (default: 0.01)
    /// </summary>
    public float SkipLayerStart { get; set; } = 0.01f;

    /// <summary>
    /// SLG disabling point: (default: 0.2)
    /// </summary>
    public float SkipLayerEnd { get; set; } = 0.2f;

    public ControlNetParameter ControlNet { get; } = new();

    // Stable Diffusion only
    /// <summary>
    /// unconditional guidance scale: (default: 7.5)
    /// </summary>
    public float CfgScale { get; set; } = 7.5f;

    public PhotoMakerParameter PhotoMaker { get; } = new();

    // Flux only
    /// <summary>
    /// 
    /// </summary>
    public float Guidance { get; set; } = 3.5f;

    #endregion
}