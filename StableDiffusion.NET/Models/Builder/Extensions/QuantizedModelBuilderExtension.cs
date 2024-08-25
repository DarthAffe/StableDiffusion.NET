using System;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class QuantizedModelBuilderExtension
{
    public static T WithoutMultithreading<T>(this T builder)
        where T : IQuantizedModelBuilder
    {
        builder.Parameter.ThreadCount = 1;

        return builder;
    }

    public static T WithMultithreading<T>(this T builder, int threadCount = 0)
        where T : IQuantizedModelBuilder
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threadCount, 0, nameof(threadCount));

        if (threadCount == 0) threadCount = Environment.ProcessorCount;

        builder.Parameter.ThreadCount = threadCount;

        return builder;
    }

    public static T WithQuantization<T>(this T builder, Quantization quantization)
        where T : IQuantizedModelBuilder
    {
        builder.Parameter.Quantization = quantization;

        return builder;
    }
}