using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public class SyclBackend : IBackend
{
    #region Properties & Fields

    //TODO DarthAffe 10.08.2024: tbh I'm not really sure how to detect a sycl-compatible system so for now it's disabled by default
    public bool IsEnabled { get; set; } = false;

    public int Priority { get; set; } = 5;

    public bool IsAvailable => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                             || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            && (RuntimeInformation.OSArchitecture == Architecture.X64);

    public string PathPart => "sycl";

    #endregion

    #region Constructors

    internal SyclBackend()
    { }

    #endregion
}