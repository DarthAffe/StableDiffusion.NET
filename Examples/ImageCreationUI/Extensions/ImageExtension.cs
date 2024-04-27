using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using StableDiffusion.NET.Helper.Images.Colors;
using StableDiffusion.NET.Helper.Images;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageCreationUI;

public static class ImageExtension
{
    public static Bitmap ToBitmap(this IImage image) => image.AsRefImage<ColorRGB>().ToBitmap();
    public static Bitmap ToBitmap(this Image<ColorRGB> image) => image.AsRefImage<ColorRGB>().ToBitmap();

    public static unsafe Bitmap ToBitmap(this RefImage<ColorRGB> image)
    {
        Bitmap output = new(image.Width, image.Height, PixelFormat.Format24bppRgb);
        Rectangle rect = new(0, 0, image.Width, image.Height);
        BitmapData bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);

        nint ptr = bmpData.Scan0;
        foreach (ReadOnlyRefEnumerable<ColorRGB> row in image.Rows)
        {
            Span<ColorBGR> target = new((void*)ptr, bmpData.Stride);
            for (int i = 0; i < row.Length; i++)
            {
                ColorRGB srcColor = row[i];
                target[i] = new ColorBGR(srcColor.B, srcColor.G, srcColor.R);
            }

            ptr += bmpData.Stride;
        }

        output.UnlockBits(bmpData);
        return output;
    }

    public static byte[] ToPng(this IImage image)
    {
        using Bitmap bitmap = image.ToBitmap();
        using MemoryStream ms = new();
        bitmap.Save(ms, ImageFormat.Png);

        return ms.ToArray();
    }

    public static unsafe Image<ColorRGB> ToImage(this Bitmap bitmap)
    {
        int width = bitmap.Width;
        int height = bitmap.Height;

        byte[] buffer = new byte[height * width * ColorRGB.ColorFormat.BytesPerPixel];
        Span<ColorRGB> colorBuffer = MemoryMarshal.Cast<byte, ColorRGB>(buffer);

        Rectangle rect = new(0, 0, bitmap.Width, bitmap.Height);
        BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

        nint ptr = bmpData.Scan0;
        for (int y = 0; y < height; y++)
        {
            Span<ColorBGR> source = new((void*)ptr, bmpData.Stride);
            Span<ColorRGB> target = colorBuffer.Slice(y * width, width);
            for (int x = 0; x < width; x++)
            {
                ColorBGR srcColor = source[x];
                target[x] = new ColorRGB(srcColor.R, srcColor.G, srcColor.B);
            }

            ptr += bmpData.Stride;
        }

        bitmap.UnlockBits(bmpData);

        return new Image<ColorRGB>(buffer, 0, 0, width, height, width);
    }
}