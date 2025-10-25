using HPPH;
using JetBrains.Annotations;
using System;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed unsafe class DiffusionModel : IDisposable
{
    #region Properties & Fields

    private bool _disposed;

    public DiffusionModelParameter ModelParameter { get; }

    private Native.Types.sd_ctx_t* _ctx;

    #endregion

    #region Constructors

    public DiffusionModel(DiffusionModelParameter modelParameter)
    {
        ArgumentNullException.ThrowIfNull(modelParameter, nameof(modelParameter));

        this.ModelParameter = modelParameter;

        Initialize();
    }

    ~DiffusionModel() => Dispose();

    #endregion

    #region Methods

    private void Initialize()
    {
        ModelParameter.Validate();

        _ctx = Native.new_sd_ctx(ModelParameter);

        if (_ctx == null) throw new NullReferenceException("Failed to initialize diffusion-model.");
    }

    public Image<ColorRGB>? GenerateImage(ImageGenerationParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_ctx == null) throw new NullReferenceException("The model is not initialized.");

        parameter.Validate();
        
        Native.Types.sd_image_t* result = Native.generate_image(_ctx, parameter);
        if (result == null) return null;

        try
        {
            return ImageHelper.GetImage(result, 0);
        }
        finally
        {
            ImageHelper.Free(result, 1);
        }
    }

    public Image<ColorRGB>[] GenerateVideo(VideoGenerationParameter parameter)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        parameter.Validate();

        Native.Types.sd_image_t* result = Native.generate_video(_ctx, parameter, out int frameCount);
        if (result == null) return [];

        try
        {
            return ImageHelper.ToImageArray(result, frameCount);
        }
        finally
        {
            ImageHelper.Free(result, frameCount);
        }
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