using HPPH;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
                                 ModelParameter.ClipGPath,
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
                                 ModelParameter.KeepVaeOnCPU,
                                 ModelParameter.FlashAttention);

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

        List<nint> ptrsToFree = [];

        try
        {
            NativeParameters nativeParameters = PrefillParameters(prompt, parameter);
            SetControlNetParameters(ref nativeParameters, parameter, ptrsToFree);

            Native.sd_image_t* result = Txt2Img(nativeParameters);

            return ImageHelper.ToImage(result);
        }
        finally
        {
            foreach (nint ptr in ptrsToFree)
                Marshal.FreeHGlobal(ptr);
        }
    }

    public IImage<ColorRGB> ImageToImage(string prompt, IImage image, DiffusionParameter? parameter = null)
    {
        parameter ??= GetDefaultParameter();

        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(image);

        parameter.Validate();

        // DarthAffe 10.08.2024: Mask needs to be a 1 channel all max value image when it's not used - I really don't like this concept as it adds unnecessary allocations, but that's how it is :(
        Span<byte> maskBuffer = new byte[image.Width * image.Height];
        maskBuffer.Fill(byte.MaxValue);

        return InternalImageToImage(prompt, image, maskBuffer, parameter);
    }

    public IImage<ColorRGB> Inpaint(string prompt, IImage image, IImage mask, DiffusionParameter? parameter = null)
    {
        parameter ??= GetDefaultParameter();

        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(mask);

        parameter.Validate();

        if (image.Width != mask.Width) throw new ArgumentException("The mask needs to have the same with as the image.", nameof(mask));
        if (image.Height != mask.Height) throw new ArgumentException("The mask needs to have the same height as the image.", nameof(mask));

        // DarthAffe 10.08.2024: HPPH does currently not support monochrome images, that's why we need to convert it here. We're going for the simple conversion as the source image is supposed to be monochrome anyway.
        Span<byte> maskBuffer = new byte[image.Width * image.Height];
        for (int y = 0; y < image.Height; y++)
            for (int x = 0; x < image.Width; x++)
            {
                IColor color = mask[x, y];
                maskBuffer[(image.Width * y) + x] = (byte)Math.Round((color.R + color.G + color.B) / 3.0);
            }

        return InternalImageToImage(prompt, image, maskBuffer, parameter);
    }

    public IImage<ColorRGB> Edit(string prompt, IImage[] refImages, DiffusionParameter? parameter = null)
    {
        parameter ??= GetDefaultParameter();

        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(refImages);

        parameter.Validate();

        List<nint> ptrsToFree = [];

        try
        {
            NativeParameters nativeParameters = PrefillParameters(prompt, parameter);
            SetControlNetParameters(ref nativeParameters, parameter, ptrsToFree);

            Native.sd_image_t[] nativeRefImages = new Native.sd_image_t[refImages.Length];

            for (int i = 0; i < refImages.Length; i++)
            {
                IImage image = refImages[i];
                if (image is not IImage<ColorRGB> refImage)
                    refImage = image.ConvertTo<ColorRGB>();

                nativeRefImages[i] = refImage.ToSdImage(out nint dataPtr);
                ptrsToFree.Add(dataPtr);
            }

            fixed (Native.sd_image_t* nativeRefImagesPtr = nativeRefImages)
            {
                nativeParameters.ref_images = nativeRefImagesPtr;
                nativeParameters.ref_images_count = nativeRefImages.Length;

                Native.sd_image_t* result = Edit(nativeParameters);

                return ImageHelper.ToImage(result);
            }
        }
        finally
        {
            foreach (nint ptr in ptrsToFree)
                Marshal.FreeHGlobal(ptr);
        }
    }

    private Image<ColorRGB> InternalImageToImage(string prompt, IImage image, Span<byte> mask, DiffusionParameter parameter)
    {
        List<nint> ptrsToFree = [];

        try
        {
            NativeParameters nativeParameters = PrefillParameters(prompt, parameter);
            SetControlNetParameters(ref nativeParameters, parameter, ptrsToFree);

            if (image is not IImage<ColorRGB> refImage)
                refImage = image.ConvertTo<ColorRGB>();

            nativeParameters.init_image = refImage.ToSdImage(out nint imagePtr);
            ptrsToFree.Add(imagePtr);

            fixed (byte* maskPtr = mask)
            {
                Native.sd_image_t maskImage = new()
                {
                    width = (uint)refImage.Width,
                    height = (uint)refImage.Height,
                    channel = 1,
                    data = maskPtr
                };
                nativeParameters.mask_image = maskImage;

                Native.sd_image_t* result = Img2Img(nativeParameters);

                return ImageHelper.ToImage(result);
            }
        }
        finally
        {
            foreach (nint ptr in ptrsToFree)
                Marshal.FreeHGlobal(ptr);
        }
    }

    private static NativeParameters PrefillParameters(string prompt, DiffusionParameter parameter)
        => new()
        {
            prompt = prompt,
            negative_prompt = parameter.NegativePrompt,
            clip_skip = parameter.ClipSkip,
            cfg_scale = parameter.CfgScale,
            guidance = parameter.Guidance,
            eta = parameter.Eta,
            width = parameter.Width,
            height = parameter.Height,
            sample_method = parameter.SampleMethod,
            sample_steps = parameter.SampleSteps,
            seed = parameter.Seed,
            batch_count = 1,
            control_cond = null,
            control_strength = 0,
            style_strength = parameter.PhotoMaker.StyleRatio,
            normalize_input = parameter.PhotoMaker.NormalizeInput,
            input_id_images_path = parameter.PhotoMaker.InputIdImageDirectory,
            skip_layers = parameter.SkipLayers,
            skip_layers_count = parameter.SkipLayers.Length,
            slg_scale = parameter.SlgScale,
            skip_layer_start = parameter.SkipLayerStart,
            skip_layer_end = parameter.SkipLayerEnd,
            strength = parameter.Strength,
        };

    private static void SetControlNetParameters(ref NativeParameters nativeParameters, DiffusionParameter parameter, List<nint> ptrsToFree)
    {
        if (!parameter.ControlNet.IsEnabled) return;
        if (parameter.ControlNet.Image == null) return;

        if (parameter.ControlNet.Image is not IImage<ColorRGB> controlNetImage)
            controlNetImage = parameter.ControlNet.Image!.ConvertTo<ColorRGB>();

        Native.sd_image_t* nativeControlNetImage = controlNetImage.ToSdImagePtr(out nint controlNetImagePtr);
        ptrsToFree.Add(controlNetImagePtr);
        ptrsToFree.Add((nint)nativeControlNetImage);

        nativeParameters.control_cond = nativeControlNetImage;
        nativeParameters.control_strength = parameter.ControlNet.Strength;

        if (parameter.ControlNet.CannyPreprocess)
        {
            nativeParameters.control_cond->data = Native.preprocess_canny(nativeParameters.control_cond->data,
                parameter.Width,
                parameter.Height,
                parameter.ControlNet.CannyHighThreshold,
                parameter.ControlNet.CannyLowThreshold,
                parameter.ControlNet.CannyWeak,
                parameter.ControlNet.CannyStrong,
                parameter.ControlNet.CannyInverse);
            ptrsToFree.Add((nint)nativeParameters.control_cond->data);
        }
    }

    private Native.sd_image_t* Txt2Img(NativeParameters parameter)
        => Native.txt2img(_ctx,
                          parameter.prompt,
                          parameter.negative_prompt,
                          parameter.clip_skip,
                          parameter.cfg_scale,
                          parameter.guidance,
                          parameter.eta,
                          parameter.width,
                          parameter.height,
                          parameter.sample_method,
                          parameter.sample_steps,
                          parameter.seed,
                          parameter.batch_count,
                          parameter.control_cond,
                          parameter.control_strength,
                          parameter.style_strength,
                          parameter.normalize_input,
                          parameter.input_id_images_path,
                          parameter.skip_layers,
                          parameter.skip_layers_count,
                          parameter.slg_scale,
                          parameter.skip_layer_start,
                          parameter.skip_layer_end
        );

    private Native.sd_image_t* Img2Img(NativeParameters parameter)
        => Native.img2img(_ctx,
                          parameter.init_image,
                          parameter.mask_image,
                          parameter.prompt,
                          parameter.negative_prompt,
                          parameter.clip_skip,
                          parameter.cfg_scale,
                          parameter.guidance,
                          parameter.width,
                          parameter.height,
                          parameter.sample_method,
                          parameter.sample_steps,
                          parameter.strength,
                          parameter.seed,
                          parameter.batch_count,
                          parameter.control_cond,
                          parameter.control_strength,
                          parameter.style_strength,
                          parameter.normalize_input,
                          parameter.input_id_images_path,
                          parameter.skip_layers,
                          parameter.skip_layers_count,
                          parameter.slg_scale,
                          parameter.skip_layer_start,
                          parameter.skip_layer_end
        );

    private Native.sd_image_t* Edit(NativeParameters parameter)
        => Native.edit(_ctx,
                       parameter.ref_images,
                       parameter.ref_images_count,
                       parameter.prompt,
                       parameter.negative_prompt,
                       parameter.clip_skip,
                       parameter.cfg_scale,
                       parameter.guidance,
                       parameter.eta,
                       parameter.width,
                       parameter.height,
                       parameter.sample_method,
                       parameter.sample_steps,
                       parameter.strength,
                       parameter.seed,
                       parameter.batch_count,
                       parameter.control_cond,
                       parameter.control_strength,
                       parameter.style_strength,
                       parameter.normalize_input,
                       parameter.skip_layers,
                       parameter.skip_layers_count,
                       parameter.slg_scale,
                       parameter.skip_layer_start,
                       parameter.skip_layer_end
        );

    public void Dispose()
    {
        if (_disposed) return;

        if (_ctx != null)
            Native.free_sd_ctx(_ctx);

        GC.SuppressFinalize(this);
        _disposed = true;
    }

    #endregion

    private ref struct NativeParameters
    {
        internal string prompt;
        internal string negative_prompt;
        internal int clip_skip;
        internal float cfg_scale;
        internal float guidance;
        internal float eta;
        internal int width;
        internal int height;
        internal Sampler sample_method;
        internal int sample_steps;
        internal long seed;
        internal int batch_count;
        internal Native.sd_image_t* control_cond;
        internal float control_strength;
        internal float style_strength;
        internal bool normalize_input;
        internal string input_id_images_path;
        internal int[] skip_layers;
        internal int skip_layers_count;
        internal float slg_scale;
        internal float skip_layer_start;
        internal float skip_layer_end;

        internal Native.sd_image_t init_image;
        internal Native.sd_image_t mask_image;

        internal Native.sd_image_t* ref_images;
        internal int ref_images_count;
        internal float strength;
    }
}