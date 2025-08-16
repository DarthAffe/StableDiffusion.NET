#pragma warning disable CA2208

using JetBrains.Annotations;
using System;

namespace StableDiffusion.NET;

[PublicAPI]
public static class ParameterExtension
{
    public static void Validate(this UpscaleModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.ModelPath, nameof(UpscaleModelParameter.ModelPath));
    }

    public static void Validate(this DiffusionModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.ThreadCount, nameof(DiffusionModelParameter.ThreadCount));
        ArgumentNullException.ThrowIfNull(parameter.VaePath, nameof(DiffusionModelParameter.VaePath));
        ArgumentNullException.ThrowIfNull(parameter.TaesdPath, nameof(DiffusionModelParameter.TaesdPath));
        ArgumentNullException.ThrowIfNull(parameter.LoraModelDirectory, nameof(DiffusionModelParameter.LoraModelDirectory));
        ArgumentNullException.ThrowIfNull(parameter.ControlNetPath, nameof(DiffusionModelParameter.ControlNetPath));
        ArgumentNullException.ThrowIfNull(parameter.EmbeddingsDirectory, nameof(DiffusionModelParameter.EmbeddingsDirectory));
        ArgumentNullException.ThrowIfNull(parameter.StackedIdEmbeddingsDirectory, nameof(DiffusionModelParameter.StackedIdEmbeddingsDirectory));

        if (!string.IsNullOrWhiteSpace(parameter.VaePath) && !string.IsNullOrWhiteSpace(parameter.TaesdPath)) throw new ArgumentException("VAE and TAESD are mutually exclusive.");

        if (!Enum.IsDefined(parameter.RngType)) throw new ArgumentOutOfRangeException(nameof(DiffusionModelParameter.RngType));
        if (!Enum.IsDefined(parameter.Schedule)) throw new ArgumentOutOfRangeException(nameof(DiffusionModelParameter.Schedule));
    }

    public static void Validate(this ImageGenerationParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.ControlNet, nameof(ImageGenerationParameter.ControlNet));
        ArgumentNullException.ThrowIfNull(parameter.PhotoMaker, nameof(ImageGenerationParameter.PhotoMaker));
        ArgumentNullException.ThrowIfNull(parameter.NegativePrompt, nameof(ImageGenerationParameter.NegativePrompt));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.Width, nameof(ImageGenerationParameter.Width));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.Height, nameof(ImageGenerationParameter.Height));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.SampleSteps, nameof(ImageGenerationParameter.SampleSteps));

        if (!Enum.IsDefined(parameter.SampleMethod)) throw new ArgumentOutOfRangeException(nameof(ImageGenerationParameter.SampleMethod));

        parameter.Guidance.Validate();
        parameter.ControlNet.Validate();
        parameter.PhotoMaker.Validate();
    }

    public static void Validate(this ControlNetParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(ImageGenerationParameter.ControlNet));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Strength, nameof(ControlNetParameter.Strength));
    }

    public static void Validate(this PhotoMakerParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(ImageGenerationParameter.PhotoMaker));
        ArgumentNullException.ThrowIfNull(parameter.InputIdImageDirectory, nameof(PhotoMakerParameter.InputIdImageDirectory));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.StyleStrength, nameof(PhotoMakerParameter.StyleStrength));
    }

    public static void Validate(this GuidanceParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.ImgCfg);
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.DistilledGuidance);
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.MinCfg);
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.TxtCfg);

        parameter.Slg.Validate();
    }

    public static void Validate(this SlgParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.Layers);

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Scale);
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.SkipLayerStart);
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.SkipLayerEnd);
    }
    
    public static void Validate(this CannyParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        ArgumentNullException.ThrowIfNull(parameter.Image);

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.HighThreshold, nameof(CannyParameter.HighThreshold));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.LowThreshold, nameof(CannyParameter.LowThreshold));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Weak, nameof(CannyParameter.Weak));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Strong, nameof(CannyParameter.Strong));
    }
}