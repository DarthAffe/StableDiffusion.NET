namespace StableDiffusion.NET;

public interface IDiffusionModelParameter
{
    DiffusionModelType DiffusionModelType { get; set; }

    string VaePath { get; set; }
    string TaesdPath { get; set; }

    string LoraModelDirectory { get; set; }
    string EmbeddingsDirectory { get; set; }
    string ControlNetPath { get; set; }

    bool VaeDecodeOnly { get; set; }
    bool VaeTiling { get; set; }
    bool KeepControlNetOnCPU { get; set; }
    bool KeepClipOnCPU { get; set; }
    bool KeepVaeOnCPU { get; set; }
    bool FlashAttention { get; set; }

    RngType RngType { get; set; }
    Schedule Schedule { get; set; }
}