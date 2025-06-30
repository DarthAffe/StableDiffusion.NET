using HPPH;
using System;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

internal static class ImageHelper
{
    public static unsafe Image<ColorRGB> ToImage(Native.sd_image_t* sdImage)
    {
        Image<ColorRGB> image = ToImage(*sdImage);

        Marshal.FreeHGlobal((nint)sdImage);

        return image;
    }

    public static unsafe Image<ColorRGB> ToImage(Native.sd_image_t sdImage)
    {
        int width = (int)sdImage.width;
        int height = (int)sdImage.height;
        int bpp = (int)sdImage.channel;

        Image<ColorRGB> image = Image<ColorRGB>.Create(new ReadOnlySpan<byte>(sdImage.data, width * height * bpp), width, height, width * bpp);

        Dispose(sdImage);

        return image;
    }

    public static unsafe void Dispose(Native.sd_image_t image) => Marshal.FreeHGlobal((nint)image.data);

    public static unsafe Native.sd_image_t ToSdImage(this IImage image, out nint dataPtr)
    {
        int sizeInBytes = image.SizeInBytes;

        dataPtr = Marshal.AllocHGlobal(sizeInBytes);
        image.CopyTo(new Span<byte>((void*)dataPtr, sizeInBytes));

        return image.ToSdImage((byte*)dataPtr);
    }

    public static unsafe Native.sd_image_t ToSdImage(this IImage image, byte* pinnedReference)
        => new()
        {
            width = (uint)image.Width,
            height = (uint)image.Height,
            channel = (uint)image.ColorFormat.BytesPerPixel,
            data = pinnedReference
        };

    public static unsafe Native.sd_image_t* ToSdImagePtr(this IImage image, out nint dataPtr)
    {
        int sizeInBytes = image.SizeInBytes;

        dataPtr = Marshal.AllocHGlobal(sizeInBytes);
        image.CopyTo(new Span<byte>((void*)dataPtr, sizeInBytes));

        return image.ToSdImagePtr((byte*)dataPtr);
    }

    public static unsafe Native.sd_image_t* ToSdImagePtr(this IImage image, byte* pinnedReference)
    {
        Native.sd_image_t* nativeImage = (Native.sd_image_t*)Marshal.AllocHGlobal(sizeof(Native.sd_image_t));

        nativeImage->width = (uint)image.Width;
        nativeImage->height = (uint)image.Height;
        nativeImage->channel = (uint)image.ColorFormat.BytesPerPixel;
        nativeImage->data = pinnedReference;

        return nativeImage;
    }
}