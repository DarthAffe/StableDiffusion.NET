using System;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class UpscaleModelBuilderExtension
{
    public static T WithModelPath<T>(this T builder, string modelPath)
        where T : IUpscaleModelBuilder
    {
        ArgumentNullException.ThrowIfNull(modelPath);

        builder.Parameter.ModelPath = modelPath;

        return builder;
    }

    public static T WithConvDirect<T>(this T builder, bool confDirect = true)
        where T : IUpscaleModelBuilder
    {
        builder.Parameter.ConvDirect = confDirect;

        return builder;
    }

}