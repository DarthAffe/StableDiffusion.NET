#pragma warning disable CS0169 // Field is never used
#pragma warning disable IDE1006
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UseSymbolAlias

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using HPPH;

namespace StableDiffusion.NET;

using rng_type_t = RngType;
using sample_method_t = Sampler;
using schedule_t = Schedule;
using sd_type_t = Quantization;
using sd_log_level_t = LogLevel;
using uint32_t = uint;
using uint8_t = byte;
using int64_t = long;
using size_t = nuint;
using int32_t = int;
using sd_ctx_params_t = DiffusionModelParameter;
using sd_img_gen_params_t = ImageGenerationParameter;
using sd_vid_gen_params_t = Native.Types.sd_vid_gen_params_t;
using sd_image_t = Native.Types.sd_image_t;
using sd_ctx_t = Native.Types.sd_ctx_t;
using upscaler_ctx_t = Native.Types.upscaler_ctx_t;

internal unsafe partial class Native
{
    #region Constants

    private const string LIB_NAME = "stable-diffusion";

    #endregion

    internal static class Types
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_ctx_params_t
        {
            public byte* model_path;
            public byte* clip_l_path;
            public byte* clip_g_path;
            public byte* t5xxl_path;
            public byte* diffusion_model_path;
            public byte* vae_path;
            public byte* taesd_path;
            public byte* control_net_path;
            public byte* lora_model_dir;
            public byte* embedding_dir;
            public byte* stacked_id_embed_dir;
            public sbyte vae_decode_only;
            public sbyte vae_tiling;
            public sbyte free_params_immediately;
            public int n_threads;
            public sd_type_t wtype;
            public rng_type_t rng_type;
            public schedule_t schedule;
            public sbyte keep_clip_on_cpu;
            public sbyte keep_control_net_on_cpu;
            public sbyte keep_vae_on_cpu;
            public sbyte diffusion_flash_attn;
            public sbyte diffusion_conv_direct;
            public sbyte vae_conv_direct;
            public sbyte chroma_use_dit_mask;
            public sbyte chroma_use_t5_mask;
            public int chroma_t5_mask_pad;
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
        internal struct sd_img_gen_params_t
        {
            public byte* prompt;
            public byte* negative_prompt;
            public int clip_skip;
            public sd_guidance_params_t guidance;
            public sd_image_t init_image;
            public sd_image_t* ref_images;
            public int ref_images_count;
            public sd_image_t mask_image;
            public int width;
            public int height;
            public sample_method_t sample_method;
            public int sample_steps;
            public float eta;
            public float strength;
            public int64_t seed;
            public int batch_count;
            public sd_image_t* control_cond;
            public float control_strength;
            public float style_strength;
            public sbyte normalize_input;
            public byte* input_id_images_path;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct sd_vid_gen_params_t
        {
            public sd_image_t init_image;
            public int width;
            public int height;
            public sd_guidance_params_t guidance;
            public sample_method_t sample_method;
            public int sample_steps;
            public float strength;
            public int64_t seed;
            public int video_frames;
            public int motion_bucket_id;
            public int fps;
            public float augmentation_level;
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
    internal static partial string sd_schedule_name(schedule_t schedule);

    [LibraryImport(LIB_NAME, EntryPoint = "str_to_schedule")]
    internal static partial schedule_t str_to_schedule([MarshalAs(UnmanagedType.LPStr)] string str);

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
    internal static partial void sd_vid_gen_params_init(ref sd_vid_gen_params_t sd_vid_gen_params);

    [LibraryImport(LIB_NAME, EntryPoint = "generate_video")]
    [return: MarshalUsing(typeof(ImageMarshaller))]
    internal static partial sd_image_t* generate_video(sd_ctx_t* sd_ctx, in sd_vid_gen_params_t sd_vid_gen_params);  // broken

    //

    [LibraryImport(LIB_NAME, EntryPoint = "new_upscaler_ctx")]
    internal static partial upscaler_ctx_t* new_upscaler_ctx([MarshalAs(UnmanagedType.LPStr)] string esrgan_path, int n_threads, [MarshalAs(UnmanagedType.I1)] bool direct);

    [LibraryImport(LIB_NAME, EntryPoint = "free_upscaler_ctx")]
    internal static partial void free_upscaler_ctx(upscaler_ctx_t* upscaler_ctx);

    //

    [LibraryImport(LIB_NAME, EntryPoint = "upscale")]
    [return: MarshalUsing(typeof(ImageMarshaller))]
    internal static partial Image<ColorRGB> upscale(upscaler_ctx_t* upscaler_ctx, [MarshalUsing(typeof(ImageMarshaller))] IImage input_image, uint32_t upscale_factor);

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
    internal static partial uint8_t* preprocess_canny(uint8_t* img,
                                                      int width,
                                                      int height,
                                                      float high_threshold,
                                                      float low_threshold,
                                                      float weak,
                                                      float strong,
                                                      [MarshalAs(UnmanagedType.I1)] bool inverse);

    #endregion
}