using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(DiffusionModelParameter), MarshalMode.ManagedToUnmanagedIn, typeof(DiffusionModelParameterMarshaller))]
[CustomMarshaller(typeof(DiffusionModelParameter), MarshalMode.ManagedToUnmanagedRef, typeof(DiffusionModelParameterMarshaller))]
internal static unsafe class DiffusionModelParameterMarshaller
{
    public static Native.Types.sd_ctx_params_t ConvertToUnmanaged(DiffusionModelParameter managed)
        => new()
        {
            model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ModelPath),
            clip_l_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ClipLPath),
            clip_g_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ClipGPath),
            clip_vision_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ClipVisionPath),
            t5xxl_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.T5xxlPath),
            qwen2vl_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.Qwen2VLPath),
            qwen2vl_vision_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.Qwen2VLVisionPath),
            diffusion_model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.DiffusionModelPath),
            high_noise_diffusion_model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.HighNoiseDiffusionModelPath),
            vae_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.VaePath),
            taesd_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.TaesdPath),
            control_net_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ControlNetPath),
            lora_model_dir = AnsiStringMarshaller.ConvertToUnmanaged(managed.LoraModelDirectory),
            embedding_dir = AnsiStringMarshaller.ConvertToUnmanaged(managed.EmbeddingsDirectory),
            photo_maker_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.StackedIdEmbeddingsDirectory),
            vae_decode_only = (sbyte)(managed.VaeDecodeOnly ? 1 : 0),
            free_params_immediately = 0, // DarthAffe 06.08.2025: Static value
            n_threads = managed.ThreadCount,
            wtype = managed.Quantization,
            rng_type = managed.RngType,
            prediction = managed.Prediction,
            offload_params_to_cpu = (sbyte)(managed.OffloadParamsToCPU ? 1 : 0),
            keep_clip_on_cpu = (sbyte)(managed.KeepClipOnCPU ? 1 : 0),
            keep_control_net_on_cpu = (sbyte)(managed.KeepControlNetOnCPU ? 1 : 0),
            keep_vae_on_cpu = (sbyte)(managed.KeepVaeOnCPU ? 1 : 0),
            diffusion_flash_attn = (sbyte)(managed.FlashAttention ? 1 : 0),
            diffusion_conv_direct = (sbyte)(managed.DiffusionConvDirect ? 1 : 0),
            vae_conv_direct = (sbyte)(managed.VaeConvDirect ? 1 : 0),
            force_sdxl_vae_conv_scale = (sbyte)(managed.ForceSdxlVaeConvScale ? 1 : 0),
            chroma_use_dit_mask = (sbyte)(managed.ChromaUseDitMap ? 1 : 0),
            chroma_use_t5_mask = (sbyte)(managed.ChromaEnableT5Map ? 1 : 0),
            chroma_t5_mask_pad = managed.ChromaT5MaskPad,
            flow_shift = managed.FlowShift
        };

    public static DiffusionModelParameter ConvertToManaged(Native.Types.sd_ctx_params_t unmanaged)
        => new()
        {
            ModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.model_path) ?? string.Empty,
            ClipLPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_l_path) ?? string.Empty,
            ClipGPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_g_path) ?? string.Empty,
            ClipVisionPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_vision_path) ?? string.Empty,
            T5xxlPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.t5xxl_path) ?? string.Empty,
            Qwen2VLPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.qwen2vl_path) ?? string.Empty,
            Qwen2VLVisionPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.qwen2vl_vision_path) ?? string.Empty,
            DiffusionModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.diffusion_model_path) ?? string.Empty,
            HighNoiseDiffusionModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.high_noise_diffusion_model_path) ?? string.Empty,
            VaePath = AnsiStringMarshaller.ConvertToManaged(unmanaged.vae_path) ?? string.Empty,
            TaesdPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.taesd_path) ?? string.Empty,
            ControlNetPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.control_net_path) ?? string.Empty,
            LoraModelDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.lora_model_dir) ?? string.Empty,
            EmbeddingsDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.embedding_dir) ?? string.Empty,
            StackedIdEmbeddingsDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.photo_maker_path) ?? string.Empty,
            VaeDecodeOnly = unmanaged.vae_decode_only == 1,
            ThreadCount = unmanaged.n_threads,
            Quantization = unmanaged.wtype,
            RngType = unmanaged.rng_type,
            Prediction = unmanaged.prediction,
            OffloadParamsToCPU = unmanaged.offload_params_to_cpu == 1,
            KeepClipOnCPU = unmanaged.keep_clip_on_cpu == 1,
            KeepControlNetOnCPU = unmanaged.keep_control_net_on_cpu == 1,
            KeepVaeOnCPU = unmanaged.keep_vae_on_cpu == 1,
            FlashAttention = unmanaged.diffusion_flash_attn == 1,
            DiffusionConvDirect = unmanaged.diffusion_conv_direct == 1,
            VaeConvDirect = unmanaged.vae_conv_direct == 1,
            ForceSdxlVaeConvScale = unmanaged.force_sdxl_vae_conv_scale == 1,
            ChromaUseDitMap = unmanaged.chroma_use_dit_mask == 1,
            ChromaEnableT5Map = unmanaged.chroma_use_t5_mask == 1,
            ChromaT5MaskPad = unmanaged.chroma_t5_mask_pad,
            FlowShift = unmanaged.flow_shift
        };

    public static void Free(Native.Types.sd_ctx_params_t unmanaged)
    {
        AnsiStringMarshaller.Free(unmanaged.model_path);
        AnsiStringMarshaller.Free(unmanaged.clip_l_path);
        AnsiStringMarshaller.Free(unmanaged.clip_g_path);
        AnsiStringMarshaller.Free(unmanaged.t5xxl_path);
        AnsiStringMarshaller.Free(unmanaged.qwen2vl_path);
        AnsiStringMarshaller.Free(unmanaged.qwen2vl_vision_path);
        AnsiStringMarshaller.Free(unmanaged.diffusion_model_path);
        AnsiStringMarshaller.Free(unmanaged.vae_path);
        AnsiStringMarshaller.Free(unmanaged.taesd_path);
        AnsiStringMarshaller.Free(unmanaged.control_net_path);
        AnsiStringMarshaller.Free(unmanaged.lora_model_dir);
        AnsiStringMarshaller.Free(unmanaged.embedding_dir);
        AnsiStringMarshaller.Free(unmanaged.photo_maker_path);
    }
}