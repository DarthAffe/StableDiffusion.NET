using System;

namespace StableDiffusion.NET;

public class LibraryPathCreatingEventArgs(string path) : EventArgs
{
    public string Path { get; set; } = path;
}