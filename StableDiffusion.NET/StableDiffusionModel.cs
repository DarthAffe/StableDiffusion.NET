﻿using System;
using System.Runtime.InteropServices;
using HPPH;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed unsafe class StableDiffusionModel : IDisposable
{
    #region Properties & Fields

    // ReSharper disable NotAccessedField.Local - They are important, the delegate can be collected if it's not stored!
    private static readonly Native.sd_log_cb_t LOG_CALLBACK;
    private static readonly Native.sd_progress_cb_t PROGRESS_CALLBACK;
    // ReSharper restore NotAccessedField.Local

    private bool _disposed;

    private readonly string _modelPath;
    private readonly ModelParameter _parameter;
    private readonly UpscalerModelParameter? _upscalerParameter;

    private Native.sd_ctx_t* _ctx;
    private Native.upscaler_ctx_t* _upscalerCtx;

    #endregion

    #region Events

    public static event EventHandler<StableDiffusionLogEventArgs>? Log;
    public static event EventHandler<StableDiffusionProgressEventArgs>? Progress;

    #endregion

    #region Constructors

    static StableDiffusionModel()
    {
        Native.sd_set_log_callback(LOG_CALLBACK = OnNativeLog, null);
        Native.sd_set_progress_callback(PROGRESS_CALLBACK = OnNativeProgress, null);
    }

    public StableDiffusionModel(string modelPath, ModelParameter parameter, UpscalerModelParameter? upscalerParameter = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelPath, nameof(modelPath));

        parameter.Validate();
        upscalerParameter?.Validate();

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
                                 _parameter.StackedIdEmbeddingsDirectory,
                                 _parameter.VaeDecodeOnly,
                                 _parameter.VaeTiling,
                                  false,
                                 _parameter.ThreadCount,
                                 _parameter.Quantization,
                                 _parameter.RngType,
                                 _parameter.Schedule,
                                 _parameter.KeepClipOnCPU,
                                 _parameter.KeepControlNetOnCPU,
                                 _parameter.KeepVaeOnCPU);
        if (_ctx == null) throw new NullReferenceException("Failed to initialize Stable Diffusion");

        if (_upscalerParameter != null)
        {
            _upscalerCtx = Native.new_upscaler_ctx(_upscalerParameter.ESRGANPath,
                                                   _upscalerParameter.ThreadCount,
                                                   _upscalerParameter.Quantization);
            if (_upscalerCtx == null) throw new NullReferenceException("Failed to initialize Stable Diffusion");
        }
    }

    /// <summary>
    /// Manually load the native stable diffusion library.
    /// Once set, it will continue to be used for all instances.
    /// </summary>
    /// <param name="libraryPath">Path to the stable diffusion library.</param>
    /// <returns>Bool if the library loaded.</returns>
    public static bool LoadNativeLibrary(string libraryPath)
        => Native.LoadNativeLibrary(libraryPath);

    public IImage<ColorRGB> TextToImage(string prompt, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);

        parameter.Validate();

        Native.sd_image_t* result;
        if (parameter.ControlNet.IsEnabled)
        {
            fixed (byte* imagePtr = parameter.ControlNet.Image!.ToRawArray())
            {
                if (parameter.ControlNet.CannyPreprocess)
                {
                    Native.sd_image_t controlNetImage = new()
                    {
                        width = (uint)parameter.ControlNet.Image.Width,
                        height = (uint)parameter.ControlNet.Image.Height,
                        channel = (uint)parameter.ControlNet.Image.ColorFormat.BytesPerPixel,
                        data = Native.preprocess_canny(imagePtr,
                                                       parameter.Width,
                                                       parameter.Height,
                                                       parameter.ControlNet.CannyHighThreshold,
                                                       parameter.ControlNet.CannyLowThreshold,
                                                       parameter.ControlNet.CannyWeak,
                                                       parameter.ControlNet.CannyStrong,
                                                       parameter.ControlNet.CannyInverse)
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
                                            parameter.ControlNet.Strength,
                                            parameter.PhotoMaker.StyleRatio,
                                            parameter.PhotoMaker.NormalizeInput,
                                            parameter.PhotoMaker.InputIdImageDirectory);

                    Marshal.FreeHGlobal((nint)controlNetImage.data);
                }
                else
                {
                    Native.sd_image_t controlNetImage = new()
                    {
                        width = (uint)parameter.ControlNet.Image.Width,
                        height = (uint)parameter.ControlNet.Image.Height,
                        channel = (uint)parameter.ControlNet.Image.ColorFormat.BytesPerPixel,
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
                                            parameter.ControlNet.Strength,
                                            parameter.PhotoMaker.StyleRatio,
                                            parameter.PhotoMaker.NormalizeInput,
                                            parameter.PhotoMaker.InputIdImageDirectory);
                }
            }
        }
        else
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
                                    0,
                                    parameter.PhotoMaker.StyleRatio,
                                    parameter.PhotoMaker.NormalizeInput,
                                    parameter.PhotoMaker.InputIdImageDirectory);
        }

        return ImageHelper.ToImage(result);
    }

    public IImage<ColorRGB> ImageToImage(string prompt, IImage<ColorRGB> image, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);

        parameter.Validate();

        fixed (byte* imagePtr = image.AsRefImage())
            return ImageToImage(prompt, image.ToSdImage(imagePtr), parameter);
    }

    private IImage<ColorRGB> ImageToImage(string prompt, Native.sd_image_t image, StableDiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);

        parameter.Validate();

        Native.sd_image_t* result;
        if (parameter.ControlNet.IsEnabled)
        {
            fixed (byte* imagePtr = parameter.ControlNet.Image!.ToRawArray())
            {
                if (parameter.ControlNet.CannyPreprocess)
                {
                    Native.sd_image_t controlNetImage = new()
                    {
                        width = (uint)parameter.ControlNet.Image.Width,
                        height = (uint)parameter.ControlNet.Image.Height,
                        channel = (uint)parameter.ControlNet.Image.ColorFormat.BytesPerPixel,
                        data = Native.preprocess_canny(imagePtr,
                                                       parameter.Width,
                                                       parameter.Height,
                                                       parameter.ControlNet.CannyHighThreshold,
                                                       parameter.ControlNet.CannyLowThreshold,
                                                       parameter.ControlNet.CannyWeak,
                                                       parameter.ControlNet.CannyStrong,
                                                       parameter.ControlNet.CannyInverse)
                    };

                    result = Native.img2img(_ctx,
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
                                            1,
                                            &controlNetImage,
                                            parameter.ControlNet.Strength,
                                            parameter.PhotoMaker.StyleRatio,
                                            parameter.PhotoMaker.NormalizeInput,
                                            parameter.PhotoMaker.InputIdImageDirectory);

                    Marshal.FreeHGlobal((nint)controlNetImage.data);
                }
                else
                {
                    Native.sd_image_t controlNetImage = new()
                    {
                        width = (uint)parameter.ControlNet.Image.Width,
                        height = (uint)parameter.ControlNet.Image.Height,
                        channel = (uint)parameter.ControlNet.Image.ColorFormat.BytesPerPixel,
                        data = imagePtr
                    };

                    result = Native.img2img(_ctx,
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
                                            1,
                                            &controlNetImage,
                                            parameter.ControlNet.Strength,
                                            parameter.PhotoMaker.StyleRatio,
                                            parameter.PhotoMaker.NormalizeInput,
                                            parameter.PhotoMaker.InputIdImageDirectory);
                }
            }
        }
        else
        {
            result = Native.img2img(_ctx,
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
                                    1,
                                    null,
                                    0,
                                    parameter.PhotoMaker.StyleRatio,
                                    parameter.PhotoMaker.NormalizeInput,
                                    parameter.PhotoMaker.InputIdImageDirectory);
        }

        return ImageHelper.ToImage(result);
    }

    public IImage<ColorRGB> Upscale(IImage<ColorRGB> image, int upscaleFactor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(upscaleFactor, 0, nameof(upscaleFactor));

        if (_upscalerCtx == null) throw new NullReferenceException("The upscaler is not initialized.");

        fixed (byte* imagePtr = image.ConvertTo<ColorRGB>().AsRefImage())
        {
            Native.sd_image_t result = Native.upscale(_upscalerCtx, image.ToSdImage(imagePtr), upscaleFactor);
            return ImageHelper.ToImage(&result);
        }
    }

    private IImage<ColorRGB> Upscale(Native.sd_image_t image, int upscaleFactor)
    {
        Native.sd_image_t result = Native.upscale(_upscalerCtx, image, upscaleFactor);
        return ImageHelper.ToImage(&result);
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
