using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public class VulkanBackend : IBackend
{
    #region Properties & Fields

    //TODO DarthAffe 28.08.2024: Find a way to detect vulkan compatibility
    public bool IsEnabled { get; set; } = false;

    public int Priority { get; set; } = 5;

    public bool IsAvailable => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                            && (RuntimeInformation.OSArchitecture == Architecture.X64);

    public string PathPart => "vulkan";

    #endregion

    #region Constructors

    internal VulkanBackend()
    { }

    #endregion
}