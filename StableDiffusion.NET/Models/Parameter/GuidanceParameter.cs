namespace StableDiffusion.NET;

public sealed class GuidanceParameter
{
    public float TxtCfg { get; set; } = 7.0f;
    public float ImgCfg { get; set; } = 7.0f;
    public float MinCfg { get; set; } = 1.0f;
    public float DistilledGuidance { get; set; } = 3.5f;

    public SlgParameter Slg { get; } = new();

    internal GuidanceParameter() { }
}