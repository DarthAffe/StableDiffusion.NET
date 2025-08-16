using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class StableDiffusion3_5ModelBuilder : IDiffusionModelBuilder
{
    #region Properties & Fields

    public DiffusionModelParameter Parameter { get; }
    IDiffusionModelParameter IDiffusionModelBuilder.Parameter => Parameter;
    IModelParameter IModelBuilder.Parameter => Parameter;


    #endregion

    #region Constructors

    public StableDiffusion3_5ModelBuilder(string modelPath, string clipLPath, string clipGPath, string t5xxlPath)
    {
        Parameter = new DiffusionModelParameter
        {
            ModelPath = modelPath,
            ClipLPath = clipLPath,
            ClipGPath = clipGPath,
            T5xxlPath = t5xxlPath,
        };
    }

    #endregion

    #region Methods

    public DiffusionModel Build() => new(Parameter);

    #endregion
}