using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class StableDiffusionModelBuilder : IDiffusionModelBuilder, IQuantizedModelBuilder, IPhotomakerModelBuilder
{
    #region Properties & Fields

    public DiffusionModelParameter Parameter { get; }
    IDiffusionModelParameter IDiffusionModelBuilder.Parameter => Parameter;
    IQuantizedModelParameter IQuantizedModelBuilder.Parameter => Parameter;
    IPhotomakerModelParameter IPhotomakerModelBuilder.Parameter => Parameter;

    #endregion

    #region Constructors

    public StableDiffusionModelBuilder(string modelPath)
    {
        Parameter = new DiffusionModelParameter { DiffusionModelType = DiffusionModelType.StableDiffusion, ModelPath = modelPath };
    }

    #endregion

    #region Methods

    public DiffusionModel Build() => new(Parameter);

    #endregion
}