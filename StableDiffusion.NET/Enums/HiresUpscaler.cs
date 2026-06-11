namespace StableDiffusion.NET;

public enum HiresUpscaler
{
    None,
    Latent,
    LatentNearest,
    LatentNearestExact,
    LatentAntialiased,
    LatentBicubic,
    LatentBicubicAntialiased,
    Lanczos,
    Nearest,
    Model,
}