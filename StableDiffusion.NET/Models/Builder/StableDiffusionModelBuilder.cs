using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class StableDiffusionModelBuilder : IDiffusionModelBuilder
{
    #region Properties & Fields

    public DiffusionModelParameter Parameter { get; }
    IDiffusionModelParameter IDiffusionModelBuilder.Parameter => Parameter;
    IModelParameter IModelBuilder.Parameter => Parameter;


    #endregion

    #region Constructors

    public StableDiffusionModelBuilder(string modelPath)
    {
        Parameter = new DiffusionModelParameter { ModelPath = modelPath };
    }

    #endregion

    #region Methods

    public DiffusionModel Build() => new(Parameter);

    #endregion
}