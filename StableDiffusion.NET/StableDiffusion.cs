using System;
using System.IO;

namespace StableDiffusion.NET;

public sealed unsafe class StableDiffusionModel : IDisposable
{
    #region Properties & Fields

    private bool _disposed;

    private readonly string _modelPath;
    private readonly ModelParameter _parameter;

    private Native.stable_diffusion_ctx* _ctx;

    #endregion

    #region Constructors

    public StableDiffusionModel(string modelPath, ModelParameter parameter)
    {
        this._modelPath = modelPath;
        this._parameter = parameter;

        Initialize();
    }

    ~StableDiffusionModel() => Dispose();

    #endregion

    #region Methods

    private void Initialize()
    {
        _ctx = Native.stable_diffusion_init(_parameter.ThreadCount, _parameter.VaeDecodeOnly, _parameter.TaesdPath, false, _parameter.LoraModelDir, _parameter.RngType.GetNativeName() ?? "STD_DEFAULT_RNG");
        if (_ctx == null) throw new NullReferenceException("Failed to initialize Stable Diffusion");

        bool success = Native.stable_diffusion_load_from_file(_ctx, _modelPath, _parameter.VaePath, _parameter.Quantization.GetNativeName() ?? "DEFAULT", _parameter.Schedule.GetNativeName() ?? "DEFAULT");
        if (!success) throw new IOException("Failed to load model");
    }

    public Image TextToImage(string prompt, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        byte* result = Native.stable_diffusion_predict_image(_ctx, parameter.ParamPtr, prompt);
        return new Image(result, parameter.Width, parameter.Height);
    }

    public Image ImageToImage(string prompt, Span<byte> image, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        fixed (byte* imagePtr = image)
        {
            byte* result = Native.stable_diffusion_image_predict_image(_ctx, parameter.ParamPtr, imagePtr, prompt);
            return new Image(result, parameter.Width, parameter.Height);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        Native.stable_diffusion_free(_ctx);

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    public static string GetSystemInfo() => Native.stable_diffusion_get_system_info();

    public static void SetLogLevel(LogLevel level) => Native.stable_diffusion_set_log_level(level.GetNativeName() ?? "ERROR");

    #endregion
}
