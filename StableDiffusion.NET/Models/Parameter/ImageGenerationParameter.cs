using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class ImageGenerationParameter
{
    #region Properties & Fields

    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// the negative prompt (default: "");
    /// </summary>
    public string NegativePrompt { get; set; } = string.Empty;

    /// <summary>
    /// ignore last layers of CLIP network; 1 ignores none, 2 ignores one layer (default: -1)
    /// -1 represents unspecified, will be 1 for SD1.x, 2 for SD2.x
    /// </summary>
    public int ClipSkip { get; set; } = -1;

    public GuidanceParameter Guidance { get; } = new();

    public IImage? InitImage { get; set; }

    public IImage[]? RefImages { get; set; }

    public IImage? MaskImage { get; set; }

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
    /// eta in DDIM, only for DDIM and TCD (default: 0)
    /// </summary>
    public float Eta { get; set; } = 0f;

    /// <summary>
    /// strength for noising/unnoising (default: 0.7)
    /// </summary>
    public float Strength { get; set; } = 0.7f;

    /// <summary>
    /// RNG seed. use -1 for a random seed (default: -1)
    /// </summary>
    public long Seed { get; set; } = -1;

    public ControlNetParameter ControlNet { get; } = new();

    public PhotoMakerParameter PhotoMaker { get; } = new();

    public static ImageGenerationParameter Create() => new();

    public static ImageGenerationParameter TextToImage(string prompt) => Create().WithPrompt(prompt);
    public static ImageGenerationParameter ImageToImage(string prompt, IImage image) => Create().WithPrompt(prompt).WithInitImage(image);

    #endregion
}