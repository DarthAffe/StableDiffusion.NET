// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Runtime.InteropServices.Marshalling;

namespace StableDiffusion.NET;

[CustomMarshaller(typeof(CacheParameter), MarshalMode.ManagedToUnmanagedIn, typeof(CacheParameterMarshallerIn))]
[CustomMarshaller(typeof(CacheParameter), MarshalMode.ManagedToUnmanagedOut, typeof(CacheParameterMarshaller))]
[CustomMarshaller(typeof(CacheParameter), MarshalMode.ManagedToUnmanagedRef, typeof(CacheParameterMarshallerRef))]
internal static unsafe class CacheParameterMarshaller
{
    public static CacheParameter ConvertToManaged(Native.Types.sd_cache_params_t unmanaged)
    {
        CacheParameter parameter = new()
        {
            Mode = unmanaged.mode,
            ReuseThreshold = unmanaged.reuse_threshold,
            StartPercent = unmanaged.start_percent,
            EndPercent = unmanaged.end_percent,
            ErrorDecayRate = unmanaged.error_decay_rate,
            UseRelativeThreshold = unmanaged.use_relative_threshold == 1,
            ResetErrorOnCompute = unmanaged.reset_error_on_compute == 1,
            FnComputeBlocks = unmanaged.Fn_compute_blocks,
            BnComputeBlocks = unmanaged.Bn_compute_blocks,
            ResidualDiffThreshold = unmanaged.residual_diff_threshold,
            MaxWarmupSteps = unmanaged.max_warmup_steps,
            MaxCachedSteps = unmanaged.max_cached_steps,
            MaxContinuousCachedSteps = unmanaged.max_continuous_cached_steps,
            TaylorseerNDerivatives = unmanaged.taylorseer_n_derivatives,
            TaylorseerSkipInterval = unmanaged.taylorseer_skip_interval,
            ScmMask = AnsiStringMarshaller.ConvertToManaged(unmanaged.scm_mask),
            ScmPolicyDynamic = unmanaged.scm_policy_dynamic == 1
        };

        return parameter;
    }

    internal ref struct CacheParameterMarshallerIn
    {
        private Native.Types.sd_cache_params_t _cacheParams;
        
        public void FromManaged(CacheParameter managed)
        {

            _cacheParams = new Native.Types.sd_cache_params_t
            {
                mode = managed.Mode,
                reuse_threshold = managed.ReuseThreshold,
                start_percent = managed.StartPercent,
                end_percent = managed.EndPercent,
                error_decay_rate = managed.ErrorDecayRate,
                use_relative_threshold = (sbyte)(managed.UseRelativeThreshold ? 1 : 0),
                reset_error_on_compute = (sbyte)(managed.ResetErrorOnCompute ? 1 : 0),
                Fn_compute_blocks = managed.FnComputeBlocks,
                Bn_compute_blocks = managed.BnComputeBlocks,
                residual_diff_threshold = managed.ResidualDiffThreshold,
                max_warmup_steps = managed.MaxWarmupSteps,
                max_cached_steps = managed.MaxCachedSteps,
                max_continuous_cached_steps = managed.MaxContinuousCachedSteps,
                taylorseer_n_derivatives = managed.TaylorseerNDerivatives,
                taylorseer_skip_interval = managed.TaylorseerSkipInterval,
                scm_mask = AnsiStringMarshaller.ConvertToUnmanaged(managed.ScmMask),
                scm_policy_dynamic = (sbyte)(managed.ScmPolicyDynamic ? 1 : 0)
            };
        }

        public Native.Types.sd_cache_params_t ToUnmanaged() => _cacheParams;

        public void Free()
        {
            AnsiStringMarshaller.Free(_cacheParams.scm_mask);
        }
    }

    internal ref struct CacheParameterMarshallerRef()
    {
        private CacheParameterMarshallerIn _inMarshaller = new();
        private CacheParameter? _parameter;

        public void FromManaged(CacheParameter managed) => _inMarshaller.FromManaged(managed);
        public Native.Types.sd_cache_params_t ToUnmanaged() => _inMarshaller.ToUnmanaged();

        public void FromUnmanaged(Native.Types.sd_cache_params_t unmanaged) => _parameter = ConvertToManaged(unmanaged);
        public CacheParameter ToManaged() => _parameter ?? throw new NullReferenceException($"{nameof(FromUnmanaged)} needs to be called before ToManaged.");

        public void Free() => _inMarshaller.Free();
    }
}