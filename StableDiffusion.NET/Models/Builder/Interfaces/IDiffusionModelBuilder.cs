namespace StableDiffusion.NET;

public interface IDiffusionModelBuilder
{
    IDiffusionModelParameter Parameter { get; }
}