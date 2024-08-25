namespace StableDiffusion.NET;

public sealed class DiffusionModelParameter : IDiffusionModelParameter, IQuantizedModelParameter, IPhotomakerModelParameter
{
    public DiffusionModelType DiffusionModelType { get; set; } = DiffusionModelType.None;

    public string VaePath { get; set; } = string.Empty;
    public string TaesdPath { get; set; } = string.Empty;

    public string LoraModelDirectory { get; set; } = string.Empty;
    public string EmbeddingsDirectory { get; set; } = string.Empty;
    public string ControlNetPath { get; set; } = string.Empty;

    public int ThreadCount { get; set; } = 1;

    public bool VaeDecodeOnly { get; set; } = false;
    public bool VaeTiling { get; set; } = false;
    public bool KeepControlNetOnCPU { get; set; } = false;
    public bool KeepClipOnCPU { get; set; } = false;
    public bool KeepVaeOnCPU { get; set; } = false;

    public RngType RngType { get; set; } = RngType.Standard;
    public Schedule Schedule { get; set; } = Schedule.Default;

    public Quantization Quantization { get; set; } = Quantization.Unspecified;

    // Stable Diffusion only
    public string ModelPath { get; set; } = string.Empty;
    public string StackedIdEmbeddingsDirectory { get; set; } = string.Empty;

    // Flux only
    public string DiffusionModelPath { get; set; } = string.Empty;
    public string ClipLPath { get; set; } = string.Empty;
    public string T5xxlPath { get; set; } = string.Empty;
}