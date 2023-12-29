#pragma warning disable CS0169 // Field is never used
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable IDE0044
// ReSharper disable InconsistentNaming
// ReSharper disable ArrangeTypeMemberModifiers

using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

internal unsafe partial class Native
{
    #region Constants

    private const string LIB_NAME = "sd-abi";

    #endregion

    #region DLL-Import

    internal struct stable_diffusion_ctx;

    internal struct stable_diffusion_full_params
    {
        string negative_prompt;
        float cfg_scale;
        int width;
        int height;
        int sample_method;
        int sample_steps;
        long seed;
        int batch_count;
        float strength;
    }

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_default_params_ref")]
    internal static partial stable_diffusion_full_params* stable_diffusion_full_default_params_ref();

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_negative_prompt")]
    internal static partial void stable_diffusion_full_params_set_negative_prompt(stable_diffusion_full_params* @params, [MarshalAs(UnmanagedType.LPStr)] string negative_prompt);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_cfg_scale")]
    internal static partial void stable_diffusion_full_params_set_cfg_scale(stable_diffusion_full_params* @params, float cfg_scale);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_width")]
    internal static partial void stable_diffusion_full_params_set_width(stable_diffusion_full_params* @params, int width);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_height")]
    internal static partial void stable_diffusion_full_params_set_height(stable_diffusion_full_params* @params, int height);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_sample_method")]
    internal static partial void stable_diffusion_full_params_set_sample_method(stable_diffusion_full_params* @params, [MarshalAs(UnmanagedType.LPStr)] string sample_method);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_sample_steps")]
    internal static partial void stable_diffusion_full_params_set_sample_steps(stable_diffusion_full_params* @params, int sample_steps);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_seed")]
    internal static partial void stable_diffusion_full_params_set_seed(stable_diffusion_full_params* @params, long seed);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_batch_count")]
    internal static partial void stable_diffusion_full_params_set_batch_count(stable_diffusion_full_params* @params, int batch_count);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_full_params_set_strength")]
    internal static partial void stable_diffusion_full_params_set_strength(stable_diffusion_full_params* @params, float strength);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_init")]
    internal static partial stable_diffusion_ctx* stable_diffusion_init(int n_threads, [MarshalAs(UnmanagedType.I1)] bool vae_decode_only, [MarshalAs(UnmanagedType.LPStr)] string taesd_path, [MarshalAs(UnmanagedType.LPStr)] string esrgan_path, [MarshalAs(UnmanagedType.I1)] bool free_params_immediately, [MarshalAs(UnmanagedType.I1)] bool vae_tiling, [MarshalAs(UnmanagedType.LPStr)] string lora_model_dir, [MarshalAs(UnmanagedType.LPStr)] string rng_type);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_load_from_file")]
    [return: MarshalAs(UnmanagedType.I1)]
    internal static partial bool stable_diffusion_load_from_file(stable_diffusion_ctx* ctx, [MarshalAs(UnmanagedType.LPStr)] string file_path, [MarshalAs(UnmanagedType.LPStr)] string vae_path, [MarshalAs(UnmanagedType.LPStr)] string wtype, [MarshalAs(UnmanagedType.LPStr)] string schedule, int clip_skip);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_predict_image")]
    internal static partial byte* stable_diffusion_predict_image(stable_diffusion_ctx* ctx, stable_diffusion_full_params* @params, [MarshalAs(UnmanagedType.LPStr)] string prompt);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_image_predict_image")]
    internal static partial byte* stable_diffusion_image_predict_image(stable_diffusion_ctx* ctx, stable_diffusion_full_params* @params, byte* init_image, [MarshalAs(UnmanagedType.LPStr)] string prompt);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_set_log_level")]
    internal static partial void stable_diffusion_set_log_level([MarshalAs(UnmanagedType.LPStr)] string level);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_get_system_info")]
    [return: MarshalAs(UnmanagedType.LPStr)]
    internal static partial string stable_diffusion_get_system_info();

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_free")]
    internal static partial void stable_diffusion_free(stable_diffusion_ctx* ctx);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_free_full_params")]
    internal static partial void stable_diffusion_free_full_params(stable_diffusion_full_params* @params);

    [LibraryImport(LIB_NAME, EntryPoint = "stable_diffusion_free_buffer")]
    internal static partial void stable_diffusion_free_buffer(byte* buffer);

    #endregion
}