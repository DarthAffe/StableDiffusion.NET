namespace StableDiffusion.NET;

public sealed class SampleParameter
{
    public GuidanceParameter Guidance { get; } = new();

    /// <summary>
    /// Denoiser sigma schedule (default: Default)
    /// </summary>
    public Scheduler Scheduler { get; set; } = Scheduler.Default;

    /// <summary>
    /// sampling method (default: Default)
    /// </summary>
    public Sampler SampleMethod { get; set; } = Sampler.Default;

    /// <summary>
    /// number of sample steps (default: 20)
    /// </summary>
    public int SampleSteps { get; set; } = 20;

    /// <summary>
    /// eta in DDIM, only for DDIM and TCD (default: 0)
    /// </summary>
    public float Eta { get; set; } = 0f;

    public int ShiftedTimestep { get; set; } = 0;

    public float[] CustomSigmas { get; set; } = [];

    internal SampleParameter() { }
}