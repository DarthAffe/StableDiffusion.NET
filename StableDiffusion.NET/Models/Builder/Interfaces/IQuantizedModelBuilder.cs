namespace StableDiffusion.NET;

public interface IQuantizedModelBuilder
{
    IQuantizedModelParameter Parameter { get; }
}