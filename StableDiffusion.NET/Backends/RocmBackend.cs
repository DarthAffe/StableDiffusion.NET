namespace StableDiffusion.NET;

public class RocmBackend : IBackend
{
    #region Properties & Fields

    public bool IsEnabled { get; set; } = true;

    public int Priority => 10;

    public bool IsAvailable => false;

    public string PathPart { get; } = string.Empty;

    #endregion

    #region Constructors

    internal RocmBackend() { }

    #endregion
}