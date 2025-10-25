// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(SampleParameter), MarshalMode.ManagedToUnmanagedIn, typeof(SampleParameterMarshallerIn))]
[CustomMarshaller(typeof(SampleParameter), MarshalMode.ManagedToUnmanagedOut, typeof(SampleParameterMarshaller))]
[CustomMarshaller(typeof(SampleParameter), MarshalMode.ManagedToUnmanagedRef, typeof(SampleParameterMarshallerRef))]
internal static class SampleParameterMarshaller
{
    public static unsafe SampleParameter ConvertToManaged(Native.Types.sd_sample_params_t unmanaged)
    {
        SampleParameter parameter = new()
        {
            Guidance =
            {
                TxtCfg = unmanaged.guidance.txt_cfg,
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
            Scheduler = unmanaged.scheduler,
            SampleMethod = unmanaged.sample_method,
            SampleSteps = unmanaged.sample_steps,
            Eta = unmanaged.eta,
            ShiftedTimestep = unmanaged.shifted_timestep
        };

        if (unmanaged.guidance.slg.layers != null)
            new Span<int>(unmanaged.guidance.slg.layers, (int)unmanaged.guidance.slg.layer_count).CopyTo(parameter.Guidance.Slg.Layers);

        return parameter;
    }

    public static unsafe void Free(Native.Types.sd_sample_params_t unmanaged)
    {
        if (unmanaged.guidance.slg.layers != null)
            NativeMemory.Free(unmanaged.guidance.slg.layers);
    }

    internal unsafe ref struct SampleParameterMarshallerIn
    {
        private Native.Types.sd_sample_params_t _sampleParams;

        private int* _slgLayers;

        public void FromManaged(SampleParameter managed)
        {
            _slgLayers = (int*)NativeMemory.Alloc((nuint)managed.Guidance.Slg.Layers.Length, (nuint)Marshal.SizeOf<int>());
            managed.Guidance.Slg.Layers.AsSpan().CopyTo(new Span<int>(_slgLayers, managed.Guidance.Slg.Layers.Length));

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

            _sampleParams = new Native.Types.sd_sample_params_t
            {
                guidance = guidance,
                scheduler = managed.Scheduler,
                sample_method = managed.SampleMethod,
                sample_steps = managed.SampleSteps,
                eta = managed.Eta,
                shifted_timestep = managed.ShiftedTimestep
            };
        }

        public Native.Types.sd_sample_params_t ToUnmanaged() => _sampleParams;

        public void Free()
        {
            if (_slgLayers != null)
                NativeMemory.Free(_slgLayers);
        }
    }

    internal ref struct SampleParameterMarshallerRef()
    {
        private SampleParameterMarshallerIn _inMarshaller = new();
        private SampleParameter? _parameter;

        public void FromManaged(SampleParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_sample_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_sample_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public SampleParameter ToManaged() => _parameter ?? throw new NullReferenceException($"{nameof(FromUnmanaged)} needs to be called before ToManaged.");

        public void Free() => _inMarshaller.Free();
    }
}