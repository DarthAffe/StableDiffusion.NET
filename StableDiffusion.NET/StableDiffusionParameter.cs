﻿using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class StableDiffusionParameter
{
    #region Properties & Fields

    public string NegativePrompt { get; set; } = string.Empty;
    public float CfgScale { get; set; } = 7.5f;
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
    public Sampler SampleMethod { get; set; } = Sampler.Euler_A;
    public int SampleSteps { get; set; } = 25;
    public long Seed { get; set; } = -1;
    public float Strength { get; set; } = 0.7f;
    public int ClipSkip { get; set; } = -1;

    public StableDiffusionControlNetParameter ControlNet { get; } = new();
    public PhotoMakerParameter PhotoMaker { get; } = new();

    #endregion
}

[PublicAPI]
public sealed class StableDiffusionControlNetParameter
{
    public bool IsEnabled => Image != null;

    public IImage<ColorRGB>? Image { get; set; } = null;
    public float Strength { get; set; } = 0.9f;
    public bool CannyPreprocess { get; set; } = false;
    public float CannyHighThreshold { get; set; } = 0.08f;
    public float CannyLowThreshold { get; set; } = 0.08f;
    public float CannyWeak { get; set; } = 0.8f;
    public float CannyStrong { get; set; } = 1.0f;
    public bool CannyInverse { get; set; } = false;
}

[PublicAPI]
public sealed class PhotoMakerParameter
{
    public string InputIdImageDirectory { get; set; } = string.Empty;
    public float StyleRatio { get; set; } = 20f;
    public bool NormalizeInput { get; set; } = false;
}