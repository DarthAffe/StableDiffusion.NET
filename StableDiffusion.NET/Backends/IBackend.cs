using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public interface IBackend
{
    bool IsEnabled { get; set; }
    public int Priority { get; }
    bool IsAvailable { get; }
    string PathPart { get; }
}