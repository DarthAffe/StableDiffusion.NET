#pragma warning disable CS0169 // Field is never used
#pragma warning disable IDE1006
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

using rng_type_t = RngType;
using sample_method_t = Sampler;
using schedule_t = Schedule;
using sd_type_t = Quantization;
using sd_log_level_t = LogLevel;

internal unsafe partial class Native
{
    #region Constants

    private const string LIB_NAME = "stable-diffusion";

    #endregion

    #region Delegates

    internal delegate void sd_log_cb_t(sd_log_level_t level, [MarshalAs(UnmanagedType.LPStr)] string text, void* data);
    internal delegate void sd_progress_cb_t(int step, int steps, float time, void* data);

    #endregion

    #region DLL-Import

    internal struct sd_ctx_t;
    internal struct upscaler_ctx_t;

    [StructLayout(LayoutKind.Sequential)]
    internal struct sd_image_t
    {
        internal uint width;
        internal uint height;
        internal uint channel;
        internal byte* data;
    }

    [LibraryImport(LIB_NAME, EntryPoint = "get_num_physical_cores")]
    internal static partial int get_num_physical_cores();

    [LibraryImport(LIB_NAME, EntryPoint = "sd_get_system_info")]
    internal static partial void* sd_get_system_info();

    [LibraryImport(LIB_NAME, EntryPoint = "new_sd_ctx")]
    internal static partial sd_ctx_t* new_sd_ctx([MarshalAs(UnmanagedType.LPStr)] string model_path,
                                                 [MarshalAs(UnmanagedType.LPStr)] string vae_path,
                                                 [MarshalAs(UnmanagedType.LPStr)] string taesd_path,
                                                 [MarshalAs(UnmanagedType.LPStr)] string control_net_path_c_str,
                                                 [MarshalAs(UnmanagedType.LPStr)] string lora_model_dir,
                                                 [MarshalAs(UnmanagedType.LPStr)] string embed_dir_c_str,
                                                 [MarshalAs(UnmanagedType.LPStr)] string stacked_id_embed_dir_c_str,
                                                 [MarshalAs(UnmanagedType.I1)] bool vae_decode_only,
                                                 [MarshalAs(UnmanagedType.I1)] bool vae_tiling,
                                                 [MarshalAs(UnmanagedType.I1)] bool free_params_immediately,
                                                 int n_threads,
                                                 sd_type_t wtype,
                                                 rng_type_t rng_type,
                                                 schedule_t s,
                                                 [MarshalAs(UnmanagedType.I1)] bool keep_clip_on_cpu,
                                                 [MarshalAs(UnmanagedType.I1)] bool keep_control_net_cpu,
                                                 [MarshalAs(UnmanagedType.I1)] bool keep_vae_on_cpu);

    [LibraryImport(LIB_NAME, EntryPoint = "free_sd_ctx")]
    internal static partial void free_sd_ctx(sd_ctx_t* sd_ctx);

    [LibraryImport(LIB_NAME, EntryPoint = "txt2img")]
    internal static partial sd_image_t* txt2img(sd_ctx_t* sd_ctx,
                                                [MarshalAs(UnmanagedType.LPStr)] string prompt,
                                                [MarshalAs(UnmanagedType.LPStr)] string negative_prompt,
                                                int clip_skip,
                                                float cfg_scale,
                                                int width,
                                                int height,
                                                sample_method_t sample_method,
                                                int sample_steps,
                                                long seed,
                                                int batch_count,
                                                sd_image_t* control_cond,
                                                float control_strength,
                                                float style_strength,
                                                [MarshalAs(UnmanagedType.I1)] bool normalize_input,
                                                [MarshalAs(UnmanagedType.LPStr)] string input_id_images_path);

    [LibraryImport(LIB_NAME, EntryPoint = "img2img")]
    internal static partial sd_image_t* img2img(sd_ctx_t* sd_ctx,
                                                sd_image_t init_image,
                                                [MarshalAs(UnmanagedType.LPStr)] string prompt,
                                                [MarshalAs(UnmanagedType.LPStr)] string negative_prompt,
                                                int clip_skip,
                                                float cfg_scale,
                                                int width,
                                                int height,
                                                sample_method_t sample_method,
                                                int sample_steps,
                                                float strength,
                                                long seed,
                                                int batch_count);

    [LibraryImport(LIB_NAME, EntryPoint = "img2vid")]
    internal static partial sd_image_t* img2vid(sd_ctx_t* sd_ctx,
                                                sd_image_t init_image,
                                                int width,
                                                int height,
                                                int video_frames,
                                                int motion_bucket_id,
                                                int fps,
                                                float augmentation_level,
                                                float min_cfg,
                                                float cfg_scale,
                                                sample_method_t sample_method,
                                                int sample_steps,
                                                float strength,
                                                long seed);

    [LibraryImport(LIB_NAME, EntryPoint = "new_upscaler_ctx")]
    internal static partial upscaler_ctx_t* new_upscaler_ctx([MarshalAs(UnmanagedType.LPStr)] string esrgan_path,
                                                             int n_threads,
                                                             sd_type_t wtype);

    [LibraryImport(LIB_NAME, EntryPoint = "free_upscaler_ctx")]
    internal static partial void free_upscaler_ctx(upscaler_ctx_t* upscaler_ctx);

    [LibraryImport(LIB_NAME, EntryPoint = "upscale")]
    internal static partial sd_image_t upscale(upscaler_ctx_t* upscaler_ctx,
                                               sd_image_t input_image,
                                               int upscale_factor);

    [LibraryImport(LIB_NAME, EntryPoint = "convert")]
    internal static partial void convert([MarshalAs(UnmanagedType.LPStr)] string input_path,
                                         [MarshalAs(UnmanagedType.LPStr)] string vae_path,
                                         [MarshalAs(UnmanagedType.LPStr)] string output_path,
                                         sd_type_t output_type);

    [LibraryImport(LIB_NAME, EntryPoint = "preprocess_canny")]
    internal static partial byte* preprocess_canny(byte* img,
                                                   int width,
                                                   int height,
                                                   float high_threshold,
                                                   float low_threshold,
                                                   float weak,
                                                   float strong,
                                                   [MarshalAs(UnmanagedType.I1)] bool inverse);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_set_log_callback")]
    internal static partial void sd_set_log_callback(sd_log_cb_t sd_log_cb, void* data);

    [LibraryImport(LIB_NAME, EntryPoint = "sd_set_progress_callback")]
    internal static partial void sd_set_progress_callback(sd_progress_cb_t cb, void* data);

    #endregion
}