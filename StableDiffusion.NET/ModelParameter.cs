﻿namespace StableDiffusion.NET;

public class ModelParameter
{
    #region Properties & Fields

    public int ThreadCount { get; set; } = 8;
    public bool VaeDecodeOnly { get; set; } = false;
    public bool VaeTiling { get; set; } = false;
    public string TaesdPath { get; set; } = string.Empty;
    public string LoraModelDir { get; set; } = string.Empty;
    public RngType RngType { get; set; } = RngType.Standard;
    public string VaePath { get; set; } = string.Empty;

    //TODO DarthAffe 01.01.2024: K-Quants doesn't seem to work so far
    public Quantization Quantization { get; set; } = Quantization.F16;

    public Schedule Schedule { get; set; } = Schedule.Default;

    #endregion
}
