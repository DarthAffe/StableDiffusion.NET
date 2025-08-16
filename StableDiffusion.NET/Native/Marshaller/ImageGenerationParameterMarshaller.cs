// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(ImageGenerationParameter), MarshalMode.ManagedToUnmanagedIn, typeof(ImageGenerationParameterMarshallerIn))]
[CustomMarshaller(typeof(ImageGenerationParameter), MarshalMode.ManagedToUnmanagedOut, typeof(ImageGenerationParameterMarshaller))]
[CustomMarshaller(typeof(ImageGenerationParameter), MarshalMode.ManagedToUnmanagedRef, typeof(ImageGenerationParameterMarshallerRef))]
internal static class ImageGenerationParameterMarshaller
{
    public static unsafe ImageGenerationParameter ConvertToManaged(Native.Types.sd_img_gen_params_t unmanaged)
    {
        ImageGenerationParameter parameter = new()
        {
            Prompt = AnsiStringMarshaller.ConvertToManaged(unmanaged.prompt) ?? string.Empty,
            NegativePrompt = AnsiStringMarshaller.ConvertToManaged(unmanaged.negative_prompt) ?? string.Empty,
            ClipSkip = unmanaged.clip_skip,
            Guidance =
            {
                TxtCfg  = unmanaged.guidance.txt_cfg,
                ImgCfg = unmanaged.guidance.img_cfg,
                MinCfg = unmanaged.guidance.min_cfg,
                DistilledGuidance = unmanaged.guidance.distilled_guidance,
                Slg =
                {
                    Layers = new int[unmanaged.guidance.slg.layer_count],
                    SkipLayerStart = unmanaged.guidance.slg.layer_start,
                    SkipLayerEnd = unmanaged.guidance.slg.layer_end,
                    Scale = unmanaged.guidance.slg.scale
                }
            },
            InitImage = unmanaged.init_image.data == null ? null : unmanaged.init_image.ToImage(),
            RefImages = unmanaged.ref_images == null ? null : ImageHelper.ToImageArrayIFace(unmanaged.ref_images, unmanaged.ref_images_count),
            MaskImage = unmanaged.mask_image.data == null ? null : unmanaged.mask_image.ToImage(),
            Width = unmanaged.width,
            Height = unmanaged.height,
            SampleMethod = unmanaged.sample_method,
            SampleSteps = unmanaged.sample_steps,
            Eta = unmanaged.eta,
            Strength = unmanaged.strength,
            Seed = unmanaged.seed,
            ControlNet =
            {
                Image = unmanaged.control_cond == null ? null : ImageHelper.GetImage(unmanaged.control_cond, 0),
                Strength = unmanaged.control_strength,
            },
            PhotoMaker =
            {
                StyleStrength = unmanaged.style_strength,
                NormalizeInput = unmanaged.normalize_input == 1,
                InputIdImageDirectory = AnsiStringMarshaller.ConvertToManaged(unmanaged.input_id_images_path) ?? string.Empty,
            }
        };

        if (unmanaged.guidance.slg.layers != null)
            new Span<int>(unmanaged.guidance.slg.layers, (int)unmanaged.guidance.slg.layer_count).CopyTo(parameter.Guidance.Slg.Layers);

        return parameter;
    }

    public static unsafe void Free(Native.Types.sd_img_gen_params_t unmanaged)
    {
        AnsiStringMarshaller.Free(unmanaged.prompt);
        AnsiStringMarshaller.Free(unmanaged.negative_prompt);
        AnsiStringMarshaller.Free(unmanaged.input_id_images_path);

        unmanaged.init_image.Free();
        unmanaged.mask_image.Free();

        if (unmanaged.ref_images != null)
            ImageHelper.Free(unmanaged.ref_images, unmanaged.ref_images_count);

        if (unmanaged.control_cond != null)
            ImageHelper.Free(unmanaged.control_cond, 1);

        if (unmanaged.guidance.slg.layers != null)
            NativeMemory.Free(unmanaged.guidance.slg.layers);
    }

    internal unsafe ref struct ImageGenerationParameterMarshallerIn
    {
        private Native.Types.sd_img_gen_params_t _imgGenParams;

        private Native.Types.sd_image_t _initImage;
        private Native.Types.sd_image_t _maskImage;
        private Native.Types.sd_image_t* _refImages;
        private Native.Types.sd_image_t* _controlNetImage;
        private int* _slgLayers;

        public void FromManaged(ImageGenerationParameter managed)
        {
            _initImage = managed.InitImage?.ToSdImage() ?? new Native.Types.sd_image_t();
            _refImages = managed.RefImages == null ? null : managed.RefImages.ToSdImage();
            _controlNetImage = managed.ControlNet.Image == null ? null : managed.ControlNet.Image.ToSdImagePtr();

            _slgLayers = (int*)NativeMemory.Alloc((nuint)managed.Guidance.Slg.Layers.Length, (nuint)Marshal.SizeOf<int>());
            managed.Guidance.Slg.Layers.AsSpan().CopyTo(new Span<int>(_slgLayers, managed.Guidance.Slg.Layers.Length));

            if (managed.MaskImage != null)
                _maskImage = managed.MaskImage.ToSdImage(true);
            else if (managed.InitImage != null)
            {
                // DarthAffe 16.08.2025: Mask needs to be a 1 channel all max value image when it's not used - I really don't like this concept as it adds unnecessary allocations, but that's how it is :(
                uint maskImageByteSize = _initImage.width * _initImage.height;
                _maskImage = new Native.Types.sd_image_t
                {
                    width = _initImage.width,
                    height = _initImage.height,
                    channel = 1,
                    data = (byte*)NativeMemory.Alloc(maskImageByteSize)
                };
                new Span<byte>(_maskImage.data, (int)maskImageByteSize).Fill(byte.MaxValue);
            }

            Native.Types.sd_slg_params_t slg = new()
            {
                layers = _slgLayers,
                layer_count = (uint)managed.Guidance.Slg.Layers.Length,
                layer_start = managed.Guidance.Slg.SkipLayerStart,
                layer_end = managed.Guidance.Slg.SkipLayerEnd,
                scale = managed.Guidance.Slg.Scale,
            };

            Native.Types.sd_guidance_params_t guidance = new()
            {
                txt_cfg = managed.Guidance.TxtCfg,
                img_cfg = managed.Guidance.ImgCfg,
                min_cfg = managed.Guidance.MinCfg,
                distilled_guidance = managed.Guidance.DistilledGuidance,
                slg = slg
            };

            _imgGenParams = new Native.Types.sd_img_gen_params_t
            {
                prompt = AnsiStringMarshaller.ConvertToUnmanaged(managed.Prompt),
                negative_prompt = AnsiStringMarshaller.ConvertToUnmanaged(managed.NegativePrompt),
                clip_skip = managed.ClipSkip,
                guidance = guidance,
                init_image = _initImage,
                ref_images = _refImages,
                ref_images_count = managed.RefImages?.Length ?? 0,
                mask_image = _maskImage,
                width = managed.Width,
                height = managed.Height,
                sample_method = managed.SampleMethod,
                sample_steps = managed.SampleSteps,
                eta = managed.Eta,
                strength = managed.Strength,
                seed = managed.Seed,
                batch_count = 1,
                control_cond = _controlNetImage,
                control_strength = managed.ControlNet.Strength,
                style_strength = managed.PhotoMaker.StyleStrength,
                normalize_input = (sbyte)(managed.PhotoMaker.NormalizeInput ? 1 : 0),
                input_id_images_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.PhotoMaker.InputIdImageDirectory),
            };
        }

        public Native.Types.sd_img_gen_params_t ToUnmanaged() => _imgGenParams;

        public void Free()
        {
            AnsiStringMarshaller.Free(_imgGenParams.prompt);
            AnsiStringMarshaller.Free(_imgGenParams.negative_prompt);
            AnsiStringMarshaller.Free(_imgGenParams.input_id_images_path);

            _initImage.Free();
            _maskImage.Free();

            if (_refImages != null)
                ImageHelper.Free(_refImages, _imgGenParams.ref_images_count);

            if (_controlNetImage != null)
                ImageHelper.Free(_controlNetImage, 1);

            if (_slgLayers != null)
                NativeMemory.Free(_slgLayers);
        }
    }

    internal ref struct ImageGenerationParameterMarshallerRef()
    {
        private ImageGenerationParameterMarshallerIn _inMarshaller = new();
        private ImageGenerationParameter _parameter;

        public void FromManaged(ImageGenerationParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_img_gen_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_img_gen_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public ImageGenerationParameter ToManaged() => _parameter;

        public void Free() => _inMarshaller.Free();
    }
}