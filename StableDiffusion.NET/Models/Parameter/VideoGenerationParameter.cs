using HPPH;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class VideoGenerationParameter
{
    #region Properties & Fields

    public string Prompt { get; set; } = string.Empty;

    public string NegativePrompt { get; set; } = string.Empty;

    public int ClipSkip { get; set; } = -1;

    public IImage? InitImage { get; set; }

    public IImage? EndImage { get; set; }

    public IImage[]? ControlFrames { get; set; }

    public int Width { get; set; } = 512;

    public int Height { get; set; } = 512;

    public SampleParameter SampleParameter { get; internal init; } = new();

    public SampleParameter HighNoiseSampleParameter { get; internal init; } = new() { SampleSteps = -1 };

    public float MoeBoundry { get; set; } = 0.875f;

    public float Strength { get; set; } = 0.75f;

    public long Seed { get; set; } = -1;

    public int FrameCount { get; set; } = 6;

    public float VaceStrength { get; set; } = 1.0f;

    public TilingParameter VaeTiling { get; internal init; } = new();

    public CacheParameter Cache { get; internal init; } = new();

    public List<Lora> Loras { get; } = [];

    #endregion

    public static VideoGenerationParameter Create() => new();

    public static VideoGenerationParameter TextToVideo(string prompt) => Create().WithPrompt(prompt);
}