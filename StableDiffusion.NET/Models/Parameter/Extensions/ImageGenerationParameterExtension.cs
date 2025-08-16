using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class ImageGenerationParameterExtension
{
    public static ImageGenerationParameter WithPrompt(this ImageGenerationParameter parameter, string prompt)
    {
        parameter.Prompt = prompt;

        return parameter;
    }

    public static ImageGenerationParameter WithNegativePrompt(this ImageGenerationParameter parameter, string negativePrompt)
    {
        parameter.NegativePrompt = negativePrompt;

        return parameter;
    }

    public static ImageGenerationParameter WithClipSkip(this ImageGenerationParameter parameter, int clipSkip)
    {
        parameter.ClipSkip = clipSkip;

        return parameter;
    }

    public static ImageGenerationParameter WithCfg(this ImageGenerationParameter parameter, float cfg)
    {
        parameter.WithTxtCfg(cfg);
        parameter.WithImgCfg(cfg);

        return parameter;
    }

    public static ImageGenerationParameter WithTxtCfg(this ImageGenerationParameter parameter, float txtCfg)
    {
        parameter.Guidance.TxtCfg = txtCfg;

        return parameter;
    }

    public static ImageGenerationParameter WithImgCfg(this ImageGenerationParameter parameter, float imgCfg)
    {
        parameter.Guidance.ImgCfg = imgCfg;

        return parameter;
    }

    public static ImageGenerationParameter WithMinCfg(this ImageGenerationParameter parameter, float minCfg)
    {
        parameter.Guidance.MinCfg = minCfg;

        return parameter;
    }

    public static ImageGenerationParameter WithGuidance(this ImageGenerationParameter parameter, float guidance)
    {
        parameter.Guidance.DistilledGuidance = guidance;

        return parameter;
    }

    public static ImageGenerationParameter WithSlgScale(this ImageGenerationParameter parameter, float slgScale)
    {
        parameter.Guidance.Slg.Scale = slgScale;

        return parameter;
    }

    public static ImageGenerationParameter WithSkipLayers(this ImageGenerationParameter parameter, int[] layers)
    {
        parameter.Guidance.Slg.Layers = layers;

        return parameter;
    }

    public static ImageGenerationParameter WithSkipLayerStart(this ImageGenerationParameter parameter, float skipLayerStart)
    {
        parameter.Guidance.Slg.SkipLayerStart = skipLayerStart;

        return parameter;
    }

    public static ImageGenerationParameter WithSkipLayerEnd(this ImageGenerationParameter parameter, float skipLayerEnd)
    {
        parameter.Guidance.Slg.SkipLayerEnd = skipLayerEnd;

        return parameter;
    }

    public static ImageGenerationParameter WithInitImage(this ImageGenerationParameter parameter, IImage image)
    {
        parameter.InitImage = image;

        return parameter;
    }

    public static ImageGenerationParameter WithRefImages(this ImageGenerationParameter parameter, params IImage[] images)
    {
        parameter.RefImages = images;

        return parameter;
    }

    public static ImageGenerationParameter WithMaskImage(this ImageGenerationParameter parameter, IImage image)
    {
        parameter.MaskImage = image;

        return parameter;
    }

    public static ImageGenerationParameter WithSize(this ImageGenerationParameter parameter, int? width = null, int? height = null)
    {
        if (width != null)
            parameter.Width = width.Value;

        if (height != null)
            parameter.Height = height.Value;

        return parameter;
    }

    public static ImageGenerationParameter WithSampler(this ImageGenerationParameter parameter, Sampler sampler)
    {
        parameter.SampleMethod = sampler;

        return parameter;
    }

    public static ImageGenerationParameter WithSteps(this ImageGenerationParameter parameter, int steps)
    {
        parameter.SampleSteps = steps;

        return parameter;
    }

    public static ImageGenerationParameter WithEta(this ImageGenerationParameter parameter, float eta)
    {
        parameter.Eta = eta;

        return parameter;
    }

    public static ImageGenerationParameter WithStrength(this ImageGenerationParameter parameter, float strength)
    {
        parameter.Strength = strength;

        return parameter;
    }

    public static ImageGenerationParameter WithSeed(this ImageGenerationParameter parameter, long seed)
    {
        parameter.Seed = seed;

        return parameter;
    }

    public static ImageGenerationParameter WithControlNet(this ImageGenerationParameter parameter, IImage image, float? strength = null)
    {
        parameter.ControlNet.Image = image;

        if (strength != null)
            parameter.ControlNet.Strength = strength.Value;

        return parameter;
    }

    public static ImageGenerationParameter WithPhotomaker(this ImageGenerationParameter parameter, string inputIdImageDirectory, float? styleStrength = null, bool? normalizeInput = null)
    {
        parameter.PhotoMaker.InputIdImageDirectory = inputIdImageDirectory;

        if (styleStrength != null)
            parameter.PhotoMaker.StyleStrength = styleStrength.Value;

        if (normalizeInput != null)
            parameter.PhotoMaker.NormalizeInput = normalizeInput.Value;

        return parameter;
    }

    #region Defaults

    public static ImageGenerationParameter WithSd1Defaults(this ImageGenerationParameter parameter)
        => parameter.WithSize(512, 512)
                    .WithCfg(7.5f)
                    .WithGuidance(1f)
                    .WithSteps(25)
                    .WithSampler(Sampler.Euler_A);

    public static ImageGenerationParameter WithSDXLDefaults(this ImageGenerationParameter parameter)
        => parameter.WithSize(1024, 1024)
                    .WithCfg(7f)
                    .WithGuidance(1f)
                    .WithSteps(30)
                    .WithSampler(Sampler.Euler_A);

    public static ImageGenerationParameter WithSD3_5Defaults(this ImageGenerationParameter parameter)
        => parameter.WithSize(1024, 1024)
                    .WithCfg(4.5f)
                    .WithGuidance(1f)
                    .WithSteps(20)
                    .WithSampler(Sampler.Euler);

    public static ImageGenerationParameter WithFluxDefaults(this ImageGenerationParameter parameter)
        => parameter.WithSize(1024, 1024)
                    .WithCfg(1f)
                    .WithGuidance(3.5f)
                    .WithSteps(20)
                    .WithSampler(Sampler.Euler);

    #endregion
}