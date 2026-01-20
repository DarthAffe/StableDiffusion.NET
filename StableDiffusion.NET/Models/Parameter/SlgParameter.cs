namespace StableDiffusion.NET;

public sealed class SlgParameter
{
    /// <summary>
    /// Layers to skip for SLG steps
    /// </summary>
    public int[] Layers { get; set; } = [];

    /// <summary>
    /// SLG enabling point: (default: 0.01)
    /// </summary>
    public float SkipLayerStart { get; set; } = 0.01f;

    /// <summary>
    /// SLG disabling point: (default: 0.2)
    /// </summary>
    public float SkipLayerEnd { get; set; } = 0.2f;

    /// <summary>
    /// skip layer guidance (SLG) scale, only for DiT models: (default: 0)
    /// 0 means disabled, a value of 2.5 is nice for sd3.5 medium
    /// </summary>
    public float Scale { get; set; } = 0f;

    internal SlgParameter() { }
}