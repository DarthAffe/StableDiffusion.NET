namespace StableDiffusion.NET;

public interface IBackend
{
    bool IsEnabled { get; set; }
    public int Priority { get; }
    bool IsAvailable { get; }
    string PathPart { get; }
}