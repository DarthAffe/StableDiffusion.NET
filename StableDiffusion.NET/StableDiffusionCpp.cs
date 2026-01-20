using HPPH;
using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[PublicAPI]
public static unsafe class StableDiffusionCpp
{
    #region Properties & Fields

    // ReSharper disable NotAccessedField.Local - They are important, the delegate can be collected if it's not stored!
    private static Native.sd_log_cb_t? _logCallback;
    private static Native.sd_progress_cb_t? _progressCallback;
    private static Native.sd_preview_cb_t? _previewCallback;
    // ReSharper restore NotAccessedField.Local

    public static string ExpectedSDCommit => "b87fe13";

    #endregion

    #region Events

    public static event EventHandler<StableDiffusionLogEventArgs>? Log;
    public static event EventHandler<StableDiffusionProgressEventArgs>? Progress;
    public static event EventHandler<StableDiffusionPreviewEventArgs>? Preview;

    #endregion

    #region Methods

    public static bool LoadNativeLibrary(string libraryPath) => Native.LoadNativeLibrary(libraryPath);

    public static void InitializeEvents()
    {
        Native.sd_set_log_callback(_logCallback = OnNativeLog, null);
        Native.sd_set_progress_callback(_progressCallback = OnNativeProgress, null);
    }

    public static void EnablePreview(Preview mode, int interval, bool denoised, bool noisy)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(interval);

        if (mode == NET.Preview.None)
            _previewCallback = null;

        else if (_previewCallback == null)
            _previewCallback = OnPreview;

        Native.sd_set_preview_callback(_previewCallback, mode, interval, denoised, noisy, null);
    }

    public static void Convert(string modelPath, string vaePath, Quantization quantization, string outputPath, bool convertName = false, string tensorTypeRules = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(modelPath));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(outputPath));
        ArgumentNullException.ThrowIfNull(vaePath);
        if (!Enum.IsDefined(quantization)) throw new ArgumentOutOfRangeException(nameof(quantization));

        Native.convert(modelPath, vaePath, outputPath, quantization, tensorTypeRules, convertName);
    }

    public static string GetSystemInfo() => Native.sd_get_system_info();

    public static int GetNumPhysicalCores() => Native.sd_get_num_physical_cores();

    public static string GetSDCommit() => AnsiStringMarshaller.ConvertToManaged(Native.sd_commit()) ?? string.Empty;

    public static string GetSDVersion() => AnsiStringMarshaller.ConvertToManaged(Native.sd_version()) ?? string.Empty;

    public static Image<ColorRGB> PreprocessCanny(CannyParameter parameter)
    {
        parameter.Validate();

        IImage<ColorRGB> controlImage = parameter.Image as IImage<ColorRGB> ?? parameter.Image!.ConvertTo<ColorRGB>();

        Native.Types.sd_image_t sdImage = controlImage.ToSdImage();
        try
        {
            bool result = Native.preprocess_canny(sdImage,
                                                  parameter.HighThreshold,
                                                  parameter.LowThreshold,
                                                  parameter.Weak,
                                                  parameter.Strong,
                                                  parameter.Inverse);

            return sdImage.ToImage();
        }
        finally
        {
            sdImage.Free();
        }
    }

    private static void OnNativeLog(LogLevel level, string text, void* data)
    {
        try
        {
            Log?.Invoke(null, new StableDiffusionLogEventArgs(level, text));
        }
        catch { /**/ }
    }

    private static void OnNativeProgress(int step, int steps, float time, void* data)
    {
        try
        {
            Progress?.Invoke(null, new StableDiffusionProgressEventArgs(step, steps, time));
        }
        catch { /**/ }
    }

    private static void OnPreview(int step, int frameCount, Native.Types.sd_image_t* frames, bool isNoisy, void* data)
    {
        try
        {
            if (frameCount <= 0 || frames == null) return;

            Image<ColorRGB> image = ImageHelper.GetImage(frames, 0);

            Preview?.Invoke(null, new StableDiffusionPreviewEventArgs(step, isNoisy, image));
        }
        catch { /**/ }
    }

    #endregion
}