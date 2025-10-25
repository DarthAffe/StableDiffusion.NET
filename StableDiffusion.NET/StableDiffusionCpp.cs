using System;
using System.Runtime.InteropServices;
using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static unsafe class StableDiffusionCpp
{
    #region Properties & Fields

    // ReSharper disable NotAccessedField.Local - They are important, the delegate can be collected if it's not stored!
    private static Native.sd_log_cb_t? _logCallback;
    private static Native.sd_progress_cb_t? _progressCallback;
    // ReSharper restore NotAccessedField.Local

    #endregion

    #region Events

    public static event EventHandler<StableDiffusionLogEventArgs>? Log;
    public static event EventHandler<StableDiffusionProgressEventArgs>? Progress;

    #endregion

    #region Methods

    public static bool LoadNativeLibrary(string libraryPath) => Native.LoadNativeLibrary(libraryPath);

    public static void InitializeEvents()
    {
        Native.sd_set_log_callback(_logCallback = OnNativeLog, null);
        Native.sd_set_progress_callback(_progressCallback = OnNativeProgress, null);
    }

    public static void Convert(string modelPath, string vaePath, Quantization quantization, string outputPath, string tensorTypeRules = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(modelPath));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(outputPath));
        ArgumentNullException.ThrowIfNull(vaePath);
        if (!Enum.IsDefined(quantization)) throw new ArgumentOutOfRangeException(nameof(quantization));

        Native.convert(modelPath, vaePath, outputPath, quantization, tensorTypeRules);
    }

    public static string GetSystemInfo() => Native.sd_get_system_info();

    public static int GetNumPhysicalCores() => Native.get_num_physical_cores();

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

    #endregion
}