using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class ModelBuilder
{
    public static StableDiffusionModelBuilder StableDiffusion(string modelPath) => new(modelPath);
    public static FluxModelBuilder Flux(string diffusionModelPath, string clipLPath, string t5xxlPath) => new(diffusionModelPath, clipLPath, t5xxlPath);
    public static ESRGANModelBuilder ESRGAN(string modelPath) => new(modelPath);
}