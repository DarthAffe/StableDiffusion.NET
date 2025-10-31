using System;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class DiffusionModelBuilderExtension
{
    public static DiffusionModelParameter WithVae(this DiffusionModelParameter parameter, string vaePath)
    {
        ArgumentNullException.ThrowIfNull(vaePath);

        if (!string.IsNullOrWhiteSpace(parameter.TaesdPath)) throw new ArgumentException("TAESD is already enabled. VAE and TAESD are mutually exclusive.", nameof(vaePath));

        parameter.VaePath = vaePath;

        return parameter;
    }

    public static DiffusionModelParameter WithTaesd(this DiffusionModelParameter parameter, string taesdPath)
    {
        ArgumentNullException.ThrowIfNull(taesdPath);

        if (!string.IsNullOrWhiteSpace(parameter.VaePath)) throw new ArgumentException("VAE is already enabled. TAESD and VAE are mutually exclusive.", nameof(taesdPath));

        parameter.TaesdPath = taesdPath;

        return parameter;
    }

    public static DiffusionModelParameter WithLoraSupport(this DiffusionModelParameter parameter, string loraModelDirectory)
    {
        ArgumentNullException.ThrowIfNull(loraModelDirectory);

        parameter.LoraModelDirectory = loraModelDirectory;

        return parameter;
    }

    public static DiffusionModelParameter WithEmbeddingSupport(this DiffusionModelParameter parameter, string embeddingsDirectory)
    {
        ArgumentNullException.ThrowIfNull(embeddingsDirectory);

        parameter.EmbeddingsDirectory = embeddingsDirectory;

        return parameter;
    }

    public static DiffusionModelParameter WithControlNet(this DiffusionModelParameter parameter, string controlNetPath)
    {
        ArgumentNullException.ThrowIfNull(controlNetPath);

        parameter.ControlNetPath = controlNetPath;

        return parameter;
    }

    public static DiffusionModelParameter WithoutMultithreading(this DiffusionModelParameter parameter)
    {
        parameter.ThreadCount = 1;

        return parameter;
    }

    public static DiffusionModelParameter WithMultithreading(this DiffusionModelParameter parameter, int threadCount = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threadCount, 0, nameof(threadCount));

        if (threadCount == 0) threadCount = Environment.ProcessorCount;

        parameter.ThreadCount = threadCount;

        return parameter;
    }

    public static DiffusionModelParameter WithVaeDecodeOnly(this DiffusionModelParameter parameter, bool vaeDecodeOnly = true)
    {
        parameter.VaeDecodeOnly = vaeDecodeOnly;

        return parameter;
    }

    public static DiffusionModelParameter WithImmediatelyFreedParams(this DiffusionModelParameter parameter, bool immediatelyFreedParams = true)
    {
        parameter.FreeParamsImmediately = immediatelyFreedParams;

        return parameter;
    }

    public static DiffusionModelParameter WithVaeTiling(this DiffusionModelParameter parameter, bool vaeTiling = true)
    {
        parameter.VaeTiling = vaeTiling;

        return parameter;
    }

    public static DiffusionModelParameter WithOffloadedParamsToCPU(this DiffusionModelParameter parameter, bool offloadParamsToCPU = true)
    {
        parameter.OffloadParamsToCPU = offloadParamsToCPU;

        return parameter;
    }

    public static DiffusionModelParameter WithClipNetOnCpu(this DiffusionModelParameter parameter, bool keepClipNetOnCpu = true)
    {
        parameter.KeepClipOnCPU = keepClipNetOnCpu;

        return parameter;
    }

    public static DiffusionModelParameter WithControlNetOnCpu(this DiffusionModelParameter parameter, bool keepControlNetOnCpu = true)
    {
        parameter.KeepControlNetOnCPU = keepControlNetOnCpu;

        return parameter;
    }

    public static DiffusionModelParameter WithVaeOnCpu(this DiffusionModelParameter parameter, bool keepVaeOnCpu = true)
    {
        parameter.KeepVaeOnCPU = keepVaeOnCpu;

        return parameter;
    }

    public static DiffusionModelParameter WithFlashAttention(this DiffusionModelParameter parameter, bool flashAttention = true)
    {
        parameter.FlashAttention = flashAttention;

        return parameter;
    }

    public static DiffusionModelParameter WithDiffusionConvDirect(this DiffusionModelParameter parameter, bool diffusionConvDirect = true)
    {
        parameter.DiffusionConvDirect = diffusionConvDirect;

        return parameter;
    }

    public static DiffusionModelParameter WithVaeConvDirect(this DiffusionModelParameter parameter, bool vaeConvDirect = true)
    {
        parameter.VaeConvDirect = vaeConvDirect;

        return parameter;
    }

    public static DiffusionModelParameter WithRngType(this DiffusionModelParameter parameter, RngType rngType)
    {
        if (!Enum.IsDefined(rngType)) throw new ArgumentOutOfRangeException(nameof(rngType));

        parameter.RngType = rngType;

        return parameter;
    }

    public static DiffusionModelParameter WithPrediction(this DiffusionModelParameter parameter, Prediction prediction)
    {
        parameter.Prediction = prediction;

        return parameter;
    }

    public static DiffusionModelParameter WithQuantization(this DiffusionModelParameter parameter, Quantization quantization)
    {
        if (!Enum.IsDefined(quantization)) throw new ArgumentOutOfRangeException(nameof(quantization));

        parameter.Quantization = quantization;

        return parameter;
    }

    public static DiffusionModelParameter WithFlowShift(this DiffusionModelParameter parameter, float flowShift)
    {
        parameter.FlowShift = flowShift;

        return parameter;
    }

    public static DiffusionModelParameter WithForcedSdxlVaeConvScale(this DiffusionModelParameter parameter, bool forcedScale = true)
    {
        parameter.ForceSdxlVaeConvScale = forcedScale;

        return parameter;
    }

    public static DiffusionModelParameter WithPhotomaker(this DiffusionModelParameter parameter, string stackedIdEmbeddingsDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stackedIdEmbeddingsDirectory, nameof(stackedIdEmbeddingsDirectory));

        parameter.StackedIdEmbeddingsDirectory = stackedIdEmbeddingsDirectory;

        return parameter;
    }

    public static DiffusionModelParameter WithModelPath(this DiffusionModelParameter parameter, string modelPath)
    {
        ArgumentNullException.ThrowIfNull(modelPath);

        parameter.ModelPath = modelPath;

        return parameter;
    }

    public static DiffusionModelParameter WithDiffusionModelPath(this DiffusionModelParameter parameter, string diffusionModelPath)
    {
        ArgumentNullException.ThrowIfNull(diffusionModelPath);

        parameter.DiffusionModelPath = diffusionModelPath;

        return parameter;
    }

    public static DiffusionModelParameter WithClipLPath(this DiffusionModelParameter parameter, string clipLPath)
    {
        ArgumentNullException.ThrowIfNull(clipLPath);

        parameter.ClipLPath = clipLPath;

        return parameter;
    }

    public static DiffusionModelParameter WithClipGPath(this DiffusionModelParameter parameter, string clipGPath)
    {
        ArgumentNullException.ThrowIfNull(clipGPath);

        parameter.ClipGPath = clipGPath;

        return parameter;
    }

    public static DiffusionModelParameter WithT5xxlPath(this DiffusionModelParameter parameter, string t5xxlPath)
    {
        ArgumentNullException.ThrowIfNull(t5xxlPath);

        parameter.T5xxlPath = t5xxlPath;

        return parameter;
    }

    public static DiffusionModelParameter WithQwen2VLPath(this DiffusionModelParameter parameter, string qwen2VLPath)
    {
        parameter.Qwen2VLPath = qwen2VLPath;

        return parameter;
    }

    public static DiffusionModelParameter WithQwen2VLVisionPath(this DiffusionModelParameter parameter, string qwen2VLVisionPath)
    {
        parameter.Qwen2VLVisionPath = qwen2VLVisionPath;

        return parameter;
    }

    public static DiffusionModelParameter WithClipVisionPath(this DiffusionModelParameter parameter, string clipVisionPath)
    {
        parameter.ClipVisionPath = clipVisionPath;

        return parameter;
    }

    public static DiffusionModelParameter WithHighNoiseDiffusionModelPath(this DiffusionModelParameter parameter, string highNoiseDiffusionModelPath)
    {
        parameter.HighNoiseDiffusionModelPath = highNoiseDiffusionModelPath;

        return parameter;
    }

    public static DiffusionModelParameter UseChromaDitMap(this DiffusionModelParameter parameter, bool useChromaDitMap = true)
    {
        parameter.ChromaUseDitMap = useChromaDitMap;

        return parameter;
    }

    public static DiffusionModelParameter EnableChromaT5Map(this DiffusionModelParameter parameter, bool enableChromaT5Map = true)
    {
        parameter.ChromaEnableT5Map = enableChromaT5Map;

        return parameter;
    }

    public static DiffusionModelParameter WithChromaT5MaskPad(this DiffusionModelParameter parameter, int chromaT5MaskPad)
    {
        parameter.ChromaT5MaskPad = chromaT5MaskPad;

        return parameter;
    }
}