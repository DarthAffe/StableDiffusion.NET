using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(DiffusionModelParameter), MarshalMode.ManagedToUnmanagedIn, typeof(DiffusionModelParameterMarshallerIn))]
[CustomMarshaller(typeof(DiffusionModelParameter), MarshalMode.ManagedToUnmanagedOut, typeof(DiffusionModelParameterMarshaller))]
[CustomMarshaller(typeof(DiffusionModelParameter), MarshalMode.ManagedToUnmanagedRef, typeof(DiffusionModelParameterMarshallerRef))]
internal static unsafe class DiffusionModelParameterMarshaller
{
    public static DiffusionModelParameter ConvertToManaged(Native.Types.sd_ctx_params_t unmanaged)
    {
        DiffusionModelParameter parameter = new()
        {
            ModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.model_path) ?? string.Empty,
            ClipLPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_l_path) ?? string.Empty,
            ClipGPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_g_path) ?? string.Empty,
            ClipVisionPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.clip_vision_path) ?? string.Empty,
            T5xxlPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.t5xxl_path) ?? string.Empty,
            LLMPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.llm_path) ?? string.Empty,
            LLMVisionPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.llm_vision_path) ?? string.Empty,
            DiffusionModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.diffusion_model_path) ?? string.Empty,
            HighNoiseDiffusionModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.high_noise_diffusion_model_path) ?? string.Empty,
            VaePath = AnsiStringMarshaller.ConvertToManaged(unmanaged.vae_path) ?? string.Empty,
            TaesdPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.taesd_path) ?? string.Empty,
            ControlNetPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.control_net_path) ?? string.Empty,
            LoraModelDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.lora_model_dir) ?? string.Empty,
            StackedIdEmbeddingsDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.photo_maker_path) ?? string.Empty,
            TensorTypeRules = AnsiStringMarshaller.ConvertToManaged(unmanaged.tensor_type_rules) ?? string.Empty,
            VaeDecodeOnly = unmanaged.vae_decode_only == 1,
            FreeParamsImmediately = unmanaged.free_params_immediately == 1,
            ThreadCount = unmanaged.n_threads,
            Quantization = unmanaged.wtype,
            RngType = unmanaged.rng_type,
            SamplerRngType = unmanaged.sampler_rng_type,
            Prediction = unmanaged.prediction,
            LoraApplyMode = unmanaged.lora_apply_mode,
            OffloadParamsToCPU = unmanaged.offload_params_to_cpu == 1,
            KeepClipOnCPU = unmanaged.keep_clip_on_cpu == 1,
            KeepControlNetOnCPU = unmanaged.keep_control_net_on_cpu == 1,
            KeepVaeOnCPU = unmanaged.keep_vae_on_cpu == 1,
            FlashAttention = unmanaged.diffusion_flash_attn == 1,
            TaePreviewOnly = unmanaged.tae_preview_only == 1,
            DiffusionConvDirect = unmanaged.diffusion_conv_direct == 1,
            VaeConvDirect = unmanaged.vae_conv_direct == 1,
            ForceSdxlVaeConvScale = unmanaged.force_sdxl_vae_conv_scale == 1,
            ChromaUseDitMap = unmanaged.chroma_use_dit_mask == 1,
            ChromaEnableT5Map = unmanaged.chroma_use_t5_mask == 1,
            ChromaT5MaskPad = unmanaged.chroma_t5_mask_pad,
            FlowShift = unmanaged.flow_shift
        };

        for (int i = 0; i < unmanaged.embedding_count; i++)
        {
            Native.Types.sd_embedding_t embedding = unmanaged.embeddings[i];
            parameter.Embeddings.Add(new Embedding
            {
                Name = AnsiStringMarshaller.ConvertToManaged(embedding.name) ?? string.Empty,
                Path = AnsiStringMarshaller.ConvertToManaged(embedding.path) ?? string.Empty
            });
        }

        return parameter;
    }

    internal ref struct DiffusionModelParameterMarshallerIn
    {
        private Native.Types.sd_ctx_params_t _ctxParams;

        private Native.Types.sd_embedding_t* _embeddings;

        public DiffusionModelParameterMarshallerIn() { }

        public void FromManaged(DiffusionModelParameter managed)
        {
            //_embeddings = (Native.Types.sd_embedding_t*)NativeMemory.Alloc((nuint)managed.Embeddings.Count, (nuint)Marshal.SizeOf<Native.Types.sd_embedding_t>());

            //for (int i = 0; i < managed.Embeddings.Count; i++)
            //{
            //    Embedding embedding = managed.Embeddings[i];

            //    _embeddings[i] = new Native.Types.sd_embedding_t
            //    {
            //        name = AnsiStringMarshaller.ConvertToUnmanaged(embedding.Name),
            //        path = AnsiStringMarshaller.ConvertToUnmanaged(embedding.Path),
            //    };
            //}

            //HACK DarthAffe 25.12.2025 Workaround to support EmbeddingsDir till the next major release
            List<Embedding> embeddings = [];
            {
                embeddings.AddRange(managed.Embeddings);

                try
                {
                    if (!string.IsNullOrWhiteSpace(managed.EmbeddingsDirectory) && Directory.Exists(managed.EmbeddingsDirectory))
                    {
                        foreach (string file in Directory.GetFiles(managed.EmbeddingsDirectory))
                            embeddings.Add(new Embedding(Path.GetFileNameWithoutExtension(file), file));
                    }
                }
                catch { /**/ }

                _embeddings = (Native.Types.sd_embedding_t*)NativeMemory.Alloc((nuint)embeddings.Count, (nuint)Marshal.SizeOf<Native.Types.sd_embedding_t>());

                for (int i = 0; i < embeddings.Count; i++)
                {
                    Embedding embedding = embeddings[i];

                    _embeddings[i] = new Native.Types.sd_embedding_t
                    {
                        name = AnsiStringMarshaller.ConvertToUnmanaged(embedding.Name),
                        path = AnsiStringMarshaller.ConvertToUnmanaged(embedding.Path),
                    };
                }
            }

            _ctxParams = new Native.Types.sd_ctx_params_t
            {
                model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ModelPath),
                clip_l_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ClipLPath),
                clip_g_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ClipGPath),
                clip_vision_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ClipVisionPath),
                t5xxl_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.T5xxlPath),
                llm_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.LLMPath),
                llm_vision_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.LLMVisionPath),
                diffusion_model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.DiffusionModelPath),
                high_noise_diffusion_model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.HighNoiseDiffusionModelPath),
                vae_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.VaePath),
                taesd_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.TaesdPath),
                control_net_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ControlNetPath),
                lora_model_dir = AnsiStringMarshaller.ConvertToUnmanaged(managed.LoraModelDirectory),
                embeddings = _embeddings,
                embedding_count = (uint)embeddings.Count,
                photo_maker_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.StackedIdEmbeddingsDirectory),
                tensor_type_rules = AnsiStringMarshaller.ConvertToUnmanaged(managed.TensorTypeRules),
                vae_decode_only = (sbyte)(managed.VaeDecodeOnly ? 1 : 0),
                free_params_immediately = (sbyte)(managed.FreeParamsImmediately ? 1 : 0),
                n_threads = managed.ThreadCount,
                wtype = managed.Quantization,
                rng_type = managed.RngType,
                sampler_rng_type = managed.SamplerRngType,
                prediction = managed.Prediction,
                lora_apply_mode = managed.LoraApplyMode,
                offload_params_to_cpu = (sbyte)(managed.OffloadParamsToCPU ? 1 : 0),
                keep_clip_on_cpu = (sbyte)(managed.KeepClipOnCPU ? 1 : 0),
                keep_control_net_on_cpu = (sbyte)(managed.KeepControlNetOnCPU ? 1 : 0),
                keep_vae_on_cpu = (sbyte)(managed.KeepVaeOnCPU ? 1 : 0),
                diffusion_flash_attn = (sbyte)(managed.FlashAttention ? 1 : 0),
                tae_preview_only = (sbyte)(managed.TaePreviewOnly ? 1 : 0),
                diffusion_conv_direct = (sbyte)(managed.DiffusionConvDirect ? 1 : 0),
                vae_conv_direct = (sbyte)(managed.VaeConvDirect ? 1 : 0),
                force_sdxl_vae_conv_scale = (sbyte)(managed.ForceSdxlVaeConvScale ? 1 : 0),
                chroma_use_dit_mask = (sbyte)(managed.ChromaUseDitMap ? 1 : 0),
                chroma_use_t5_mask = (sbyte)(managed.ChromaEnableT5Map ? 1 : 0),
                chroma_t5_mask_pad = managed.ChromaT5MaskPad,
                flow_shift = managed.FlowShift
            };
        }

        public Native.Types.sd_ctx_params_t ToUnmanaged() => _ctxParams;

        public void Free()
        {

            AnsiStringMarshaller.Free(_ctxParams.model_path);
            AnsiStringMarshaller.Free(_ctxParams.clip_l_path);
            AnsiStringMarshaller.Free(_ctxParams.clip_g_path);
            AnsiStringMarshaller.Free(_ctxParams.t5xxl_path);
            AnsiStringMarshaller.Free(_ctxParams.llm_path);
            AnsiStringMarshaller.Free(_ctxParams.llm_vision_path);
            AnsiStringMarshaller.Free(_ctxParams.diffusion_model_path);
            AnsiStringMarshaller.Free(_ctxParams.vae_path);
            AnsiStringMarshaller.Free(_ctxParams.taesd_path);
            AnsiStringMarshaller.Free(_ctxParams.control_net_path);
            AnsiStringMarshaller.Free(_ctxParams.lora_model_dir);
            AnsiStringMarshaller.Free(_ctxParams.photo_maker_path);
            AnsiStringMarshaller.Free(_ctxParams.tensor_type_rules);

            for (int i = 0; i < _ctxParams.embedding_count; i++)
            {
                AnsiStringMarshaller.Free(_ctxParams.embeddings[i].name);
                AnsiStringMarshaller.Free(_ctxParams.embeddings[i].path);
            }

            if (_embeddings != null)
                NativeMemory.Free(_embeddings);
        }
    }

    internal ref struct DiffusionModelParameterMarshallerRef()
    {
        private DiffusionModelParameterMarshallerIn _inMarshaller = new();
        private DiffusionModelParameter? _parameter;

        public void FromManaged(DiffusionModelParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_ctx_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_ctx_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public DiffusionModelParameter ToManaged() => _parameter!;

        public void Free() => _inMarshaller.Free();
    }
}