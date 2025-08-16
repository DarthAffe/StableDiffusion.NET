using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public interface IModelBuilder
{
    IModelParameter Parameter { get; }
}
