using System.Drawing;
using System.Drawing.Imaging;
using StableDiffusion.NET.Helper.Images.Colors;
using StableDiffusion.NET.Helper.Images;
using System.IO;

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
}