using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class ESRGANModelBuilder : IQuantizedModelBuilder
{
    #region Properties & Fields

    public UpscaleModelParameter Parameter { get; }
    IQuantizedModelParameter IQuantizedModelBuilder.Parameter => Parameter;

    #endregion

    #region Constructors

    public ESRGANModelBuilder(string modelPath)
    {
        Parameter = new UpscaleModelParameter { ModelPath = modelPath };
    }

    #endregion

    #region Methods

    public UpscaleModel Build() => new(Parameter);

    #endregion
}