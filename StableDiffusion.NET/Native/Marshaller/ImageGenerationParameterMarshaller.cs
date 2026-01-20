// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(ImageGenerationParameter), MarshalMode.ManagedToUnmanagedIn, typeof(ImageGenerationParameterMarshallerIn))]
[CustomMarshaller(typeof(ImageGenerationParameter), MarshalMode.ManagedToUnmanagedOut, typeof(ImageGenerationParameterMarshaller))]
[CustomMarshaller(typeof(ImageGenerationParameter), MarshalMode.ManagedToUnmanagedRef, typeof(ImageGenerationParameterMarshallerRef))]
internal static unsafe class ImageGenerationParameterMarshaller
{
    public static ImageGenerationParameter ConvertToManaged(Native.Types.sd_img_gen_params_t unmanaged)
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
            VaeTiling = TilingParameterMarshaller.ConvertToManaged(unmanaged.vae_tiling_params),
            Cache = CacheParameterMarshaller.ConvertToManaged(unmanaged.cache)
        };

        for (int i = 0; i < unmanaged.lora_count; i++)
        {
            Native.Types.sd_lora_t lora = unmanaged.loras[i];
            parameter.Loras.Add(new Lora
            {
                IsHighNoise = lora.is_high_noise == 1,
                Multiplier = lora.multiplier,
                Path = AnsiStringMarshaller.ConvertToManaged(lora.path) ?? string.Empty
            });
        }

        return parameter;
    }

    internal ref struct ImageGenerationParameterMarshallerIn
    {
        private SampleParameterMarshaller.SampleParameterMarshallerIn _sampleParameterMarshaller = new();
        private TilingParameterMarshaller.TilingParameterMarshallerIn _tilingParameterMarshaller = new();
        private CacheParameterMarshaller.CacheParameterMarshallerIn _cacheParameterMarshaller = new();
        private Native.Types.sd_img_gen_params_t _imgGenParams;

        private Native.Types.sd_image_t _initImage;
        private Native.Types.sd_image_t _maskImage;
        private Native.Types.sd_image_t _controlNetImage;
        private Native.Types.sd_image_t* _refImages;
        private Native.Types.sd_image_t* _pmIdImages;
        private Native.Types.sd_lora_t* _loras;

        public ImageGenerationParameterMarshallerIn() { }

        public void FromManaged(ImageGenerationParameter managed)
        {
            _sampleParameterMarshaller.FromManaged(managed.SampleParameter);
            _tilingParameterMarshaller.FromManaged(managed.VaeTiling);
            _cacheParameterMarshaller.FromManaged(managed.Cache);

            _initImage = managed.InitImage?.ToSdImage() ?? new Native.Types.sd_image_t();
            _controlNetImage = managed.ControlNet.Image?.ToSdImage() ?? new Native.Types.sd_image_t();
            _refImages = managed.RefImages == null ? null : managed.RefImages.ToSdImage();
            _pmIdImages = managed.PhotoMaker.IdImages == null ? null : managed.PhotoMaker.IdImages.ToSdImage();

            _loras = (Native.Types.sd_lora_t*)NativeMemory.Alloc((nuint)managed.Loras.Count, (nuint)Marshal.SizeOf<Native.Types.sd_lora_t>());
            for (int i = 0; i < managed.Loras.Count; i++)
            {
                Lora lora = managed.Loras[i];
                _loras[i] = new Native.Types.sd_lora_t
                {
                    is_high_noise = (sbyte)(lora.IsHighNoise ? 1 : 0),
                    multiplier = lora.Multiplier,
                    path = AnsiStringMarshaller.ConvertToUnmanaged(lora.Path)
                };
            }

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
                vae_tiling_params = _tilingParameterMarshaller.ToUnmanaged(),
                cache = _cacheParameterMarshaller.ToUnmanaged(),
                loras = _loras,
                lora_count = (uint)managed.Loras.Count,
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
            _tilingParameterMarshaller.Free();
            _cacheParameterMarshaller.Free();

            for (int i = 0; i < _imgGenParams.lora_count; i++)
                AnsiStringMarshaller.Free(_imgGenParams.loras[i].path);

            if (_loras != null)
                NativeMemory.Free(_loras);
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