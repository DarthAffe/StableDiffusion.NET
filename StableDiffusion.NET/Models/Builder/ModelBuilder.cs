﻿using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class ModelBuilder
{
    public static StableDiffusionModelBuilder StableDiffusion(string modelPath) => new(modelPath);
    public static FluxModelBuilder Flux(string diffusionModelPath, string clipLPath, string t5xxlPath, string vaePath) => new(diffusionModelPath, clipLPath, t5xxlPath, vaePath);
    public static ESRGANModelBuilder ESRGAN(string modelPath) => new(modelPath);
}