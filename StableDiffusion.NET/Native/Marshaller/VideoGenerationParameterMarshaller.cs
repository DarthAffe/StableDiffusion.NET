// ReSharper disable MemberCanBeMadeStatic.Global

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(VideoGenerationParameter), MarshalMode.ManagedToUnmanagedIn, typeof(VideoGenerationParameterMarshallerIn))]
[CustomMarshaller(typeof(VideoGenerationParameter), MarshalMode.ManagedToUnmanagedOut, typeof(VideoGenerationParameterMarshaller))]
[CustomMarshaller(typeof(VideoGenerationParameter), MarshalMode.ManagedToUnmanagedRef, typeof(VideoGenerationParameterMarshallerRef))]
internal static unsafe class VideoGenerationParameterMarshaller
{
    public static VideoGenerationParameter ConvertToManaged(Native.Types.sd_vid_gen_params_t unmanaged)
    {
        VideoGenerationParameter parameter = new()
        {
            Prompt = AnsiStringMarshaller.ConvertToManaged(unmanaged.prompt) ?? string.Empty,
            NegativePrompt = AnsiStringMarshaller.ConvertToManaged(unmanaged.negative_prompt) ?? string.Empty,
            ClipSkip = unmanaged.clip_skip,
            InitImage = unmanaged.init_image.data == null ? null : unmanaged.init_image.ToImage(),
            EndImage = unmanaged.end_image.data == null ? null : unmanaged.end_image.ToImage(),
            ControlFrames = unmanaged.control_frames == null ? null : ImageHelper.ToImageArrayIFace(unmanaged.control_frames, unmanaged.control_frames_size),
            Width = unmanaged.width,
            Height = unmanaged.height,
            SampleParameter = SampleParameterMarshaller.ConvertToManaged(unmanaged.sample_params),
            HighNoiseSampleParameter = SampleParameterMarshaller.ConvertToManaged(unmanaged.high_noise_sample_params),
            MoeBoundry = unmanaged.moe_boundary,
            Strength = unmanaged.strength,
            Seed = unmanaged.seed,
            FrameCount = unmanaged.video_frames,
            VaceStrength = unmanaged.vace_strength,
            EasyCache =
            {
                IsEnabled = unmanaged.easycache.enabled == 1,
                ReuseThreshold = unmanaged.easycache.reuse_threshold,
                StartPercent = unmanaged.easycache.start_percent,
                EndPercent = unmanaged.easycache.end_percent
            }
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

    internal ref struct VideoGenerationParameterMarshallerIn
    {
        private SampleParameterMarshaller.SampleParameterMarshallerIn _sampleParameterMarshaller = new();
        private SampleParameterMarshaller.SampleParameterMarshallerIn _highNoiseSampleParameterMarshaller = new();
        private Native.Types.sd_vid_gen_params_t _vidGenParams;

        private Native.Types.sd_image_t _initImage;
        private Native.Types.sd_image_t _endImage;
        private Native.Types.sd_image_t* _controlFrames;
        private Native.Types.sd_lora_t* _loras;

        public VideoGenerationParameterMarshallerIn() { }

        public void FromManaged(VideoGenerationParameter managed)
        {
            _sampleParameterMarshaller.FromManaged(managed.SampleParameter);
            _highNoiseSampleParameterMarshaller.FromManaged(managed.HighNoiseSampleParameter);

            _initImage = managed.InitImage?.ToSdImage() ?? new Native.Types.sd_image_t();
            _endImage = managed.EndImage?.ToSdImage() ?? new Native.Types.sd_image_t();
            _controlFrames = managed.ControlFrames == null ? null : managed.ControlFrames.ToSdImage();

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

            Native.Types.sd_easycache_params_t easyCache = new()
            {
                enabled = (sbyte)(managed.EasyCache.IsEnabled ? 1 : 0),
                reuse_threshold = managed.EasyCache.ReuseThreshold,
                start_percent = managed.EasyCache.StartPercent,
                end_percent = managed.EasyCache.EndPercent,
            };

            _vidGenParams = new Native.Types.sd_vid_gen_params_t
            {
                prompt = AnsiStringMarshaller.ConvertToUnmanaged(managed.Prompt),
                negative_prompt = AnsiStringMarshaller.ConvertToUnmanaged(managed.NegativePrompt),
                clip_skip = managed.ClipSkip,
                init_image = _initImage,
                end_image = _endImage,
                control_frames = _controlFrames,
                control_frames_size = managed.ControlFrames?.Length ?? 0,
                width = managed.Width,
                height = managed.Height,
                sample_params = _sampleParameterMarshaller.ToUnmanaged(),
                high_noise_sample_params = _highNoiseSampleParameterMarshaller.ToUnmanaged(),
                moe_boundary = managed.MoeBoundry,
                strength = managed.Strength,
                seed = managed.Seed,
                video_frames = managed.FrameCount,
                vace_strength = managed.VaceStrength,
                easycache = easyCache,
                loras = _loras,
                lora_count = (uint)managed.Loras.Count
            };
        }

        public Native.Types.sd_vid_gen_params_t ToUnmanaged() => _vidGenParams;

        public void Free()
        {
            AnsiStringMarshaller.Free(_vidGenParams.prompt);
            AnsiStringMarshaller.Free(_vidGenParams.negative_prompt);

            _initImage.Free();
            _endImage.Free();

            if (_controlFrames != null)
                ImageHelper.Free(_controlFrames, _vidGenParams.control_frames_size);

            _sampleParameterMarshaller.Free();
            _highNoiseSampleParameterMarshaller.Free();

            for (int i = 0; i < _vidGenParams.lora_count; i++)
                AnsiStringMarshaller.Free(_vidGenParams.loras[i].path);

            if (_loras != null)
                NativeMemory.Free(_loras);
        }
    }

    internal ref struct VideoGenerationParameterMarshallerRef()
    {
        private VideoGenerationParameterMarshallerIn _inMarshaller = new();
        private VideoGenerationParameter? _parameter;

        public void FromManaged(VideoGenerationParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_vid_gen_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_vid_gen_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public VideoGenerationParameter ToManaged() => _parameter!;

        public void Free() => _inMarshaller.Free();
    }
}