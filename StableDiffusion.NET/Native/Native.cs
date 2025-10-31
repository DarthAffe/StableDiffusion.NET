﻿#pragma warning disable IDE1006
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UseSymbolAlias

using HPPH;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

using int32_t = int;
using int64_t = long;
using rng_type_t = RngType;
using sample_method_t = Sampler;
using scheduler_t = Scheduler;
using prediction_t = Prediction;
using sd_ctx_params_t = DiffusionModelParameter;
using sd_ctx_t = Native.Types.sd_ctx_t;
using sd_image_t = Native.Types.sd_image_t;
using sd_sample_params_t = SampleParameter;
using sd_img_gen_params_t = ImageGenerationParameter;
using sd_log_level_t = LogLevel;
using sd_type_t = Quantization;
using sd_vid_gen_params_t = VideoGenerationParameter;
using size_t = nuint;
using uint32_t = uint;
using uint8_t = byte;
using upscaler_ctx_t = Native.Types.upscaler_ctx_t;

internal unsafe partial class Native
{
    #region Constants

    private const string LIB_NAME = "stable-diffusion";

    #endregion

    internal static class Types
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct sd_tiling_params_t
        {
            public sbyte enabled;
            public int tile_size_x;
            public int tile_size_y;
            public float target_overlap;
            public float rel_size_x;
            public float rel_size_y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_ctx_params_t
        {
            public byte* model_path;
            public byte* clip_l_path;
            public byte* clip_g_path;
            public byte* clip_vision_path;
            public byte* t5xxl_path;
            public byte* qwen2vl_path;
            public byte* qwen2vl_vision_path;
            public byte* diffusion_model_path;
            public byte* high_noise_diffusion_model_path;
            public byte* vae_path;
            public byte* taesd_path;
            public byte* control_net_path;
            public byte* lora_model_dir;
            public byte* embedding_dir;
            public byte* photo_maker_path;
            public sbyte vae_decode_only;
            public sbyte free_params_immediately;
            public int n_threads;
            public sd_type_t wtype;
            public rng_type_t rng_type;
            public prediction_t prediction;
            public sbyte offload_params_to_cpu;
            public sbyte keep_clip_on_cpu;
            public sbyte keep_control_net_on_cpu;
            public sbyte keep_vae_on_cpu;
            public sbyte diffusion_flash_attn;
            public sbyte diffusion_conv_direct;
            public sbyte vae_conv_direct;
            public sbyte force_sdxl_vae_conv_scale;
            public sbyte chroma_use_dit_mask;
            public sbyte chroma_use_t5_mask;
            public int chroma_t5_mask_pad;
            public float flow_shift;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_image_t
        {
            public uint32_t width;
            public uint32_t height;
            public uint32_t channel;
            public uint8_t* data;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_slg_params_t
        {
            public int* layers;
            public size_t layer_count;
            public float layer_start;
            public float layer_end;
            public float scale;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_guidance_params_t
        {
            public float txt_cfg;
            public float img_cfg;
            public float min_cfg;
            public float distilled_guidance;
            public sd_slg_params_t slg;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_sample_params_t
        {
            public sd_guidance_params_t guidance;
            public scheduler_t scheduler;
            public sample_method_t sample_method;
            public int sample_steps;
            public float eta;
            public int shifted_timestep;
        }

        internal struct sd_pm_params_t
        {
            public sd_image_t* id_images;
            public int id_images_count;
            public byte* id_embed_path;
            public float style_strength;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_img_gen_params_t
        {
            public byte* prompt;
            public byte* negative_prompt;
            public int clip_skip;
            public sd_image_t init_image;
            public sd_image_t* ref_images;
            public int ref_images_count;
            public sbyte auto_resize_ref_image;
            public sbyte increase_ref_index;
            public sd_image_t mask_image;
            public int width;
            public int height;
            public sd_sample_params_t sample_params;
            public float strength;
            public int64_t seed;
            public int batch_count;
            public sd_image_t control_image;
            public float control_strength;
            public sd_pm_params_t pm_params;
            public sd_tiling_params_t vae_tiling_params;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_vid_gen_params_t
        {
            public byte* prompt;
            public byte* negative_prompt;
            public int clip_skip;
            public sd_image_t init_image;
            public sd_image_t end_image;
            public sd_image_t* control_frames;
            public int control_frames_size;
            public int width;
            public int height;
            public sd_sample_params_t sample_params;
            public sd_sample_params_t high_noise_sample_params;
            public float moe_boundary;
            public float strength;
            public int64_t seed;
            public int video_frames;
            public float vace_strength;
        }

        internal struct sd_ctx_t;
        internal struct upscaler_ctx_t;
    }

    #region Delegates

    internal delegate void sd_log_cb_t(sd_log_level_t level, [MarshalAs(UnmanagedType.LPStr)] string text, void* data);
    internal delegate void sd_progress_cb_t(int step, int steps, float time, void* data);

    #endregion

    #region Methods

    [LibraryImport(LIB_NAME, EntryPoint = "sd_set_log_callback")]
    internal static partial void sd_set_log_callback(sd_log_cb_t sd_log_cb, void* data);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_set_progress_callback")]
    internal static partial void sd_set_progress_callback(sd_progress_cb_t cb, void* data);

    [LibraryImport(LIB_NAME, EntryPoint = "get_num_physical_cores")]
    internal static partial int32_t get_num_physical_cores();

    [LibraryImport(LIB_NAME, EntryPoint = "sd_get_system_info")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_get_system_info();

    //

    [LibraryImport(LIB_NAME, EntryPoint = "sd_type_name")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_type_name(sd_type_t type);

    [LibraryImport(LIB_NAME, EntryPoint = "str_to_sd_type")]
    internal static partial sd_type_t str_to_sd_type([MarshalAs(UnmanagedType.LPStr)] string str);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_rng_type_name")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_rng_type_name(rng_type_t rng_type);

    [LibraryImport(LIB_NAME, EntryPoint = "str_to_rng_type")]
    internal static partial rng_type_t str_to_rng_type([MarshalAs(UnmanagedType.LPStr)] string str);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_sample_method_name")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_sample_method_name(sample_method_t sample_method);

    [LibraryImport(LIB_NAME, EntryPoint = "str_to_sample_method")]
    internal static partial sample_method_t str_to_sample_method([MarshalAs(UnmanagedType.LPStr)] string str);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_schedule_name")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_schedule_name(scheduler_t schedule);

    [LibraryImport(LIB_NAME, EntryPoint = "str_to_schedule")]
    internal static partial scheduler_t str_to_schedule([MarshalAs(UnmanagedType.LPStr)] string str);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_prediction_name")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_prediction_name(prediction_t prediction);

    [LibraryImport(LIB_NAME, EntryPoint = "str_to_prediction")]
    internal static partial prediction_t str_to_prediction([MarshalAs(UnmanagedType.LPStr)] string str);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "sd_ctx_params_init")]
    internal static partial void sd_ctx_params_init([MarshalUsing(typeof(DiffusionModelParameterMarshaller))] ref sd_ctx_params_t sd_ctx_params);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_ctx_params_to_str")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_ctx_params_to_str([MarshalUsing(typeof(DiffusionModelParameterMarshaller))] in sd_ctx_params_t sd_ctx_params);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "new_sd_ctx")]
    internal static partial sd_ctx_t* new_sd_ctx([MarshalUsing(typeof(DiffusionModelParameterMarshaller))] in sd_ctx_params_t sd_ctx_params);

    [LibraryImport(LIB_NAME, EntryPoint = "free_sd_ctx")]
    internal static partial void free_sd_ctx(sd_ctx_t* sd_ctx);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_get_default_sample_method")]
    internal static partial sample_method_t sd_get_default_sample_method(sd_ctx_t* sd_ctx);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "sd_sample_params_init")]
    internal static partial void sd_sample_params_init([MarshalUsing(typeof(SampleParameterMarshaller))] ref sd_sample_params_t sample_params);
    [LibraryImport(LIB_NAME, EntryPoint = "sd_sample_params_to_str")]
    internal static partial char* sd_sample_params_to_str([MarshalUsing(typeof(SampleParameterMarshaller))] in sd_sample_params_t sample_params);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "sd_img_gen_params_init")]
    internal static partial void sd_img_gen_params_init([MarshalUsing(typeof(ImageGenerationParameterMarshaller))] ref sd_img_gen_params_t sd_img_gen_params);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_img_gen_params_to_str")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string sd_img_gen_params_to_str([MarshalUsing(typeof(ImageGenerationParameterMarshaller))] in sd_img_gen_params_t sd_img_gen_params);

    [LibraryImport(LIB_NAME, EntryPoint = "generate_image")]
    internal static partial sd_image_t* generate_image(sd_ctx_t* sd_ctx, [MarshalUsing(typeof(ImageGenerationParameterMarshaller))] in sd_img_gen_params_t sd_img_gen_params);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "sd_vid_gen_params_init")]
    internal static partial void sd_vid_gen_params_init([MarshalUsing(typeof(VideoGenerationParameterMarshaller))] ref sd_vid_gen_params_t sd_vid_gen_params);

    [LibraryImport(LIB_NAME, EntryPoint = "generate_video")]
    internal static partial sd_image_t* generate_video(sd_ctx_t* sd_ctx, [MarshalUsing(typeof(VideoGenerationParameterMarshaller))] in sd_vid_gen_params_t sd_vid_gen_params, out int num_frames_out);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "new_upscaler_ctx")]
    internal static partial upscaler_ctx_t* new_upscaler_ctx([MarshalAs(UnmanagedType.LPStr)] string esrgan_path,
                                                             [MarshalAs(UnmanagedType.I1)] bool offload_params_to_cpu,
                                                             [MarshalAs(UnmanagedType.I1)] bool direct,
                                                             int n_threads);

    [LibraryImport(LIB_NAME, EntryPoint = "free_upscaler_ctx")]
    internal static partial void free_upscaler_ctx(upscaler_ctx_t* upscaler_ctx);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "upscale")]
    [return: MarshalUsing(typeof(ImageMarshaller))]
    internal static partial Image<ColorRGB> upscale(upscaler_ctx_t* upscaler_ctx, [MarshalUsing(typeof(ImageMarshaller))] IImage input_image, uint32_t upscale_factor);

    [LibraryImport(LIB_NAME, EntryPoint = "get_upscale_factor")]
    internal static partial int get_upscale_factor(upscaler_ctx_t* upscaler_ctx);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "convert")]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool convert([MarshalAs(UnmanagedType.LPStr)] string input_path,
                                         [MarshalAs(UnmanagedType.LPStr)] string vae_path,
                                         [MarshalAs(UnmanagedType.LPStr)] string output_path,
                                         sd_type_t output_type,
                                         [MarshalAs(UnmanagedType.LPStr)] string tensor_type_rules);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "preprocess_canny")]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool preprocess_canny(sd_image_t image,
                                                  float high_threshold,
                                                  float low_threshold,
                                                  float weak,
                                                  float strong,
                                                  [MarshalAs(UnmanagedType.I1)] bool inverse);

    #endregion
}