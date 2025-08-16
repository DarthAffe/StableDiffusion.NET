using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public interface IDiffusionModelParameter : IModelParameter
{
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
    bool DiffusionConvDirect { get; set; }
    bool VaeConfDirect { get; set; }

    RngType RngType { get; set; }
    Schedule Schedule { get; set; }
    Quantization Quantization { get; set; }

    string ModelPath { get; set; }

    string StackedIdEmbeddingsDirectory { get; set; }

    string DiffusionModelPath { get; set; }
    string ClipLPath { get; set; }
    string T5xxlPath { get; set; }

    bool ChromaUseDitMap { get; set; }
    bool ChromaEnableT5Map { get; set; }
    int ChromaT5MaskPad { get; set; }

    string ClipGPath { get; set; }
}