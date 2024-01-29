using System;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

public sealed unsafe class StableDiffusionModel : IDisposable
{
    #region Properties & Fields

    private bool _disposed;

    private readonly string _modelPath;
    private readonly ModelParameter _parameter;
    private readonly UpscalerModelParameter? _upscalerParameter;

    private Native.sd_ctx_t* _ctx;
    private Native.upscaler_ctx_t* _upscalerCtx;

    #endregion

    #region Events

    public static event EventHandler<StableDiffusionLogEventArgs>? Log;

    #endregion

    #region Constructors

    static StableDiffusionModel()
    {
        Native.sd_set_log_callback(OnNativeLog, null);
    }

    public StableDiffusionModel(string modelPath, ModelParameter parameter, UpscalerModelParameter? upscalerParameter = null)
    {
        this._modelPath = modelPath;
        this._parameter = parameter;
        this._upscalerParameter = upscalerParameter;

        Initialize();
    }

    ~StableDiffusionModel() => Dispose();

    #endregion

    #region Methods

    private void Initialize()
    {
        _ctx = Native.new_sd_ctx(_modelPath,
                                 _parameter.VaePath,
                                 _parameter.TaesdPath,
                                 _parameter.ControlNetPath,
                                 _parameter.LoraModelDir,
                                 _parameter.EmbeddingsDirectory,
                                 _parameter.VaeDecodeOnly,
                                 _parameter.VaeTiling,
                                  false,
                                 _parameter.ThreadCount,
                                 _parameter.Quantization,
                                 _parameter.RngType,
                                 _parameter.Schedule,
                                 _parameter.KeepControlNetOnCPU);
        if (_ctx == null) throw new NullReferenceException("Failed to initialize Stable Diffusion");

        if (_upscalerParameter != null)
        {
            _upscalerCtx = Native.new_upscaler_ctx(_upscalerParameter.ESRGANPath,
                                                   _upscalerParameter.ThreadCount,
                                                   _upscalerParameter.Quantization);
            if (_upscalerCtx == null) throw new NullReferenceException("Failed to initialize Stable Diffusion");
        }
    }

    public StableDiffusionImage TextToImage(string prompt, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        Native.sd_image_t* result;
        if ((parameter.ControlNetImage == null) || (parameter.ControlNetImage.Length == 0))
        {
            result = Native.txt2img(_ctx,
                                    prompt,
                                    parameter.NegativePrompt,
                                    parameter.ClipSkip,
                                    parameter.CfgScale,
                                    parameter.Width,
                                    parameter.Height,
                                    parameter.SampleMethod,
                                    parameter.SampleSteps,
                                    parameter.Seed,
                                    1,
                                    null,
                                    0);
        }
        else
        {
            fixed (byte* imagePtr = parameter.ControlNetImage)
            {
                Native.sd_image_t controlNetImage = new()
                {
                    width = (uint)parameter.Width,
                    height = (uint)parameter.Height,
                    channel = 3,
                    data = imagePtr
                };

                result = Native.txt2img(_ctx,
                                        prompt,
                                        parameter.NegativePrompt,
                                        parameter.ClipSkip,
                                        parameter.CfgScale,
                                        parameter.Width,
                                        parameter.Height,
                                        parameter.SampleMethod,
                                        parameter.SampleSteps,
                                        parameter.Seed,
                                        1,
                                        &controlNetImage,
                                        parameter.ControlNetStrength);
            }
        }

        return new StableDiffusionImage(result);
    }

    public StableDiffusionImage ImageToImage(string prompt, in ReadOnlySpan<byte> image, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        fixed (byte* imagePtr = image)
        {
            Native.sd_image_t img = new()
            {
                width = (uint)parameter.Width,
                height = (uint)parameter.Height,
                channel = 3,
                data = imagePtr
            };

            return ImageToImage(prompt, img, parameter);
        }
    }

    public StableDiffusionImage ImageToImage(string prompt, StableDiffusionImage image, StableDiffusionParameter parameter)
        => ImageToImage(prompt, *image.Image, parameter);

    private StableDiffusionImage ImageToImage(string prompt, Native.sd_image_t image, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        Native.sd_image_t* result = Native.img2img(_ctx,
                                                   image,
                                                   prompt,
                                                   parameter.NegativePrompt,
                                                   parameter.ClipSkip,
                                                   parameter.CfgScale,
                                                   parameter.Width,
                                                   parameter.Height,
                                                   parameter.SampleMethod,
                                                   parameter.SampleSteps,
                                                   parameter.Strength,
                                                   parameter.Seed,
                                                   1);


        return new StableDiffusionImage(result);
    }

    public StableDiffusionImage Upscale(StableDiffusionImage image, int upscaleFactor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(upscaleFactor, 0, nameof(upscaleFactor));

        if (_upscalerCtx == null) throw new NullReferenceException("The upscaler is not initialized.");

        return Upscale(*image.Image, upscaleFactor);
    }

    public StableDiffusionImage Upscale(in ReadOnlySpan<byte> image, int width, int height, int upscaleFactor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(upscaleFactor, 0, nameof(upscaleFactor));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(width, 0, nameof(upscaleFactor));
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(height, 0, nameof(upscaleFactor));

        if (_upscalerCtx == null) throw new NullReferenceException("The upscaler is not initialized.");

        fixed (byte* imagePtr = image)
        {
            Native.sd_image_t srcImage = new()
            {
                width = (uint)width,
                height = (uint)height,
                channel = 3,
                data = imagePtr
            };

            return Upscale(srcImage, upscaleFactor);
        }
    }

    private StableDiffusionImage Upscale(Native.sd_image_t image, int upscaleFactor)
    {
        Native.sd_image_t result = Native.upscale(_upscalerCtx, image, upscaleFactor);
        return new StableDiffusionImage(&result);
    }

    public void Dispose()
    {
        if (_disposed) return;

        Native.free_sd_ctx(_ctx);

        if (_upscalerCtx != null)
            Native.free_upscaler_ctx(_upscalerCtx);

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    public static void Convert(string modelPath, string vaePath, Quantization quantization, string outputPath)
        => Native.convert(modelPath, vaePath, outputPath, quantization);

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

    #endregion
}
