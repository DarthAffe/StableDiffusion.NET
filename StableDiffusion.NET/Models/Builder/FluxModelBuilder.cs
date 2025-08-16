using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class FluxModelBuilder : IDiffusionModelBuilder
{
    #region Properties & Fields

    public DiffusionModelParameter Parameter { get; }
    IDiffusionModelParameter IDiffusionModelBuilder.Parameter => Parameter;
    IModelParameter IModelBuilder.Parameter => Parameter;

    #endregion

    #region Constructors

    public FluxModelBuilder(string diffusionModelPath, string clipLPath, string t5xxlPath, string vaePath)
    {
        Parameter = new DiffusionModelParameter { DiffusionModelPath = diffusionModelPath, ClipLPath = clipLPath, T5xxlPath = t5xxlPath, VaePath = vaePath };
    }

    #endregion

    #region Methods

    public DiffusionModel Build() => new(Parameter);

    #endregion
}
