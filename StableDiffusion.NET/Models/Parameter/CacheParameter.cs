using JetBrains.Annotations;

namespace StableDiffusion.NET;

[PublicAPI]
public sealed class CacheParameter
{
    public CacheMode Mode { get; set; } = CacheMode.Disabled;
    public float ReuseThreshold { get; set; } = 1.0f;
    public float StartPercent { get; set; } = 0.15f;
    public float EndPercent { get; set; } = 0.95f;
    public float ErrorDecayRate { get; set; } = 1.0f;
    public bool UseRelativeThreshold { get; set; } = true;
    public bool ResetErrorOnCompute { get; set; } = true;
    public int FnComputeBlocks { get; set; } = 8;
    public int BnComputeBlocks { get; set; } = 0;
    public float ResidualDiffThreshold { get; set; } = 0.08f;
    public int MaxWarmupSteps { get; set; } = 8;
    public int MaxCachedSteps { get; set; } = -1;
    public int MaxContinuousCachedSteps { get; set; } = -1;
    public int TaylorseerNDerivatives { get; set; } = 1;
    public int TaylorseerSkipInterval { get; set; } = 1;
    public string? ScmMask { get; set; } = null;
    public bool ScmPolicyDynamic { get; set; } = true;

    internal CacheParameter() { }
}
