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
            SampleParameter = SampleParameterMarshaller.ConvertToManaged(unmanaged.sample_params),
            InitImage = unmanaged.init_image.data == null ? null : unmanaged.init_image.ToImage(),
            RefImages = unmanaged.ref_images == null ? null : ImageHelper.ToImageArrayIFace(unmanaged.ref_images, unmanaged.ref_images_count),
            AutoResizeRefImage = unmanaged.auto_resize_ref_image == 1,
            MaskImage = unmanaged.mask_image.data == null ? null : unmanaged.mask_image.ToImage(),
            Width = unmanaged.width,
            Height = unmanaged.height,
            Strength = unmanaged.strength,
            Seed = unmanaged.seed,
            ControlNet =
            {
                Image = unmanaged.control_image.ToImage(),
                Strength = unmanaged.control_strength,
            },
            PhotoMaker =
            {
                IdImages = unmanaged.pm_params.id_images == null ? null : ImageHelper.ToImageArrayIFace(unmanaged.pm_params.id_images, unmanaged.pm_params.id_images_count),
                IdEmbedPath =  AnsiStringMarshaller.ConvertToManaged(unmanaged.pm_params.id_embed_path) ?? string.Empty,
                StyleStrength = unmanaged.pm_params.style_strength,
            },
            VaeTiling =
            {
                IsEnabled = unmanaged.vae_tiling_params.enabled == 1,
                TileSizeX = unmanaged.vae_tiling_params.tile_size_x,
                TileSizeY = unmanaged.vae_tiling_params.tile_size_y,
                TargetOverlap = unmanaged.vae_tiling_params.target_overlap,
                RelSizeX = unmanaged.vae_tiling_params.rel_size_x,
                RelSizeY = unmanaged.vae_tiling_params.rel_size_y
            },
            EasyCache =
            {
                IsEnabled = unmanaged.easycache.enabled == 1,
                ReuseThreshold = unmanaged.easycache.reuse_threshold,
                StartPercent = unmanaged.easycache.start_percent,
                EndPercent = unmanaged.easycache.end_percent
            }
        };

        return parameter;
    }

    public static unsafe void Free(Native.Types.sd_img_gen_params_t unmanaged)
    {
        AnsiStringMarshaller.Free(unmanaged.prompt);
        AnsiStringMarshaller.Free(unmanaged.negative_prompt);

        unmanaged.init_image.Free();
        unmanaged.mask_image.Free();
        unmanaged.control_image.Free();

        if (unmanaged.ref_images != null)
            ImageHelper.Free(unmanaged.ref_images, unmanaged.ref_images_count);

        if (unmanaged.pm_params.id_images != null)
            ImageHelper.Free(unmanaged.pm_params.id_images, unmanaged.pm_params.id_images_count);

        SampleParameterMarshaller.Free(unmanaged.sample_params);
    }

    internal unsafe ref struct ImageGenerationParameterMarshallerIn
    {
        private SampleParameterMarshaller.SampleParameterMarshallerIn _sampleParameterMarshaller = new();
        private Native.Types.sd_img_gen_params_t _imgGenParams;

        private Native.Types.sd_image_t _initImage;
        private Native.Types.sd_image_t _maskImage;
        private Native.Types.sd_image_t _controlNetImage;
        private Native.Types.sd_image_t* _refImages;
        private Native.Types.sd_image_t* _pmIdImages;

        public ImageGenerationParameterMarshallerIn() { }

        public void FromManaged(ImageGenerationParameter managed)
        {
            _sampleParameterMarshaller.FromManaged(managed.SampleParameter);

            _initImage = managed.InitImage?.ToSdImage() ?? new Native.Types.sd_image_t();
            _controlNetImage = managed.ControlNet.Image?.ToSdImage() ?? new Native.Types.sd_image_t();
            _refImages = managed.RefImages == null ? null : managed.RefImages.ToSdImage();
            _pmIdImages = managed.PhotoMaker.IdImages == null ? null : managed.PhotoMaker.IdImages.ToSdImage();

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

            Native.Types.sd_pm_params_t photoMakerParams = new()
            {
                id_images = _pmIdImages,
                id_images_count = managed.PhotoMaker.IdImages?.Length ?? 0,
                id_embed_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.PhotoMaker.IdEmbedPath),
                style_strength = managed.PhotoMaker.StyleStrength
            };

            Native.Types.sd_tiling_params_t tilingParams = new()
            {
                enabled = (sbyte)(managed.VaeTiling.IsEnabled ? 1 : 0),
                tile_size_x = managed.VaeTiling.TileSizeX,
                tile_size_y = managed.VaeTiling.TileSizeY,
                target_overlap = managed.VaeTiling.TargetOverlap,
                rel_size_x = managed.VaeTiling.RelSizeX,
                rel_size_y = managed.VaeTiling.RelSizeY
            };

            Native.Types.sd_easycache_params_t easyCache = new()
            {
                enabled = (sbyte)(managed.EasyCache.IsEnabled ? 1 : 0),
                reuse_threshold = managed.EasyCache.ReuseThreshold,
                start_percent = managed.EasyCache.StartPercent,
                end_percent = managed.EasyCache.EndPercent,
            };

            _imgGenParams = new Native.Types.sd_img_gen_params_t
            {
                prompt = AnsiStringMarshaller.ConvertToUnmanaged(managed.Prompt),
                negative_prompt = AnsiStringMarshaller.ConvertToUnmanaged(managed.NegativePrompt),
                clip_skip = managed.ClipSkip,
                sample_params = _sampleParameterMarshaller.ToUnmanaged(),
                init_image = _initImage,
                ref_images = _refImages,
                ref_images_count = managed.RefImages?.Length ?? 0,
                auto_resize_ref_image = (sbyte)(managed.AutoResizeRefImage ? 1 : 0),
                mask_image = _maskImage,
                width = managed.Width,
                height = managed.Height,
                strength = managed.Strength,
                seed = managed.Seed,
                batch_count = 1,
                control_image = _controlNetImage,
                control_strength = managed.ControlNet.Strength,
                pm_params = photoMakerParams,
                vae_tiling_params = tilingParams,
                easycache = easyCache
            };
        }

        public Native.Types.sd_img_gen_params_t ToUnmanaged() => _imgGenParams;

        public void Free()
        {
            AnsiStringMarshaller.Free(_imgGenParams.prompt);
            AnsiStringMarshaller.Free(_imgGenParams.negative_prompt);
            AnsiStringMarshaller.Free(_imgGenParams.pm_params.id_embed_path);

            _initImage.Free();
            _maskImage.Free();
            _controlNetImage.Free();

            if (_refImages != null)
                ImageHelper.Free(_refImages, _imgGenParams.ref_images_count);

            if (_pmIdImages != null)
                ImageHelper.Free(_pmIdImages, _imgGenParams.pm_params.id_images_count);

            _sampleParameterMarshaller.Free();
        }
    }

    internal ref struct ImageGenerationParameterMarshallerRef()
    {
        private ImageGenerationParameterMarshallerIn _inMarshaller = new();
        private ImageGenerationParameter? _parameter;

        public void FromManaged(ImageGenerationParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_img_gen_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_img_gen_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public ImageGenerationParameter ToManaged() => _parameter!;

        public void Free() => _inMarshaller.Free();
    }
}