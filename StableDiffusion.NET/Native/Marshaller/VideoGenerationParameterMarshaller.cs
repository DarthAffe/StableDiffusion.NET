// ReSharper disable MemberCanBeMadeStatic.Global

using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(VideoGenerationParameter), MarshalMode.ManagedToUnmanagedIn, typeof(VideoGenerationParameterMarshallerIn))]
[CustomMarshaller(typeof(VideoGenerationParameter), MarshalMode.ManagedToUnmanagedOut, typeof(VideoGenerationParameterMarshaller))]
[CustomMarshaller(typeof(VideoGenerationParameter), MarshalMode.ManagedToUnmanagedRef, typeof(VideoGenerationParameterMarshallerRef))]
internal static class VideoGenerationParameterMarshaller
{
    public static unsafe VideoGenerationParameter ConvertToManaged(Native.Types.sd_vid_gen_params_t unmanaged)
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

        return parameter;
    }

    public static unsafe void Free(Native.Types.sd_vid_gen_params_t unmanaged)
    {
        AnsiStringMarshaller.Free(unmanaged.prompt);
        AnsiStringMarshaller.Free(unmanaged.negative_prompt);

        unmanaged.init_image.Free();
        unmanaged.end_image.Free();

        if (unmanaged.control_frames != null)
            ImageHelper.Free(unmanaged.control_frames, unmanaged.control_frames_size);

        SampleParameterMarshaller.Free(unmanaged.sample_params);
        SampleParameterMarshaller.Free(unmanaged.high_noise_sample_params);
    }

    internal unsafe ref struct VideoGenerationParameterMarshallerIn
    {
        private SampleParameterMarshaller.SampleParameterMarshallerIn _sampleParameterMarshaller = new();
        private SampleParameterMarshaller.SampleParameterMarshallerIn _highNoiseSampleParameterMarshaller = new();
        private Native.Types.sd_vid_gen_params_t _vidGenParams;

        private Native.Types.sd_image_t _initImage;
        private Native.Types.sd_image_t _endImage;
        private Native.Types.sd_image_t* _controlFrames;

        public VideoGenerationParameterMarshallerIn() { }

        public void FromManaged(VideoGenerationParameter managed)
        {
            _sampleParameterMarshaller.FromManaged(managed.SampleParameter);
            _highNoiseSampleParameterMarshaller.FromManaged(managed.HighNoiseSampleParameter);

            _initImage = managed.InitImage?.ToSdImage() ?? new Native.Types.sd_image_t();
            _endImage = managed.EndImage?.ToSdImage() ?? new Native.Types.sd_image_t();
            _controlFrames = managed.ControlFrames == null ? null : managed.ControlFrames.ToSdImage();
            
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