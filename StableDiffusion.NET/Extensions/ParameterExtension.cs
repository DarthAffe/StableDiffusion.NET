#pragma warning disable CA2208

using System;

namespace StableDiffusion.NET;

public static class ParameterExtension
{
    public static void Validate(this StableDiffusionParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.NegativePrompt, nameof(StableDiffusionParameter.NegativePrompt));

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.Width, nameof(StableDiffusionParameter.Width));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.Height, nameof(StableDiffusionParameter.Height));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(parameter.SampleSteps, nameof(StableDiffusionParameter.SampleSteps));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CfgScale, nameof(StableDiffusionParameter.CfgScale));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Strength, nameof(StableDiffusionParameter.Strength));

        if (!Enum.IsDefined(parameter.SampleMethod)) throw new ArgumentOutOfRangeException(nameof(StableDiffusionParameter.SampleMethod));

        // ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        parameter.ControlNet?.Validate();
        parameter.PhotoMaker?.Validate();
        // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    }

    public static void Validate(this StableDiffusionControlNetParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(StableDiffusionParameter.ControlNet));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.Strength, nameof(StableDiffusionControlNetParameter.Strength));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyHighThreshold, nameof(StableDiffusionControlNetParameter.CannyHighThreshold));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyLowThreshold, nameof(StableDiffusionControlNetParameter.CannyLowThreshold));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyWeak, nameof(StableDiffusionControlNetParameter.CannyWeak));
        ArgumentOutOfRangeException.ThrowIfNegative(parameter.CannyStrong, nameof(StableDiffusionControlNetParameter.CannyStrong));
    }

    public static void Validate(this PhotoMakerParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(StableDiffusionParameter.PhotoMaker));
        ArgumentNullException.ThrowIfNull(parameter.InputIdImageDirectory, nameof(PhotoMakerParameter.InputIdImageDirectory));

        ArgumentOutOfRangeException.ThrowIfNegative(parameter.StyleRatio, nameof(PhotoMakerParameter.StyleRatio));
    }

    public static void Validate(this ModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.TaesdPath, nameof(ModelParameter.TaesdPath));
        ArgumentNullException.ThrowIfNull(parameter.LoraModelDir, nameof(ModelParameter.LoraModelDir));
        ArgumentNullException.ThrowIfNull(parameter.VaePath, nameof(ModelParameter.VaePath));
        ArgumentNullException.ThrowIfNull(parameter.ControlNetPath, nameof(ModelParameter.ControlNetPath));
        ArgumentNullException.ThrowIfNull(parameter.EmbeddingsDirectory, nameof(ModelParameter.EmbeddingsDirectory));
        ArgumentNullException.ThrowIfNull(parameter.StackedIdEmbeddingsDirectory, nameof(ModelParameter.StackedIdEmbeddingsDirectory));

        if (!Enum.IsDefined(parameter.RngType)) throw new ArgumentOutOfRangeException(nameof(ModelParameter.RngType));
        if (!Enum.IsDefined(parameter.Quantization)) throw new ArgumentOutOfRangeException(nameof(ModelParameter.Quantization));
        if (!Enum.IsDefined(parameter.Schedule)) throw new ArgumentOutOfRangeException(nameof(ModelParameter.Schedule));
    }

    public static void Validate(this UpscalerModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));
        ArgumentNullException.ThrowIfNull(parameter.ESRGANPath, nameof(UpscalerModelParameter.ESRGANPath));

        if (!Enum.IsDefined(parameter.Quantization)) throw new ArgumentOutOfRangeException(nameof(ModelParameter.Quantization));
    }
}