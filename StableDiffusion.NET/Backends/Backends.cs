﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StableDiffusion.NET;

[PublicAPI]
public static class Backends
{
    #region Properties & Fields

    public static CpuBackend CpuBackend { get; } = new();
    public static CudaBackend CudaBackend { get; } = new();
    public static RocmBackend RocmBackend { get; } = new();
    public static SyclBackend SyclBackend { get; } = new();

    private static readonly List<IBackend> CUSTOM_BACKENDS = [];
    public static IReadOnlyList<IBackend> CustomBackends => CUSTOM_BACKENDS.AsReadOnly();

    public static IEnumerable<IBackend> RegisteredBackends => [CpuBackend, CudaBackend, RocmBackend, SyclBackend, .. CUSTOM_BACKENDS];
    public static IEnumerable<IBackend> AvailableBackends => RegisteredBackends.Where(x => x.IsAvailable);
    public static IEnumerable<IBackend> ActiveBackends => AvailableBackends.Where(x => x.IsEnabled);

    public static List<string> SearchPaths { get; } = [];

    #endregion

    #region Events

    public static event EventHandler<LibraryPathCreatingEventArgs>? LibraryPathCreating;

    #endregion

    #region Methods

    public static bool RegisterBackend(IBackend backend)
    {
        if (backend is NET.CpuBackend or NET.CudaBackend or NET.RocmBackend or NET.SyclBackend)
            throw new ArgumentException("Default backends can't be registered again.");

        if (CUSTOM_BACKENDS.Contains(backend))
            return false;

        CUSTOM_BACKENDS.Add(backend);
        return true;
    }

    public static bool UnregisterBackend(IBackend backend)
        => CUSTOM_BACKENDS.Remove(backend);

    internal static string GetFullPath(string os, string backend, string libName)
    {
        string path = Path.Combine("runtimes", os, "native", backend, libName);
        return OnLibraryPathCreating(path);
    }

    private static string OnLibraryPathCreating(string path)
    {
        try
        {
            LibraryPathCreatingEventArgs args = new(path);
            LibraryPathCreating?.Invoke(null, args);
            return args.Path;
        }
        catch
        {
            return path;
        }
    }

    #endregion
}