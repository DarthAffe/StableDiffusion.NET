using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class DiffusionModelParameter
{
    /// <summary>
    /// path to vae
    /// </summary>
    public string VaePath { get; set; } = string.Empty;

    /// <summary>
    /// path to taesd. Using Tiny AutoEncoder for fast decoding (low quality)
    /// </summary>
    public string TaesdPath { get; set; } = string.Empty;

    /// <summary>
    /// lora model directory
    /// </summary>
    public string LoraModelDirectory { get; set; } = string.Empty;

    /// <summary>
    /// path to embeddings
    /// </summary>
    public string EmbeddingsDirectory { get; set; } = string.Empty;

    /// <summary>
    /// path to control net model
    /// </summary>
    public string ControlNetPath { get; set; } = string.Empty;

    /// <summary>
    /// number of threads to use during computation (default: -1)
    /// If threads = -1, then threads will be set to the number of CPU physical cores
    /// </summary>
    public int ThreadCount { get; set; } = 1;

    /// <summary>
    /// 
    /// </summary>
    public bool VaeDecodeOnly { get; set; } = false;

    public bool FreeParamsImmediately { get; set; } = false;

    /// <summary>
    /// process vae in tiles to reduce memory usage
    /// </summary>
    public bool VaeTiling { get; set; } = false;

    public bool OffloadParamsToCPU { get; set; } = false;

    /// <summary>
    /// keep clip in cpu (for low vram)
    /// </summary>
    public bool KeepClipOnCPU { get; set; } = false;

    /// <summary>
    /// keep controlnet in cpu
    /// </summary>
    public bool KeepControlNetOnCPU { get; set; } = false;

    /// <summary>
    /// keep vae in cpu (for low vram)
    /// </summary>
    public bool KeepVaeOnCPU { get; set; } = false;

    /// <summary>
    /// use flash attention in the diffusion model (for low vram)
    /// Might lower quality, since it implies converting k and v to f16.
    /// This might crash if it is not supported by the backend.
    /// </summary>
    public bool FlashAttention { get; set; } = false;

    public bool TaePreviewOnly { get; set; } = false;

    /// <summary>
    /// use Conv2d direct in the diffusion model
    /// This might crash if it is not supported by the backend.
    /// </summary>
    public bool DiffusionConvDirect { get; set; } = false;

    /// <summary>
    /// use Conv2d direct in the vae model (should improve the performance)
    /// This might crash if it is not supported by the backend.
    /// </summary>
    public bool VaeConvDirect { get; set; } = false;

    /// <summary>
    /// RNG (default: Standard)
    /// </summary>
    public RngType RngType { get; set; } = RngType.Standard;

    public RngType SamplerRngType { get; set; } = RngType.Standard;

    public Prediction Prediction { get; set; } = Prediction.Default;

    public LoraApplyMode LoraApplyMode { get; set; } = LoraApplyMode.Auto;

    /// <summary>
    /// quantizes on load
    /// not really useful in most cases
    /// </summary>
    public Quantization Quantization { get; set; } = Quantization.Unspecified;

    public float FlowShift { get; set; } = 0;

    public bool ForceSdxlVaeConvScale { get; set; } = false;

    /// <summary>
    /// path to PHOTOMAKER stacked id embeddings
    /// </summary>
    public string StackedIdEmbeddingsDirectory { get; set; } = string.Empty;

    public string TensorTypeRules { get; set; } = string.Empty;

    /// <summary>
    /// path to full model
    /// </summary>
    public string ModelPath { get; set; } = string.Empty;

    /// <summary>
    /// path to the standalone diffusion model
    /// </summary>
    public string DiffusionModelPath { get; set; } = string.Empty;

    /// <summary>
    /// path to the clip-l text encoder
    /// </summary>
    public string ClipLPath { get; set; } = string.Empty;

    /// <summary>
    /// path to the clip-g text encoder
    /// </summary>
    public string ClipGPath { get; set; } = string.Empty;

    /// <summary>
    /// path to the the t5xxl text encoder
    /// </summary>
    public string T5xxlPath { get; set; } = string.Empty;

    public string Qwen2VLPath { get; set; } = string.Empty;

    public string Qwen2VLVisionPath { get; set; } = string.Empty;

    public string ClipVisionPath { get; set; } = string.Empty;
    public string HighNoiseDiffusionModelPath { get; set; } = string.Empty;

    public bool ChromaUseDitMap { get; set; } = true;
    public bool ChromaEnableT5Map { get; set; } = false;
    public int ChromaT5MaskPad { get; set; } = 1;

    public static DiffusionModelParameter Create() => new();
}