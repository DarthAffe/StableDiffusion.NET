using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class HiresParameter
{
    public bool Enabled { get; set; }
    public HiresUpscaler Upscaler { get; set; }
    public string? ModelPath { get; set; }
    public float Scale { get; set; }
    public int TargetWidth { get; set; }
    public int TargetHeight { get; set; }
    public int Steps { get; set; }
    public float DenoisingStrength { get; set; }
    public int UpscaleTileSize { get; set; }

    internal HiresParameter() { }
}
