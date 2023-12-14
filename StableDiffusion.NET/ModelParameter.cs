namespace StableDiffusion.NET;

public class ModelParameter
{
    #region Properties & Fields

    public int ThreadCount { get; set; } = 8;
    public bool VaeDecodeOnly { get; set; } = false;
    public string TaesdPath { get; set; } = string.Empty;
    public string LoraModelDir { get; set; } = string.Empty;
    public RngType RngType { get; set; } = RngType.Standard;
    public string VaePath { get; set; } = string.Empty;
    public Quantization Quantization { get; set; } = Quantization.Default;
    public Schedule Schedule { get; set; } = Schedule.Default;

    #endregion
}
