using System;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class PhotomakerModelBuilderExtension
{
    public static T WithPhotomaker<T>(this T builder, string stackedIdEmbeddingsDirectory)
        where T : IPhotomakerModelBuilder
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stackedIdEmbeddingsDirectory, nameof(stackedIdEmbeddingsDirectory));

        builder.Parameter.StackedIdEmbeddingsDirectory = stackedIdEmbeddingsDirectory;

        return builder;
    }
}