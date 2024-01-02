using System;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

public sealed unsafe class StableDiffusionImage : IDisposable
{
    #region Properties & Fields

    private bool _disposed;

    internal readonly Native.sd_image_t* Image;

    public int Width { get; }
    public int Height { get; }
    public int Bpp { get; }
    public int Stride { get; }

    public ReadOnlySpan<byte> Data
    {
        get
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            return new ReadOnlySpan<byte>(Image->data, Width * Height * Bpp);
        }
    }

    #endregion

    #region Constructors

    internal unsafe StableDiffusionImage(Native.sd_image_t* image)
    {
        this.Image = image;

        Width = (int)image->width;
        Height = (int)image->height;
        Bpp = (int)image->channel;
        Stride = Width * Bpp;
    }

    ~StableDiffusionImage() => Dispose();

    #endregion

    #region Methods

    public void Dispose()
    {
        if (_disposed) return;

        Marshal.FreeHGlobal((nint)Image->data);
        Marshal.FreeHGlobal((nint)Image);
        
        GC.SuppressFinalize(this);
        _disposed = true;
    }

    #endregion
}