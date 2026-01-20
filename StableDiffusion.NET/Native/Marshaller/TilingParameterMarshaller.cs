// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(TilingParameter), MarshalMode.ManagedToUnmanagedIn, typeof(TilingParameterMarshallerIn))]
[CustomMarshaller(typeof(TilingParameter), MarshalMode.ManagedToUnmanagedOut, typeof(TilingParameterMarshaller))]
[CustomMarshaller(typeof(TilingParameter), MarshalMode.ManagedToUnmanagedRef, typeof(TilingParameterMarshallerRef))]
internal static unsafe class TilingParameterMarshaller
{
    public static TilingParameter ConvertToManaged(Native.Types.sd_tiling_params_t unmanaged)
    {
        TilingParameter parameter = new()
        {
            IsEnabled = unmanaged.enabled == 1,
            TileSizeX = unmanaged.tile_size_x,
            TileSizeY = unmanaged.tile_size_y,
            TargetOverlap = unmanaged.target_overlap,
            RelSizeX = unmanaged.rel_size_x,
            RelSizeY = unmanaged.rel_size_y
        };

        return parameter;
    }

    internal ref struct TilingParameterMarshallerIn
    {
        private Native.Types.sd_tiling_params_t _tilingParams;

        public void FromManaged(TilingParameter managed)
        {
            _tilingParams = new Native.Types.sd_tiling_params_t
            {
                enabled = (sbyte)(managed.IsEnabled ? 1 : 0),
                tile_size_x = managed.TileSizeX,
                tile_size_y = managed.TileSizeY,
                target_overlap = managed.TargetOverlap,
                rel_size_x = managed.RelSizeX,
                rel_size_y = managed.RelSizeY
            };
        }

        public Native.Types.sd_tiling_params_t ToUnmanaged() => _tilingParams;

        public void Free() { }
    }

    internal ref struct TilingParameterMarshallerRef()
    {
        private TilingParameterMarshallerIn _inMarshaller = new();
        private TilingParameter? _parameter;

        public void FromManaged(TilingParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_tiling_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_tiling_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public TilingParameter ToManaged() => _parameter ?? throw new NullReferenceException($"{nameof(FromUnmanaged)} needs to be called before ToManaged.");

        public void Free() => _inMarshaller.Free();
    }
}