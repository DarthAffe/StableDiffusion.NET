namespace StableDiffusion.NET;

public enum Quantization
{
    [NativeName("DEFAULT")]
    Default,

    [NativeName("F32")]
    F32,

    [NativeName("F16")]
    F16,

    [NativeName("Q4_0")]
    Q4_0,

    [NativeName("Q4_1")]
    Q4_1,

    [NativeName("Q5_0")]
    Q5_0,

    [NativeName("Q5_1")]
    Q5_1,

    [NativeName("Q8_0")]
    Q8_0
}