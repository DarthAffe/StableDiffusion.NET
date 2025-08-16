using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public interface IUpscaleModelParameter : IModelParameter
{
    string ModelPath { get; set; }
    bool ConvDirect { get; set; }
}