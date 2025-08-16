using HPPH;
using System;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

internal static class ImageHelper
{
    public static unsafe Image<ColorRGB> ToImage(this Native.Types.sd_image_t sdImage)
    {
        int width = (int)sdImage.width;
        int height = (int)sdImage.height;
        int bpp = (int)sdImage.channel;

        switch (bpp)
        {
            case 3:
                return Image<ColorRGB>.Create(new ReadOnlySpan<byte>(sdImage.data, width * height * bpp), width, height, width * bpp);

            case 1:
                {
                    ColorRGB[] pixels = new ColorRGB[width * height];
                    Span<byte> sdData = new(sdImage.data, pixels.Length);

                    for (int i = 0; i < pixels.Length; i++)
                    {
                        byte c = sdData[i];
                        pixels[i] = new ColorRGB(c, c, c);
                    }

                    Image<ColorRGB> image = Image<ColorRGB>.Create(pixels, width, height);

                    return image;
                }

            default:
                throw new ArgumentOutOfRangeException($"Image-BPP of {bpp} is not supported");
        }
    }

    public static unsafe Native.Types.sd_image_t ToSdImage(this IImage image, bool monochrome = false)
    {
        if (monochrome)
        {
            int sizeInBytes = image.Width * image.Height;

            byte* dataPtr = (byte*)NativeMemory.Alloc((nuint)sizeInBytes);
            Span<byte> data = new(dataPtr, sizeInBytes);

            // DarthAffe 16.08.2025: HPPH does currently not support monochrome images, that's why we need to convert it here. We're going for the simple conversion as the source image is supposed to be monochrome anyway.
            for (int y = 0; y < image.Height; y++)
                for (int x = 0; x < image.Width; x++)
                {
                    IColor color = image[x, y];
                    data[(image.Width * y) + x] = (byte)Math.Round((color.R + color.G + color.B) / 3.0);
                }

            return new Native.Types.sd_image_t
            {
                width = (uint)image.Width,
                height = (uint)image.Height,
                channel = 1,
                data = dataPtr
            };
        }
        else
        {
            IImage<ColorRGB> img = image as IImage<ColorRGB> ?? image.ConvertTo<ColorRGB>();

            int sizeInBytes = img.SizeInBytes;

            byte* dataPtr = (byte*)NativeMemory.Alloc((nuint)sizeInBytes);
            img.CopyTo(new Span<byte>(dataPtr, sizeInBytes));

            return new Native.Types.sd_image_t
            {
                width = (uint)img.Width,
                height = (uint)img.Height,
                channel = (uint)img.ColorFormat.BytesPerPixel,
                data = dataPtr
            };
        }
    }

    public static unsafe Native.Types.sd_image_t* ToSdImagePtr(this IImage image, bool monochrome = false)
    {
        Native.Types.sd_image_t* imagePtr = (Native.Types.sd_image_t*)NativeMemory.Alloc((nuint)Marshal.SizeOf<Native.Types.sd_image_t>());
        imagePtr[0] = image.ToSdImage(monochrome);

        return imagePtr;
    }

    public static unsafe void Free(this Native.Types.sd_image_t sdImage)
    {
        if (sdImage.data == null) return;

        NativeMemory.Free(sdImage.data);
    }

    public static unsafe Image<ColorRGB>[] ToImageArray(Native.Types.sd_image_t* sdImage, int count)
    {
        if (sdImage == null) return [];

        Image<ColorRGB>[] images = new Image<ColorRGB>[count];

        for (int i = 0; i < images.Length; i++)
            images[i] = GetImage(sdImage, i);

        return images;
    }

    internal static unsafe IImage[] ToImageArrayIFace(Native.Types.sd_image_t* sdImage, int count)
    {
        IImage[] images = new IImage[count];

        for (int i = 0; i < images.Length; i++)
            images[i] = GetImage(sdImage, i);

        return images;
    }

    public static unsafe Image<ColorRGB> GetImage(Native.Types.sd_image_t* sdImage, int index) => sdImage[index].ToImage();

    public static unsafe Native.Types.sd_image_t* ToSdImage(this IImage[] images, bool monochrome = false)
    {
        int count = images.Length;

        Native.Types.sd_image_t* imagePtr = (Native.Types.sd_image_t*)NativeMemory.Alloc((nuint)(count * Marshal.SizeOf<Native.Types.sd_image_t>()));

        for (int i = 0; i < count; i++)
            imagePtr[i] = images[i].ToSdImage(monochrome);

        return imagePtr;
    }

    public static unsafe void Free(Native.Types.sd_image_t* sdImage, int count)
    {
        if (sdImage == null) return;

        for (int i = 0; i < count; i++)
            Free(sdImage[i]);

        NativeMemory.Free(sdImage);
    }
}