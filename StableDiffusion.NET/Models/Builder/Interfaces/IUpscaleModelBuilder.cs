using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public interface IUpscaleModelBuilder : IModelBuilder
{
    IUpscaleModelParameter Parameter { get; }

    UpscaleModel Build();
}