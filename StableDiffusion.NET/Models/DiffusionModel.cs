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

    public DiffusionModelParameter ModelParameter { get; }

    private Native.sd_ctx_t* _ctx;

    #endregion

    #region Constructors

    public DiffusionModel(DiffusionModelParameter modelParameter)
    {
        ArgumentNullException.ThrowIfNull(modelParameter, nameof(modelParameter));

        modelParameter.Validate();

        this.ModelParameter = modelParameter;

        Initialize();
    }

    ~DiffusionModel() => Dispose();

    #endregion

    #region Methods

    private void Initialize()
    {
        _ctx = Native.new_sd_ctx(ModelParameter.ModelPath,
                                 ModelParameter.ClipLPath,
                                 ModelParameter.T5xxlPath,
                                 ModelParameter.DiffusionModelPath,
                                 ModelParameter.VaePath,
                                 ModelParameter.TaesdPath,
                                 ModelParameter.ControlNetPath,
                                 ModelParameter.LoraModelDirectory,
                                 ModelParameter.EmbeddingsDirectory,
                                 ModelParameter.StackedIdEmbeddingsDirectory,
                                 ModelParameter.VaeDecodeOnly,
                                 ModelParameter.VaeTiling,
                                 false,
                                 ModelParameter.ThreadCount,
                                 ModelParameter.Quantization,
                                 ModelParameter.RngType,
                                 ModelParameter.Schedule,
                                 ModelParameter.KeepClipOnCPU,
                                 ModelParameter.KeepControlNetOnCPU,
                                 ModelParameter.KeepVaeOnCPU);

        if (_ctx == null) throw new NullReferenceException("Failed to initialize diffusion-model.");
    }

    public DiffusionParameter GetDefaultParameter() => ModelParameter.DiffusionModelType switch
    {
        DiffusionModelType.None => new DiffusionParameter(),
        DiffusionModelType.StableDiffusion => DiffusionParameter.SDXLDefault,
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