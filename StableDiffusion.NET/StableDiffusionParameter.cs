namespace StableDiffusion.NET;

public sealed class StableDiffusionParameter
{
    #region Properties & Fields

    public string NegativePrompt { get; set; } = string.Empty;
    public float CfgScale { get; set; } = 7.5f;
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
    public Sampler SampleMethod { get; set; } = Sampler.Euler_A;
    public int SampleSteps { get; set; } = 25;
    public long Seed { get; set; } = -1;
    public float Strength { get; set; } = 0.7f;
    public int ClipSkip { get; set; } = -1;

    #endregion
}