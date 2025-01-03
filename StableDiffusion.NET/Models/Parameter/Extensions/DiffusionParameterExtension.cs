using HPPH;

namespace StableDiffusion.NET;

public static class DiffusionParameterExtension
{
    public static DiffusionParameter WithSize(this DiffusionParameter parameter, int? width = null, int? height = null)
    {
        if (width != null)
            parameter.Width = width.Value;

        if (height != null)
            parameter.Height = height.Value;

        return parameter;
    }

    public static DiffusionParameter WithSampler(this DiffusionParameter parameter, Sampler sampler)
    {
        parameter.SampleMethod = sampler;

        return parameter;
    }

    public static DiffusionParameter WithSteps(this DiffusionParameter parameter, int steps)
    {
        parameter.SampleSteps = steps;

        return parameter;
    }

    public static DiffusionParameter WithSeed(this DiffusionParameter parameter, long seed)
    {
        parameter.Seed = seed;

        return parameter;
    }

    public static DiffusionParameter WithClipSkip(this DiffusionParameter parameter, int clipSkip)
    {
        parameter.ClipSkip = clipSkip;

        return parameter;
    }

    public static DiffusionParameter WithCfg(this DiffusionParameter parameter, float cfg)
    {
        parameter.CfgScale = cfg;

        return parameter;
    }

    public static DiffusionParameter WithGuidance(this DiffusionParameter parameter, float guidance)
    {
        parameter.Guidance = guidance;

        return parameter;
    }

    public static DiffusionParameter WithNegativePrompt(this DiffusionParameter parameter, string negativePrompt)
    {
        parameter.NegativePrompt = negativePrompt;

        return parameter;
    }

    public static DiffusionParameter WithSlgScale(this DiffusionParameter parameter, float slgScale)
    {
        parameter.SlgScale = slgScale;

        return parameter;
    }

    public static DiffusionParameter WithSkipLayers(this DiffusionParameter parameter, int[] layers)
    {
        parameter.SkipLayers = layers;

        return parameter;
    }

    public static DiffusionParameter WithSkipLayerStart(this DiffusionParameter parameter, float skipLayerStart)
    {
        parameter.SkipLayerStart = skipLayerStart;

        return parameter;
    }

    public static DiffusionParameter WithSkipLayerEnd(this DiffusionParameter parameter, float skipLayerEnd)
    {
        parameter.SkipLayerEnd = skipLayerEnd;

        return parameter;
    }

    public static DiffusionParameter WithControlNet(this DiffusionParameter parameter, IImage image, float? strength = null)
    {
        parameter.ControlNet.Image = image;

        if (strength != null)
            parameter.ControlNet.Strength = strength.Value;

        return parameter;
    }

    public static DiffusionParameter WithCannyPreprocessing(this DiffusionParameter parameter,
                                                            float? cannyHighThreshold = null, float? cannyLowThreshold = null,
                                                            float? cannyWeak = null, float? cannyStrong = null,
                                                            bool? cannyInverse = null)
    {
        parameter.ControlNet.CannyPreprocess = true;

        if (cannyHighThreshold != null)
            parameter.ControlNet.CannyHighThreshold = cannyHighThreshold.Value;

        if (cannyLowThreshold != null)
            parameter.ControlNet.CannyLowThreshold = cannyLowThreshold.Value;

        if (cannyWeak != null)
            parameter.ControlNet.CannyWeak = cannyWeak.Value;

        if (cannyStrong != null)
            parameter.ControlNet.CannyStrong = cannyStrong.Value;

        if (cannyInverse != null)
            parameter.ControlNet.CannyInverse = cannyInverse.Value;

        return parameter;
    }

    public static DiffusionParameter WithPhotomaker(this DiffusionParameter parameter, string inputIdImageDirectory, float? styleRatio = null, bool? normalizeInput = null)
    {
        parameter.PhotoMaker.InputIdImageDirectory = inputIdImageDirectory;

        if (styleRatio != null)
            parameter.PhotoMaker.StyleRatio = styleRatio.Value;

        if (normalizeInput != null)
            parameter.PhotoMaker.NormalizeInput = normalizeInput.Value;

        return parameter;
    }
}