using System;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class UpscaleModelBuilderExtension
{
    public static UpscaleModelParameter WithModelPath(this UpscaleModelParameter parameter, string modelPath)
    {
        ArgumentNullException.ThrowIfNull(modelPath);

        parameter.ModelPath = modelPath;

        return parameter;
    }

    public static UpscaleModelParameter WithOffloadedParamsToCPU(this UpscaleModelParameter parameter, bool offloadParamsToCPU = true)
    {
        parameter.OffloadParamsToCPU = offloadParamsToCPU;

        return parameter;
    }

    public static UpscaleModelParameter WithoutMultithreading(this UpscaleModelParameter parameter)
    {
        parameter.ThreadCount = 1;

        return parameter;
    }

    public static UpscaleModelParameter WithMultithreading(this UpscaleModelParameter parameter, int threadCount = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threadCount, 0, nameof(threadCount));

        if (threadCount == 0) threadCount = Environment.ProcessorCount;

        parameter.ThreadCount = threadCount;

        return parameter;
    }

    public static UpscaleModelParameter WithConvDirect(this UpscaleModelParameter parameter, bool convDirect = true)
    {
        parameter.ConvDirect = convDirect;

        return parameter;
    }
}