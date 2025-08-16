using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public interface IModelParameter
{
    int ThreadCount { get; set; }
}