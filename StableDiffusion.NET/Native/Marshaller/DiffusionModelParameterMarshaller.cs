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
            t5xxl_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.T5xxlPath),
            diffusion_model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.DiffusionModelPath),
            vae_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.VaePath),
            taesd_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.TaesdPath),
            control_net_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ControlNetPath),
            lora_model_dir = AnsiStringMarshaller.ConvertToUnmanaged(managed.LoraModelDirectory),
            embedding_dir = AnsiStringMarshaller.ConvertToUnmanaged(managed.EmbeddingsDirectory),
            stacked_id_embed_dir = AnsiStringMarshaller.ConvertToUnmanaged(managed.StackedIdEmbeddingsDirectory),
            vae_decode_only = (sbyte)(managed.VaeDecodeOnly ? 1 : 0),
            vae_tiling = (sbyte)(managed.VaeTiling ? 1 : 0),
            free_params_immediately = 0, // DarthAffe 06.08.2025: Static value
            n_threads = managed.ThreadCount,
            wtype = managed.Quantization,
            rng_type = managed.RngType,
            schedule = managed.Schedule,
            keep_clip_on_cpu = (sbyte)(managed.KeepClipOnCPU ? 1 : 0),
            keep_control_net_on_cpu = (sbyte)(managed.KeepControlNetOnCPU ? 1 : 0),
            keep_vae_on_cpu = (sbyte)(managed.KeepVaeOnCPU ? 1 : 0),
            diffusion_flash_attn = (sbyte)(managed.FlashAttention ? 1 : 0),
            diffusion_conv_direct = (sbyte)(managed.DiffusionConvDirect ? 1 : 0),
            vae_conv_direct = (sbyte)(managed.VaeConfDirect ? 1 : 0),
            chroma_use_dit_mask = (sbyte)(managed.ChromaUseDitMap ? 1 : 0),
            chroma_use_t5_mask = (sbyte)(managed.ChromaEnableT5Map ? 1 : 0),
            chroma_t5_mask_pad = managed.ChromaT5MaskPad
        };

    public static DiffusionModelParameter ConvertToManaged(Native.Types.sd_ctx_params_t unmanaged)
        => new()
        {
            ModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.model_path) ?? string.Empty,
            ClipLPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_l_path) ?? string.Empty,
            ClipGPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_g_path) ?? string.Empty,
            T5xxlPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.t5xxl_path) ?? string.Empty,
            DiffusionModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.diffusion_model_path) ?? string.Empty,
            VaePath = AnsiStringMarshaller.ConvertToManaged(unmanaged.vae_path) ?? string.Empty,
            TaesdPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.taesd_path) ?? string.Empty,
            ControlNetPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.control_net_path) ?? string.Empty,
            LoraModelDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.lora_model_dir) ?? string.Empty,
            EmbeddingsDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.embedding_dir) ?? string.Empty,
            StackedIdEmbeddingsDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.stacked_id_embed_dir) ?? string.Empty,
            VaeDecodeOnly = unmanaged.vae_decode_only == 1,
            VaeTiling = unmanaged.vae_tiling == 1,
            ThreadCount = unmanaged.n_threads,
            Quantization = unmanaged.wtype,
            RngType = unmanaged.rng_type,
            Schedule = unmanaged.schedule,
            KeepClipOnCPU = unmanaged.keep_clip_on_cpu == 1,
            KeepControlNetOnCPU = unmanaged.keep_control_net_on_cpu == 1,
            KeepVaeOnCPU = unmanaged.keep_vae_on_cpu == 1,
            FlashAttention = unmanaged.diffusion_flash_attn == 1,
            DiffusionConvDirect = unmanaged.diffusion_conv_direct == 1,
            VaeConfDirect = unmanaged.vae_conv_direct == 1,
            ChromaUseDitMap = unmanaged.chroma_use_dit_mask == 1,
            ChromaEnableT5Map = unmanaged.chroma_use_t5_mask == 1,
            ChromaT5MaskPad = unmanaged.chroma_t5_mask_pad
        };

    public static void Free(Native.Types.sd_ctx_params_t unmanaged)
    {
        AnsiStringMarshaller.Free(unmanaged.model_path);
        AnsiStringMarshaller.Free(unmanaged.clip_l_path);
        AnsiStringMarshaller.Free(unmanaged.clip_g_path);
        AnsiStringMarshaller.Free(unmanaged.t5xxl_path);
        AnsiStringMarshaller.Free(unmanaged.diffusion_model_path);
        AnsiStringMarshaller.Free(unmanaged.vae_path);
        AnsiStringMarshaller.Free(unmanaged.taesd_path);
        AnsiStringMarshaller.Free(unmanaged.control_net_path);
        AnsiStringMarshaller.Free(unmanaged.lora_model_dir);
        AnsiStringMarshaller.Free(unmanaged.embedding_dir);
        AnsiStringMarshaller.Free(unmanaged.stacked_id_embed_dir);
    }
}