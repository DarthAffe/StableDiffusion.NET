namespace StableDiffusion.NET;

public sealed unsafe class StableDiffusionParameter : IDisposable
{
    #region Properties & Fields

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    internal readonly Native.stable_diffusion_full_params* ParamPtr;
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

    private string _negativePrompt;
    public string NegativePrompt
    {
        get => _negativePrompt;
        set
        {
            _negativePrompt = value;
            Native.stable_diffusion_full_params_set_negative_prompt(ParamPtr, _negativePrompt);
        }
    }

    private float _cfgScale;
    public float CfgScale
    {
        get => _cfgScale;
        set
        {
            _cfgScale = value;
            Native.stable_diffusion_full_params_set_cfg_scale(ParamPtr, _cfgScale);
        }
    }

    private int _width;
    public int Width
    {
        get => _width;
        set
        {
            _width = value;
            Native.stable_diffusion_full_params_set_width(ParamPtr, _width);
        }
    }

    private int _height;
    public int Height
    {
        get => _height;
        set
        {
            _height = value;
            Native.stable_diffusion_full_params_set_height(ParamPtr, _height);
        }
    }

    private Sampler _sampleMethod;
    public Sampler SampleMethod
    {
        get => _sampleMethod;
        set
        {
            _sampleMethod = value;
            Native.stable_diffusion_full_params_set_sample_method(ParamPtr, _sampleMethod.GetNativeName() ?? "EULER_A");
        }
    }

    private int _sampleSteps;
    public int SampleSteps
    {
        get => _sampleSteps;
        set
        {
            _sampleSteps = value;
            Native.stable_diffusion_full_params_set_sample_steps(ParamPtr, _sampleSteps);
        }
    }

    private long _seed;
    public long Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            Native.stable_diffusion_full_params_set_seed(ParamPtr, _seed);
        }
    }

    private int _batchCount;
    public int BatchCount
    {
        get => _batchCount;
        set
        {
            _batchCount = value;
            Native.stable_diffusion_full_params_set_batch_count(ParamPtr, _batchCount);
        }
    }

    private float _strength;
    public float Strength
    {
        get => _strength;
        set
        {
            _strength = value;
            Native.stable_diffusion_full_params_set_strength(ParamPtr, _strength);
        }
    }

    #endregion

    #region Constructors

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public StableDiffusionParameter()
    {
        ParamPtr = Native.stable_diffusion_full_default_params_ref();

        NegativePrompt = string.Empty;
        CfgScale = 7.5f;
        Width = 512;
        Height = 512;
        SampleMethod = Sampler.Euler_A;
        SampleSteps = 25;
        Seed = -1;
        BatchCount = 1;
        Strength = 0.7f;
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    ~StableDiffusionParameter() => Dispose();

    #endregion

    #region Methods

    public void Dispose()
    {
        Native.stable_diffusion_free_full_params(ParamPtr);

        GC.SuppressFinalize(this);
    }

    #endregion
}