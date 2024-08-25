using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public static unsafe class StableDiffusionCpp
{
    #region Properties & Fields

    // ReSharper disable NotAccessedField.Local - They are important, the delegate can be collected if it's not stored!
    private static readonly Native.sd_log_cb_t LOG_CALLBACK;
    private static readonly Native.sd_progress_cb_t PROGRESS_CALLBACK;
    // ReSharper restore NotAccessedField.Local

    #endregion

    #region Events

    public static event EventHandler<StableDiffusionLogEventArgs>? Log;
    public static event EventHandler<StableDiffusionProgressEventArgs>? Progress;

    #endregion

    #region Constructors

    static StableDiffusionCpp()
    {
        Native.sd_set_log_callback(LOG_CALLBACK = OnNativeLog, null);
        Native.sd_set_progress_callback(PROGRESS_CALLBACK = OnNativeProgress, null);
    }

    #endregion

    #region Methods

    public static bool LoadNativeLibrary(string libraryPath) => Native.LoadNativeLibrary(libraryPath);

    public static void Convert(string modelPath, string vaePath, Quantization quantization, string outputPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(modelPath));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(outputPath));
        ArgumentNullException.ThrowIfNull(vaePath);
        if (!Enum.IsDefined(quantization)) throw new ArgumentOutOfRangeException(nameof(quantization));

        Native.convert(modelPath, vaePath, outputPath, quantization);
    }

    public static string GetSystemInfo()
    {
        void* s = Native.sd_get_system_info();
        return Marshal.PtrToStringUTF8((nint)s) ?? "";
    }

    public static int GetNumPhysicalCores() => Native.get_num_physical_cores();

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