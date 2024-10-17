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

    public static unsafe void Dispose(Native.sd_image_t image)
    {
        Marshal.FreeHGlobal((nint)image.data);
    }

    public static unsafe Native.sd_image_t ToSdImage(this IImage<ColorRGB> image, byte* pinnedReference)
        => new()
        {
            width = (uint)image.Width,
            height = (uint)image.Height,
            channel = (uint)image.ColorFormat.BytesPerPixel,
            data = pinnedReference
        };
}