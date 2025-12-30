using System.Diagnostics.CodeAnalysis;

namespace StableDiffusion.NET;

public sealed class Embedding
{
    public required string Name { get; init; }
    public required string Path { get; init; }

    public Embedding() { }

    [SetsRequiredMembers]
    public Embedding(string name, string path)
    {
        this.Name = name;
        this.Path = path;
    }
}