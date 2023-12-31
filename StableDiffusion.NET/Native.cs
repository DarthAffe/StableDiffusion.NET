﻿#pragma warning disable CS0169 // Field is never used
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
                                                 [MarshalAs(UnmanagedType.LPStr)] string lora_model_dir,
                                                 [MarshalAs(UnmanagedType.I1)] bool vae_decode_only,
                                                 [MarshalAs(UnmanagedType.I1)] bool vae_tiling,
                                                 [MarshalAs(UnmanagedType.I1)] bool free_params_immediately,
                                                 int n_threads,
                                                 sd_type_t wtype,
                                                 rng_type_t rng_type,
                                                 schedule_t s);

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
                                                int batch_count);

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

    [LibraryImport(LIB_NAME, EntryPoint = "sd_set_log_callback")]
    internal static partial void sd_set_log_callback(sd_log_cb_t sd_log_cb, void* data);

    #endregion
}