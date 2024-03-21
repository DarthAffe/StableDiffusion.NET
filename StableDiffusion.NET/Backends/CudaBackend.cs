using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System;

namespace StableDiffusion.NET;

public class CudaBackend : IBackend
{
    #region Constants

    private const string CUDA_VERSION_FILE = "version.json";

    #endregion

    #region Properties & Fields

    public bool IsEnabled { get; set; } = true;

    public int Priority => 10;

    public bool IsAvailable => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                             || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            && (RuntimeInformation.OSArchitecture == Architecture.X64)
                            && CudaVersion is 11 or 12;

    public string PathPart => CudaVersion switch
    {
        11 => "cuda11",
        12 => "cuda12",
        _ => string.Empty
    };

    public int CudaVersion { get; }

    #endregion

    #region Constructors

    internal CudaBackend()
    {
        CudaVersion = GetCudaMajorVersion();
    }

    #endregion

    #region Methods

    private static int GetCudaMajorVersion()
    {
        try
        {
            string? cudaPath;
            string version = "";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                cudaPath = Environment.GetEnvironmentVariable("CUDA_PATH");
                if (cudaPath is null) return -1;

                version = GetCudaVersionFromPath(cudaPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cudaPath = "/usr/local/bin/cuda";
                version = GetCudaVersionFromPath(cudaPath);
                if (string.IsNullOrEmpty(version))
                {
                    cudaPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH");
                    if (cudaPath is null)
                        return -1;

                    foreach (string path in cudaPath.Split(':'))
                    {
                        version = GetCudaVersionFromPath(Path.Combine(path, ".."));
                        if (string.IsNullOrEmpty(version))
                            break;
                    }
                }
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

    private static string GetCudaVersionFromPath(string cudaPath)
    {
        try
        {
            string json = File.ReadAllText(Path.Combine(cudaPath, CUDA_VERSION_FILE));
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;
            JsonElement cublasNode = root.GetProperty("libcublas");
            JsonElement versionNode = cublasNode.GetProperty("version");
            if (versionNode.ValueKind == JsonValueKind.Undefined)
                return string.Empty;

            return versionNode.GetString() ?? "";
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    #endregion
}