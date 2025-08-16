using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace StableDiffusion.NET;

internal static partial class Native
{
    #region Properties & Fields

    private static nint _loadedLibraryHandle;

    #endregion

    #region Constructors

    static Native()
    {
        NativeLibrary.SetDllImportResolver(typeof(Native).Assembly, ResolveDllImport);
    }

    #endregion

    #region Methods

    internal static bool LoadNativeLibrary(string libraryPath)
    {
        if (_loadedLibraryHandle != nint.Zero) return true;
        if (NativeLibrary.TryLoad(libraryPath, out nint handle))
        {
            _loadedLibraryHandle = handle;
            return true;
        }

        return false;
    }

    private static nint ResolveDllImport(string libraryname, Assembly _, DllImportSearchPath? __)
    {
        if (libraryname != LIB_NAME) return nint.Zero;
        if (_loadedLibraryHandle != nint.Zero) return _loadedLibraryHandle;

        _loadedLibraryHandle = TryLoadLibrary();

        return _loadedLibraryHandle;
    }

    private static nint TryLoadLibrary()
    {
        GetPlatformPathParts(out string os, out string fileExtension, out string libPrefix);

        foreach (IBackend backend in Backends.ActiveBackends.OrderByDescending(x => x.Priority))
        {
            string path = Backends.GetFullPath(os, backend.PathPart, $"{libPrefix}{LIB_NAME}{fileExtension}");
            if (string.IsNullOrWhiteSpace(path)) continue;

            string fullPath = TryFindPath(path);
            nint result = TryLoad(fullPath);

            if (result != nint.Zero)
                return result;
        }

        return nint.Zero;

        static nint TryLoad(string path)
        {
            if (NativeLibrary.TryLoad(path, out nint handle))
                return handle;

            return nint.Zero;
        }

        static string TryFindPath(string filename)
        {
            IEnumerable<string> searchPaths = [.. Backends.SearchPaths, AppDomain.CurrentDomain.BaseDirectory, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ""];
            foreach (string path in searchPaths)
            {
                string candidate = Path.Combine(path, filename);
                if (File.Exists(candidate))
                    return candidate;
            }

            return filename;
        }
    }

    private static void GetPlatformPathParts(out string os, out string fileExtension, out string libPrefix)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            os = "win-x64";
            fileExtension = ".dll";
            libPrefix = "";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            os = "linux-x64";
            fileExtension = ".so";
            libPrefix = "lib";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            fileExtension = ".dylib";
            os = "osx-x64";
            libPrefix = "lib";
        }
        else
            throw new NotSupportedException("Your operating system is not supported.");
    }

    #endregion
}