using HPPH;
using JetBrains.Annotations;
using System;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed unsafe class UpscaleModel : IDisposable
{
    #region Properties & Fields

    private bool _disposed;

    public UpscaleModelParameter ModelParameter { get; }

    private Native.upscaler_ctx_t* _ctx;

    #endregion

    #region Constructors

    public UpscaleModel(UpscaleModelParameter modelParameter)
    {
        ArgumentNullException.ThrowIfNull(modelParameter, nameof(modelParameter));

        modelParameter.Validate();

        this.ModelParameter = modelParameter;

        Initialize();
    }

    ~UpscaleModel() => Dispose();

    #endregion

    #region Methods

    private void Initialize()
    {
        _ctx = Native.new_upscaler_ctx(ModelParameter.ModelPath,
                                       ModelParameter.ThreadCount,
                                       ModelParameter.Quantization);

        if (_ctx == null) throw new NullReferenceException("Failed to initialize upscale-model.");
    }

    public IImage<ColorRGB> Upscale(IImage image, int upscaleFactor)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(upscaleFactor, 0, nameof(upscaleFactor));

        if (_ctx == null) throw new NullReferenceException("The model is not initialized.");

        if (image is not IImage<ColorRGB> sourceImage)
            sourceImage = image.ConvertTo<ColorRGB>();

        fixed (byte* imagePtr = sourceImage.AsRefImage())
        {
            Native.sd_image_t result = Native.upscale(_ctx, sourceImage.ToSdImage(imagePtr), upscaleFactor);
            return ImageHelper.ToImage(&result);
        }
    }

    private IImage<ColorRGB> Upscale(Native.sd_image_t image, int upscaleFactor)
    {
        Native.sd_image_t result = Native.upscale(_ctx, image, upscaleFactor);
        return ImageHelper.ToImage(&result);
    }

    public void Dispose()
    {
        if (_disposed) return;

        if (_ctx != null)
            Native.free_upscaler_ctx(_ctx);

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    #endregion
}