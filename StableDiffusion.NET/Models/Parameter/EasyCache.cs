namespace StableDiffusion.NET;

public sealed class EasyCache
{
    public bool IsEnabled { get; set; }
    public float ReuseThreshold { get; set; }
    public float StartPercent { get; set; }
    public float EndPercent { get; set; }

    internal EasyCache() { }
}