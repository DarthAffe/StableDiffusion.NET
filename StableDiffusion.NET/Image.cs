using System;

namespace StableDiffusion.NET;

public sealed unsafe class Image : IDisposable
{
    #region Constants

    public const int BPP = 3;

    #endregion

    #region Properties & Fields

    private bool _disposed;

    private readonly byte* _imagePtr;

    public int Width { get; }
    public int Height { get; }

    public ReadOnlySpan<byte> Data
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            return new ReadOnlySpan<byte>(_imagePtr, Width * Height * BPP);
        }
    }

    #endregion

    #region Constructors

    internal Image(byte* ptr, int width, int height)
    {
        this._imagePtr = ptr;
        this.Width = width;
        this.Height = height;
    }

    ~Image() => Dispose();

    #endregion

    #region Methods

    public void Dispose()
    {
        if (_disposed) return;

        Native.stable_diffusion_free_buffer(_imagePtr);

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    #endregion
}