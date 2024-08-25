using System;
using HPPH;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed unsafe class DiffusionModel : IDisposable
{
    #region Properties & Fields

    private bool _disposed;

    private readonly DiffusionModelParameter _parameter;

    private Native.sd_ctx_t* _ctx;

    #endregion

    #region Constructors

    public DiffusionModel(DiffusionModelParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));

        parameter.Validate();

        this._parameter = parameter;

        Initialize();
    }

    ~DiffusionModel() => Dispose();

    #endregion

    #region Methods

    private void Initialize()
    {
        _ctx = Native.new_sd_ctx(_parameter.ModelPath,
                                 _parameter.ClipLPath,
                                 _parameter.T5xxlPath,
                                 _parameter.DiffusionModelPath,
                                 _parameter.VaePath,
                                 _parameter.TaesdPath,
                                 _parameter.ControlNetPath,
                                 _parameter.LoraModelDirectory,
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

        if (_ctx == null) throw new NullReferenceException("Failed to initialize diffusion-model.");
    }

    public DiffusionParameter GetDefaultParameter() => _parameter.DiffusionModelType switch
    {
        DiffusionModelType.None => new DiffusionParameter(),
        DiffusionModelType.StableDiffusion => DiffusionParameter.StableDiffusionDefault,
        DiffusionModelType.Flux => DiffusionParameter.FluxDefault,
        _ => throw new ArgumentOutOfRangeException()
    };

    public IImage<ColorRGB> TextToImage(string prompt, DiffusionParameter? parameter = null)
    {
        parameter ??= GetDefaultParameter();

        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);

        parameter.Validate();

        Native.sd_image_t* result;
        if (parameter.ControlNet.IsEnabled)
        {
            if (parameter.ControlNet.Image is not IImage<ColorRGB> controlNetImage)
                controlNetImage = parameter.ControlNet.Image!.ConvertTo<ColorRGB>();

            fixed (byte* imagePtr = controlNetImage.ToRawArray())
            {
                if (parameter.ControlNet.CannyPreprocess)
                {
                    Native.sd_image_t nativeControlNetImage = new()
                    {
                        width = (uint)controlNetImage.Width,
                        height = (uint)controlNetImage.Height,
                        channel = (uint)controlNetImage.ColorFormat.BytesPerPixel,
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
                                            parameter.Guidance,
                                            parameter.Width,
                                            parameter.Height,
                                            parameter.SampleMethod,
                                            parameter.SampleSteps,
                                            parameter.Seed,
                                            1,
                                            &nativeControlNetImage,
                                            parameter.ControlNet.Strength,
                                            parameter.PhotoMaker.StyleRatio,
                                            parameter.PhotoMaker.NormalizeInput,
                                            parameter.PhotoMaker.InputIdImageDirectory);

                    Marshal.FreeHGlobal((nint)nativeControlNetImage.data);
                }
                else
                {
                    Native.sd_image_t nativeControlNetImage = new()
                    {
                        width = (uint)controlNetImage.Width,
                        height = (uint)controlNetImage.Height,
                        channel = (uint)controlNetImage.ColorFormat.BytesPerPixel,
                        data = imagePtr
                    };

                    result = Native.txt2img(_ctx,
                                            prompt,
                                            parameter.NegativePrompt,
                                            parameter.ClipSkip,
                                            parameter.CfgScale,
                                            parameter.Guidance,
                                            parameter.Width,
                                            parameter.Height,
                                            parameter.SampleMethod,
                                            parameter.SampleSteps,
                                            parameter.Seed,
                                            1,
                                            &nativeControlNetImage,
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
                                    parameter.Guidance,
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

    public IImage<ColorRGB> ImageToImage(string prompt, IImage image, DiffusionParameter? parameter = null)
    {
        parameter ??= GetDefaultParameter();

        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);

        parameter.Validate();

        if (image is not IImage<ColorRGB> refImage)
            refImage = image.ConvertTo<ColorRGB>();

        fixed (byte* imagePtr = refImage.AsRefImage())
            return ImageToImage(prompt, refImage.ToSdImage(imagePtr), parameter);
    }

    private IImage<ColorRGB> ImageToImage(string prompt, Native.sd_image_t image, DiffusionParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);

        parameter.Validate();

        Native.sd_image_t* result;
        if (parameter.ControlNet.IsEnabled)
        {
            if (parameter.ControlNet.Image is not IImage<ColorRGB> controlNetImage)
                controlNetImage = parameter.ControlNet.Image!.ConvertTo<ColorRGB>();

            fixed (byte* imagePtr = controlNetImage.ToRawArray())
            {
                if (parameter.ControlNet.CannyPreprocess)
                {
                    Native.sd_image_t nativeControlNetImage = new()
                    {
                        width = (uint)controlNetImage.Width,
                        height = (uint)controlNetImage.Height,
                        channel = (uint)controlNetImage.ColorFormat.BytesPerPixel,
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
                                            parameter.Guidance,
                                            parameter.Width,
                                            parameter.Height,
                                            parameter.SampleMethod,
                                            parameter.SampleSteps,
                                            parameter.Strength,
                                            parameter.Seed,
                                            1,
                                            &nativeControlNetImage,
                                            parameter.ControlNet.Strength,
                                            parameter.PhotoMaker.StyleRatio,
                                            parameter.PhotoMaker.NormalizeInput,
                                            parameter.PhotoMaker.InputIdImageDirectory);

                    Marshal.FreeHGlobal((nint)nativeControlNetImage.data);
                }
                else
                {
                    Native.sd_image_t nativeControlNetImage = new()
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
                                            parameter.Guidance,
                                            parameter.Width,
                                            parameter.Height,
                                            parameter.SampleMethod,
                                            parameter.SampleSteps,
                                            parameter.Strength,
                                            parameter.Seed,
                                            1,
                                            &nativeControlNetImage,
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
                                    parameter.Guidance,
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

    public void Dispose()
    {
        if (_disposed) return;

        if (_ctx != null)
            Native.free_sd_ctx(_ctx);

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    #endregion
}