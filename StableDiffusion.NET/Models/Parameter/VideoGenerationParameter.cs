using HPPH;
using JetBrains.Annotations;

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

    public int Width { get; set; }

    public int Height { get; set; }

    public SampleParameter SampleParameter { get; internal init; } = new();

    public SampleParameter HighNoiseSampleParameter { get; internal init; } = new();

    public float MoeBoundry { get; set; }

    public float Strength { get; set; }

    public long Seed { get; set; }

    public int FrameCount { get; set; }

    public float VaceStrength { get; set; }

    #endregion

    public static VideoGenerationParameter Create() => new();

    public static VideoGenerationParameter TextToVideo(string prompt) => Create().WithPrompt(prompt);
}