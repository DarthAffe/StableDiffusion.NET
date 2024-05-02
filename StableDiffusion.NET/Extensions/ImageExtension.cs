using StableDiffusion.NET.Helper.Images;
using StableDiffusion.NET.Helper.Images.Colors;

namespace StableDiffusion.NET;

public static class ImageExtension
{
    public static Image<ColorRGB> ToImage(this StableDiffusionImage image) => new(image.Data.ToArray(), 0, 0, image.Width, image.Height, image.Width);
}