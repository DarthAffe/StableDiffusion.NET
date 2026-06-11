// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(HiresParameter), MarshalMode.ManagedToUnmanagedIn, typeof(HiresParameterMarshallerIn))]
[CustomMarshaller(typeof(HiresParameter), MarshalMode.ManagedToUnmanagedOut, typeof(HiresParameterMarshaller))]
[CustomMarshaller(typeof(HiresParameter), MarshalMode.ManagedToUnmanagedRef, typeof(HiresParameterMarshallerRef))]
internal static unsafe class HiresParameterMarshaller
{
    public static HiresParameter ConvertToManaged(Native.Types.sd_hires_params_t unmanaged)
    {
        HiresParameter parameter = new()
        {
            Enabled = unmanaged.enabled == 1,
            Upscaler = unmanaged.upscaler,
            ModelPath = AnsiStringMarshaller.ConvertToManaged(unmanaged.model_path),
            Scale = unmanaged.scale,
            TargetWidth = unmanaged.target_width,
            TargetHeight = unmanaged.target_height,
            Steps = unmanaged.steps,
            DenoisingStrength = unmanaged.denoising_strength,
            UpscaleTileSize = unmanaged.upscale_tile_size
        };

        return parameter;
    }

    internal ref struct HiresParameterMarshallerIn
    {
        private Native.Types.sd_hires_params_t _hiresParams;

        public void FromManaged(HiresParameter managed)
        {
            _hiresParams = new Native.Types.sd_hires_params_t
            {
                enabled = (sbyte)(managed.Enabled ? 1 : 0),
                upscaler = managed.Upscaler,
                model_path = AnsiStringMarshaller.ConvertToUnmanaged(managed.ModelPath),
                scale = managed.Scale,
                target_width = managed.TargetWidth,
                target_height = managed.TargetHeight,
                steps = managed.Steps,
                denoising_strength = managed.DenoisingStrength,
            };
        }

        public Native.Types.sd_hires_params_t ToUnmanaged() => _hiresParams;

        public void Free()
        {
            AnsiStringMarshaller.Free(_hiresParams.model_path);
        }
    }

    internal ref struct HiresParameterMarshallerRef()
    {
        private HiresParameterMarshallerIn _inMarshaller = new();
        private HiresParameter? _parameter;

        public void FromManaged(HiresParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_hires_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_hires_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public HiresParameter ToManaged() => _parameter ?? throw new NullReferenceException($"{nameof(FromUnmanaged)} needs to be called before ToManaged.");

        public void Free() => _inMarshaller.Free();
    }
}