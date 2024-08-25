namespace StableDiffusion.NET;

public interface IQuantizedModelParameter
{
    int ThreadCount { get; set; }

    Quantization Quantization { get; set; }
}