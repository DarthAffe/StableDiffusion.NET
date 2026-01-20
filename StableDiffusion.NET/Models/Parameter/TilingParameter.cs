namespace StableDiffusion.NET;

public sealed class TilingParameter
{
    public bool IsEnabled { get; set; } = false;

    public int TileSizeX { get; set; } = 0;
    public int TileSizeY { get; set; } = 0;
    public float TargetOverlap { get; set; } = 0.5f;
    public float RelSizeX { get; set; } = 0;
    public float RelSizeY { get; set; } = 0;

    internal TilingParameter() { }
}