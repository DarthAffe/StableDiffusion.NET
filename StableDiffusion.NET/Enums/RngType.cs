namespace StableDiffusion.NET;

public enum RngType
{
    [NativeName("STD_DEFAULT_RNG")]
    Standard,

    [NativeName("CUDA_RNG")]
    Cuda
}