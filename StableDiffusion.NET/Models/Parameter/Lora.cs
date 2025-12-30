using System.Diagnostics.CodeAnalysis;

namespace StableDiffusion.NET;

public sealed class Lora
{
    public bool IsHighNoise { get; set; } = false;
    public float Multiplier { get; set; } = 1f;
    public required string Path { get; init; }

    public Lora() { }

    [SetsRequiredMembers]
    public Lora(string path)
    {
        this.Path = path;
    }
}