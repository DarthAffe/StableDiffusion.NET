using System;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class DiffusionModelBuilderExtension
{
    public static T WithVae<T>(this T builder, string vaePath)
        where T : IDiffusionModelBuilder
    {
        ArgumentNullException.ThrowIfNull(vaePath);

        if (!string.IsNullOrWhiteSpace(builder.Parameter.TaesdPath)) throw new ArgumentException("TAESD is already enabled. VAE and TAESD are mutually exclusive.", nameof(vaePath));

        builder.Parameter.VaePath = vaePath;

        return builder;
    }

    public static T WithTaesd<T>(this T builder, string taesdPath)
        where T : IDiffusionModelBuilder
    {
        ArgumentNullException.ThrowIfNull(taesdPath);

        if (!string.IsNullOrWhiteSpace(builder.Parameter.VaePath)) throw new ArgumentException("VAE is already enabled. TAESD and VAE are mutually exclusive.", nameof(taesdPath));

        builder.Parameter.TaesdPath = taesdPath;

        return builder;
    }

    public static T WithLoraSupport<T>(this T builder, string loraModelDirectory)
        where T : IDiffusionModelBuilder
    {
        ArgumentNullException.ThrowIfNull(loraModelDirectory);

        builder.Parameter.LoraModelDirectory = loraModelDirectory;

        return builder;
    }

    public static T WithEmbeddingSupport<T>(this T builder, string embeddingsDirectory)
        where T : IDiffusionModelBuilder
    {
        ArgumentNullException.ThrowIfNull(embeddingsDirectory);

        builder.Parameter.EmbeddingsDirectory = embeddingsDirectory;

        return builder;
    }

    public static T WithControlNet<T>(this T builder, string controlNetPath)
        where T : IDiffusionModelBuilder
    {
        ArgumentNullException.ThrowIfNull(controlNetPath);

        builder.Parameter.ControlNetPath = controlNetPath;

        return builder;
    }

    public static T WithVaeDecodeOnly<T>(this T builder, bool vaeDecodeOnly = true)
        where T : IDiffusionModelBuilder
    {
        builder.Parameter.VaeDecodeOnly = vaeDecodeOnly;

        return builder;
    }

    public static T WithVaeTiling<T>(this T builder, bool vaeTiling = true)
        where T : IDiffusionModelBuilder
    {
        builder.Parameter.VaeTiling = vaeTiling;

        return builder;
    }

    public static T KeepVaeOnCpu<T>(this T builder, bool keepVaeOnCpu = true)
        where T : IDiffusionModelBuilder
    {
        builder.Parameter.KeepVaeOnCPU = keepVaeOnCpu;

        return builder;
    }

    public static T KeepClipNetOnCpu<T>(this T builder, bool keepClipNetOnCpu = true)
        where T : IDiffusionModelBuilder
    {
        builder.Parameter.KeepClipOnCPU = keepClipNetOnCpu;

        return builder;
    }

    public static T KeepControlNetOnCpu<T>(this T builder, bool keepControlNetOnCpu = true)
        where T : IDiffusionModelBuilder
    {
        builder.Parameter.KeepControlNetOnCPU = keepControlNetOnCpu;

        return builder;
    }

    public static T WithRngType<T>(this T builder, RngType rngType)
        where T : IDiffusionModelBuilder
    {
        if (!Enum.IsDefined(rngType)) throw new ArgumentOutOfRangeException(nameof(rngType));

        builder.Parameter.RngType = rngType;

        return builder;
    }

    public static T WithSchedule<T>(this T builder, Schedule schedule)
        where T : IDiffusionModelBuilder
    {
        if (!Enum.IsDefined(schedule)) throw new ArgumentOutOfRangeException(nameof(schedule));

        builder.Parameter.Schedule = schedule;

        return builder;
    }

    public static T WithFlashAttention<T>(this T builder, bool flashAttention = true)
        where T : IDiffusionModelBuilder
    {
        builder.Parameter.FlashAttention = flashAttention;

        return builder;
    }
}