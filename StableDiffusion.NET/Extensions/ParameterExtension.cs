#pragma warning disable CA2208

using System;

namespace StableDiffusion.NET;

public static class ParameterExtension
{
    public static void Validate(this UpscaleModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.ModelPath, nameof(UpscaleModelParameter.ModelPath));

        if (!Enum.IsDefined(parameter.Quantization)) throw new ArgumentOutOfRangeException(nameof(UpscaleModelParameter.Quantization));
    }

    public static void Validate(this DiffusionModelParameter parameter)
    {
        ((IQuantizedModelParameter)parameter).Validate();
        ((IDiffusionModelParameter)parameter).Validate();
        ((IPhotomakerModelParameter)parameter).Validate();
    }

    public static void Validate(this IQuantizedModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.ThreadCount, nameof(IQuantizedModelParameter.ThreadCount));

        if (!Enum.IsDefined(parameter.Quantization)) throw new ArgumentOutOfRangeException(nameof(IQuantizedModelParameter.Quantization));
    }

    public static void Validate(this IPhotomakerModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.StackedIdEmbeddingsDirectory, nameof(IPhotomakerModelParameter.StackedIdEmbeddingsDirectory));
    }

    public static void Validate(this IDiffusionModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.VaePath, nameof(IDiffusionModelParameter.VaePath));
        ArgumentNullException.ThrowIfNull(parameter.TaesdPath, nameof(IDiffusionModelParameter.TaesdPath));
        ArgumentNullException.ThrowIfNull(parameter.LoraModelDirectory, nameof(IDiffusionModelParameter.LoraModelDirectory));
        ArgumentNullException.ThrowIfNull(parameter.ControlNetPath, nameof(IDiffusionModelParameter.ControlNetPath));
        ArgumentNullException.ThrowIfNull(parameter.EmbeddingsDirectory, nameof(IDiffusionModelParameter.EmbeddingsDirectory));

        if (!string.IsNullOrWhiteSpace(parameter.VaePath) && !string.IsNullOrWhiteSpace(parameter.TaesdPath)) throw new ArgumentException("VAE and TAESD are mutually exclusive.");

        if (!Enum.IsDefined(parameter.RngType)) throw new ArgumentOutOfRangeException(nameof(IDiffusionModelParameter.RngType));
        if (!Enum.IsDefined(parameter.Schedule)) throw new ArgumentOutOfRangeException(nameof(IDiffusionModelParameter.Schedule));
    }

    public static void Validate(this DiffusionParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.ControlNet, nameof(DiffusionParameter.ControlNet));
        ArgumentNullException.ThrowIfNull(parameter.PhotoMaker, nameof(DiffusionParameter.PhotoMaker));
        ArgumentNullException.ThrowIfNull(parameter.NegativePrompt, nameof(DiffusionParameter.NegativePrompt));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.Width, nameof(DiffusionParameter.Width));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.Height, nameof(DiffusionParameter.Height));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.SampleSteps, nameof(DiffusionParameter.SampleSteps));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CfgScale, nameof(DiffusionParameter.CfgScale));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Guidance, nameof(DiffusionParameter.Guidance));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Strength, nameof(DiffusionParameter.Strength));

        if (!Enum.IsDefined(parameter.SampleMethod)) throw new ArgumentOutOfRangeException(nameof(DiffusionParameter.SampleMethod));

        parameter.ControlNet.Validate();
        parameter.PhotoMaker.Validate();
    }

    public static void Validate(this ControlNetParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(DiffusionParameter.ControlNet));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Strength, nameof(ControlNetParameter.Strength));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyHighThreshold, nameof(ControlNetParameter.CannyHighThreshold));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyLowThreshold, nameof(ControlNetParameter.CannyLowThreshold));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyWeak, nameof(ControlNetParameter.CannyWeak));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyStrong, nameof(ControlNetParameter.CannyStrong));
    }

    public static void Validate(this PhotoMakerParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(DiffusionParameter.PhotoMaker));
        ArgumentNullException.ThrowIfNull(parameter.InputIdImageDirectory, nameof(PhotoMakerParameter.InputIdImageDirectory));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.StyleRatio, nameof(PhotoMakerParameter.StyleRatio));
    }
}