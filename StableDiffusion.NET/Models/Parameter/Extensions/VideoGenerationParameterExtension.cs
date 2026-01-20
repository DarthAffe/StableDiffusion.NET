using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static class VideoGenerationParameterExtension
{
    public static VideoGenerationParameter WithPrompt(this VideoGenerationParameter parameter, string prompt)
    {
        parameter.Prompt = prompt;

        return parameter;
    }

    public static VideoGenerationParameter WithNegativePrompt(this VideoGenerationParameter parameter, string negativePrompt)
    {
        parameter.NegativePrompt = negativePrompt;

        return parameter;
    }

    public static VideoGenerationParameter WithClipSkip(this VideoGenerationParameter parameter, int clipSkip)
    {
        parameter.ClipSkip = clipSkip;

        return parameter;
    }

    public static VideoGenerationParameter WithSize(this VideoGenerationParameter parameter, int? width = null, int? height = null)
    {
        if (width != null)
            parameter.Width = width.Value;

        if (height != null)
            parameter.Height = height.Value;

        return parameter;
    }

    public static VideoGenerationParameter WithInitImage(this VideoGenerationParameter parameter, IImage image)
    {
        parameter.InitImage = image;

        return parameter;
    }

    public static VideoGenerationParameter WithEndImage(this VideoGenerationParameter parameter, IImage image)
    {
        parameter.EndImage = image;

        return parameter;
    }

    public static VideoGenerationParameter WithControlFrames(this VideoGenerationParameter parameter, params IImage[] images)
    {
        parameter.ControlFrames = images;

        return parameter;
    }

    #region SampleParameter

    #region Guidance

    public static VideoGenerationParameter WithCfg(this VideoGenerationParameter parameter, float cfg)
    {
        parameter.WithTxtCfg(cfg);
        parameter.WithImgCfg(cfg);

        return parameter;
    }

    public static VideoGenerationParameter WithTxtCfg(this VideoGenerationParameter parameter, float txtCfg)
    {
        parameter.SampleParameter.Guidance.TxtCfg = txtCfg;

        return parameter;
    }

    public static VideoGenerationParameter WithImgCfg(this VideoGenerationParameter parameter, float imgCfg)
    {
        parameter.SampleParameter.Guidance.ImgCfg = imgCfg;

        return parameter;
    }

    public static VideoGenerationParameter WithGuidance(this VideoGenerationParameter parameter, float guidance)
    {
        parameter.SampleParameter.Guidance.DistilledGuidance = guidance;

        return parameter;
    }

    #region Slg

    public static VideoGenerationParameter WithSkipLayers(this VideoGenerationParameter parameter, int[] layers)
    {
        parameter.SampleParameter.Guidance.Slg.Layers = layers;

        return parameter;
    }

    public static VideoGenerationParameter WithSkipLayerStart(this VideoGenerationParameter parameter, float skipLayerStart)
    {
        parameter.SampleParameter.Guidance.Slg.SkipLayerStart = skipLayerStart;

        return parameter;
    }

    public static VideoGenerationParameter WithSkipLayerEnd(this VideoGenerationParameter parameter, float skipLayerEnd)
    {
        parameter.SampleParameter.Guidance.Slg.SkipLayerEnd = skipLayerEnd;

        return parameter;
    }

    public static VideoGenerationParameter WithSlgScale(this VideoGenerationParameter parameter, float slgScale)
    {
        parameter.SampleParameter.Guidance.Slg.Scale = slgScale;

        return parameter;
    }

    #endregion

    #endregion

    public static VideoGenerationParameter WithScheduler(this VideoGenerationParameter parameter, Scheduler scheduler)
    {
        parameter.SampleParameter.Scheduler = scheduler;

        return parameter;
    }

    public static VideoGenerationParameter WithSampler(this VideoGenerationParameter parameter, Sampler sampler)
    {
        parameter.SampleParameter.SampleMethod = sampler;

        return parameter;
    }

    public static VideoGenerationParameter WithSteps(this VideoGenerationParameter parameter, int steps)
    {
        parameter.SampleParameter.SampleSteps = steps;

        return parameter;
    }

    public static VideoGenerationParameter WithEta(this VideoGenerationParameter parameter, float eta)
    {
        parameter.SampleParameter.Eta = eta;

        return parameter;
    }

    public static VideoGenerationParameter WithShiftedTimestep(this VideoGenerationParameter parameter, int shiftedTimestep)
    {
        parameter.SampleParameter.ShiftedTimestep = shiftedTimestep;

        return parameter;
    }

    #endregion

    #region HighNoiseSampleParameter

    #region Guidance

    public static VideoGenerationParameter WithHighNoiseCfg(this VideoGenerationParameter parameter, float cfg)
    {
        parameter.WithTxtCfg(cfg);
        parameter.WithImgCfg(cfg);

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseTxtCfg(this VideoGenerationParameter parameter, float txtCfg)
    {
        parameter.HighNoiseSampleParameter.Guidance.TxtCfg = txtCfg;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseImgCfg(this VideoGenerationParameter parameter, float imgCfg)
    {
        parameter.HighNoiseSampleParameter.Guidance.ImgCfg = imgCfg;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseGuidance(this VideoGenerationParameter parameter, float guidance)
    {
        parameter.HighNoiseSampleParameter.Guidance.DistilledGuidance = guidance;

        return parameter;
    }

    #region Slg

    public static VideoGenerationParameter WithHighNoiseSkipLayers(this VideoGenerationParameter parameter, int[] layers)
    {
        parameter.HighNoiseSampleParameter.Guidance.Slg.Layers = layers;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseSkipLayerStart(this VideoGenerationParameter parameter, float skipLayerStart)
    {
        parameter.HighNoiseSampleParameter.Guidance.Slg.SkipLayerStart = skipLayerStart;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseSkipLayerEnd(this VideoGenerationParameter parameter, float skipLayerEnd)
    {
        parameter.HighNoiseSampleParameter.Guidance.Slg.SkipLayerEnd = skipLayerEnd;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseSlgScale(this VideoGenerationParameter parameter, float slgScale)
    {
        parameter.HighNoiseSampleParameter.Guidance.Slg.Scale = slgScale;

        return parameter;
    }

    #endregion

    #endregion

    public static VideoGenerationParameter WithHighNoiseScheduler(this VideoGenerationParameter parameter, Scheduler scheduler)
    {
        parameter.HighNoiseSampleParameter.Scheduler = scheduler;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseSampler(this VideoGenerationParameter parameter, Sampler sampler)
    {
        parameter.HighNoiseSampleParameter.SampleMethod = sampler;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseSteps(this VideoGenerationParameter parameter, int steps)
    {
        parameter.HighNoiseSampleParameter.SampleSteps = steps;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseEta(this VideoGenerationParameter parameter, float eta)
    {
        parameter.HighNoiseSampleParameter.Eta = eta;

        return parameter;
    }

    public static VideoGenerationParameter WithHighNoiseShiftedTimestep(this VideoGenerationParameter parameter, int shiftedTimestep)
    {
        parameter.HighNoiseSampleParameter.ShiftedTimestep = shiftedTimestep;

        return parameter;
    }

    #endregion

    public static VideoGenerationParameter WithMoeBoundry(this VideoGenerationParameter parameter, float moeBoundry)
    {
        parameter.MoeBoundry = moeBoundry;

        return parameter;
    }

    public static VideoGenerationParameter WithStrength(this VideoGenerationParameter parameter, float strength)
    {
        parameter.Strength = strength;

        return parameter;
    }

    public static VideoGenerationParameter WithSeed(this VideoGenerationParameter parameter, long seed)
    {
        parameter.Seed = seed;

        return parameter;
    }

    public static VideoGenerationParameter WithFrameCount(this VideoGenerationParameter parameter, int frameCount)
    {
        parameter.FrameCount = frameCount;

        return parameter;
    }

    public static VideoGenerationParameter WithVaceStrength(this VideoGenerationParameter parameter, float vaceStrength)
    {
        parameter.VaceStrength = vaceStrength;

        return parameter;
    }
}