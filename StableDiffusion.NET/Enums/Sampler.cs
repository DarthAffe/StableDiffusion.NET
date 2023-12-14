namespace StableDiffusion.NET;

public enum Sampler
{
    [NativeName("EULER_A")]
    Euler_A,

    [NativeName("EULER")]
    Euler,

    [NativeName("HEUN")]
    Heun,

    [NativeName("DPM2")]
    DPM2,

    [NativeName("DPMPP2S_A")]
    DPMPP2SA,

    [NativeName("DPMPP2M")]
    DPMPP2M,

    [NativeName("DPMPP2Mv2")]
    DPMPP2Mv2,

    [NativeName("LCM")]
    LCM,

    [NativeName("N_SAMPLE_METHODS")]
    N_Sample_Methods
}