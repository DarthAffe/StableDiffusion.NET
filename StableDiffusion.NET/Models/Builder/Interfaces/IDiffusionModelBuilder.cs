using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public interface IDiffusionModelBuilder : IModelBuilder
{
    IDiffusionModelParameter Parameter { get; }

    DiffusionModel Build();
}