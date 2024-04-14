using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public class UpscalerModelParameter
{
    #region Properties & Fields

    public int ThreadCount { get; set; } = 8;
    public string ESRGANPath { get; set; } = string.Empty;

    //TODO DarthAffe 01.01.2024: K-Quants doesn't seem to work so far
    public Quantization Quantization { get; set; } = Quantization.F16;

    #endregion
}
