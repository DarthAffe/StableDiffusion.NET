namespace StableDiffusion.NET;

public enum Sampler
{
    Default,
    Euler,
    Heun,
    DPM2,
    DPMPP2SA,
    DPMPP2M,
    DPMPP2Mv2,
    IPNDM,
    IPNDM_V,
    LCM,
    DDIM_Trailing,
    TCD,
    Euler_A,
}