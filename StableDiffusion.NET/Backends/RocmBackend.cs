using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace StableDiffusion.NET;

public partial class RocmBackend : IBackend
{
    #region Properties & Fields

    public bool IsEnabled { get; set; } = true;

    public int Priority => 10;

    public bool IsAvailable => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                             /*|| RuntimeInformation.IsOSPlatform(OSPlatform.Linux)*/)
                            && (RuntimeInformation.OSArchitecture == Architecture.X64)
                            && RocmVersion is 5;

    public string PathPart => RocmVersion switch
    {
        5 => "rocm5",
        _ => string.Empty
    };

    public int RocmVersion { get; }

    #endregion

    #region Constructors

    internal RocmBackend()
    {
        RocmVersion = GetRocmMajorVersion();
    }

    #endregion

    #region Methods

    private static int GetRocmMajorVersion()
    {
        try
        {
            string version = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string? rocmPath = Environment.GetEnvironmentVariable("HIP_PATH");

                if (rocmPath == null) return -1;

                Match match = GetWindowsVersionRegex().Match(rocmPath);
                if (match.Success)
                    version = match.Groups["version"].Value;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                //TODO DarthAffe 23.03.2024: Get some info where it's located on linux
            }

            if (string.IsNullOrEmpty(version))
                return -1;

            version = version.Split('.')[0];
            if (int.TryParse(version, out int majorVersion))
                return majorVersion;
        }
        catch { /* No version or error */ }

        return -1;
    }

    [GeneratedRegex(@".*?\\(?<version>\d+.\d*)\\")]
    private static partial Regex GetWindowsVersionRegex();

    #endregion
}