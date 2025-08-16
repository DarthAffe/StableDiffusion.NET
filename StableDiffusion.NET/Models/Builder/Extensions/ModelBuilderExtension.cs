using System;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class ModelBuilderExtension
{
    public static T WithoutMultithreading<T>(this T builder)
        where T : IModelBuilder
    {
        builder.Parameter.ThreadCount = 1;

        return builder;
    }

    public static T WithMultithreading<T>(this T builder, int threadCount = 0)
        where T : IModelBuilder
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(threadCount, 0, nameof(threadCount));

        if (threadCount == 0) threadCount = Environment.ProcessorCount;

        builder.Parameter.ThreadCount = threadCount;

        return builder;
    }
}