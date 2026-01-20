using System.Collections.Generic;
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

    public IImage? InitImage { get; set; }

    public IImage[]? RefImages { get; set; }

    public bool IncreaseRefIndex { get; set; } = false;

    public bool AutoResizeRefImage { get; set; } = false;

    public IImage? MaskImage { get; set; }

    /// <summary>
    /// image width, in pixel space (default: 512)
    /// </summary>
    public int Width { get; set; } = 512;

    /// <summary>
    /// image height, in pixel space (default: 512)
    /// </summary>
    public int Height { get; set; } = 512;

    public SampleParameter SampleParameter { get; internal init; } = new();

    /// <summary>
    /// strength for noising/unnoising (default: 0.75)
    /// </summary>
    public float Strength { get; set; } = 0.75f;

    /// <summary>
    /// RNG seed. use -1 for a random seed (default: -1)
    /// </summary>
    public long Seed { get; set; } = -1;

    public ControlNetParameter ControlNet { get; } = new();

    public PhotoMakerParameter PhotoMaker { get; } = new();

    public TilingParameter VaeTiling { get; internal init; } = new();

    public CacheParameter Cache { get; internal init; } = new();

    public List<Lora> Loras { get; } = [];

    #endregion

    public static ImageGenerationParameter Create() => new();

    public static ImageGenerationParameter TextToImage(string prompt) => Create().WithPrompt(prompt);
    public static ImageGenerationParameter ImageToImage(string prompt, IImage image) => Create().WithPrompt(prompt).WithInitImage(image);
}