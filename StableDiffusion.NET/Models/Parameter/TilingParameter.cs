namespace StableDiffusion.NET;

public sealed class TilingParameter
{
    public bool IsEnabled { get; set; }

    public int TileSizeX { get; set; }
    public int TileSizeY { get; set; }
    public float TargetOverlap { get; set; }
    public float RelSizeX { get; set; }
    public float RelSizeY { get; set; }

    internal TilingParameter() { }
}