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

    public static ImageGenerationParameter WithRefIndexIncrease(this ImageGenerationParameter parameter, bool refIndexIncrease = true)
    {
        parameter.IncreaseRefIndex = refIndexIncrease;

        return parameter;
    }

    public static ImageGenerationParameter WithRefImageAutoResize(this ImageGenerationParameter parameter, bool refImageAutoResize = true)
    {
        parameter.AutoResizeRefImage = refImageAutoResize;

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

    #region SampleParameter

    #region Guidance

    public static ImageGenerationParameter WithCfg(this ImageGenerationParameter parameter, float cfg)
    {
        parameter.WithTxtCfg(cfg);
        parameter.WithImgCfg(cfg);

        return parameter;
    }

    public static ImageGenerationParameter WithTxtCfg(this ImageGenerationParameter parameter, float txtCfg)
    {
        parameter.SampleParameter.Guidance.TxtCfg = txtCfg;

        return parameter;
    }

    public static ImageGenerationParameter WithImgCfg(this ImageGenerationParameter parameter, float imgCfg)
    {
        parameter.SampleParameter.Guidance.ImgCfg = imgCfg;

        return parameter;
    }

    public static ImageGenerationParameter WithGuidance(this ImageGenerationParameter parameter, float guidance)
    {
        parameter.SampleParameter.Guidance.DistilledGuidance = guidance;

        return parameter;
    }

    #region Slg

    public static ImageGenerationParameter WithSkipLayers(this ImageGenerationParameter parameter, int[] layers)
    {
        parameter.SampleParameter.Guidance.Slg.Layers = layers;

        return parameter;
    }

    public static ImageGenerationParameter WithSkipLayerStart(this ImageGenerationParameter parameter, float skipLayerStart)
    {
        parameter.SampleParameter.Guidance.Slg.SkipLayerStart = skipLayerStart;

        return parameter;
    }

    public static ImageGenerationParameter WithSkipLayerEnd(this ImageGenerationParameter parameter, float skipLayerEnd)
    {
        parameter.SampleParameter.Guidance.Slg.SkipLayerEnd = skipLayerEnd;

        return parameter;
    }

    public static ImageGenerationParameter WithSlgScale(this ImageGenerationParameter parameter, float slgScale)
    {
        parameter.SampleParameter.Guidance.Slg.Scale = slgScale;

        return parameter;
    }

    #endregion

    #endregion

    public static ImageGenerationParameter WithScheduler(this ImageGenerationParameter parameter, Scheduler scheduler)
    {
        parameter.SampleParameter.Scheduler = scheduler;

        return parameter;
    }

    public static ImageGenerationParameter WithSampler(this ImageGenerationParameter parameter, Sampler sampler)
    {
        parameter.SampleParameter.SampleMethod = sampler;

        return parameter;
    }

    public static ImageGenerationParameter WithSteps(this ImageGenerationParameter parameter, int steps)
    {
        parameter.SampleParameter.SampleSteps = steps;

        return parameter;
    }

    public static ImageGenerationParameter WithEta(this ImageGenerationParameter parameter, float eta)
    {
        parameter.SampleParameter.Eta = eta;

        return parameter;
    }

    public static ImageGenerationParameter WithShiftedTimestep(this ImageGenerationParameter parameter, int shiftedTimestep)
    {
        parameter.SampleParameter.ShiftedTimestep = shiftedTimestep;

        return parameter;
    }

    #endregion

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
        //todo
        //parameter.PhotoMaker.InputIdImageDirectory = inputIdImageDirectory;

        //if (styleStrength != null)
        //    parameter.PhotoMaker.StyleStrength = styleStrength.Value;

        //if (normalizeInput != null)
        //    parameter.PhotoMaker.NormalizeInput = normalizeInput.Value;

        return parameter;
    }

    #region VaeTiling

    public static ImageGenerationParameter WithVaeTiling(this ImageGenerationParameter parameter, bool tiling = true)
    {
        parameter.VaeTiling.IsEnabled = tiling;

        return parameter;
    }

    public static ImageGenerationParameter WithVaeTileSizeX(this ImageGenerationParameter parameter, int tileSizeX)
    {
        parameter.VaeTiling.TileSizeX = tileSizeX;

        return parameter;
    }

    public static ImageGenerationParameter WithVaeTileSizeY(this ImageGenerationParameter parameter, int tileSizeY)
    {
        parameter.VaeTiling.TileSizeY = tileSizeY;

        return parameter;
    }

    public static ImageGenerationParameter WithVaeTargetOverlap(this ImageGenerationParameter parameter, float targetOverlap)
    {
        parameter.VaeTiling.TargetOverlap = targetOverlap;

        return parameter;
    }

    public static ImageGenerationParameter WithVaeRelSizeX(this ImageGenerationParameter parameter, float relSizeX)
    {
        parameter.VaeTiling.RelSizeX = relSizeX;

        return parameter;
    }

    public static ImageGenerationParameter WithVaeRelSizeY(this ImageGenerationParameter parameter, float relSizeY)
    {
        parameter.VaeTiling.RelSizeY = relSizeY;

        return parameter;
    }

    #endregion

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