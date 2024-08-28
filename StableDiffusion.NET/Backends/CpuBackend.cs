using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

[PublicAPI]
public class CpuBackend : IBackend
{
    #region Properties & Fields

    public bool IsEnabled { get; set; } = true;

    public int Priority { get; set; } = 0;

    public bool IsAvailable => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                             || RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                             || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            && (RuntimeInformation.OSArchitecture == Architecture.X64);

    public string PathPart => Avx.GetDescription();

    private readonly List<AvxLevel> _availableAvxLevels = [];
    public IEnumerable<AvxLevel> AvailableAvxLevels => _availableAvxLevels.AsReadOnly();

    private AvxLevel _avx;
    public AvxLevel Avx
    {
        get => _avx;
        set
        {
            if (!_availableAvxLevels.Contains(value)) throw new ArgumentException("The selected AVX-Level is not supported on this system.");
            _avx = value;
        }
    }

    #endregion

    #region Constructors

    internal CpuBackend()
    {
        _availableAvxLevels.Add(AvxLevel.None);
        Avx = AvxLevel.None;

        if (System.Runtime.Intrinsics.X86.Avx.IsSupported)
        {
            _availableAvxLevels.Add(AvxLevel.Avx);
            Avx = AvxLevel.Avx;
        }

        if (System.Runtime.Intrinsics.X86.Avx2.IsSupported)
        {
            _availableAvxLevels.Add(AvxLevel.Avx2);
            Avx = AvxLevel.Avx2;
        }

        if (CheckAvx512())
        {
            _availableAvxLevels.Add(AvxLevel.Avx512);
            Avx = AvxLevel.Avx512;
        }
    }

    #endregion

    #region Methods

    private static bool CheckAvx512()
    {
        if (!System.Runtime.Intrinsics.X86.X86Base.IsSupported)
            return false;

        (_, int _, int ecx, _) = System.Runtime.Intrinsics.X86.X86Base.CpuId(7, 0);

        bool vnni = (ecx & 0b_1000_0000_0000) != 0;

        bool f = System.Runtime.Intrinsics.X86.Avx512F.IsSupported;
        bool bw = System.Runtime.Intrinsics.X86.Avx512BW.IsSupported;
        bool vbmi = System.Runtime.Intrinsics.X86.Avx512Vbmi.IsSupported;

        return vnni && vbmi && bw && f;
    }

    #endregion

    public enum AvxLevel
    {
        [Description("")]
        None,

        [Description("avx")]
        Avx,

        [Description("avx2")]
        Avx2,

        [Description("avx512")]
        Avx512,
    }
}