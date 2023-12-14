namespace StableDiffusion.NET;

public enum Schedule
{
    [NativeName("DEFAULT")]
    Default,

    [NativeName("DISCRETE")]
    Discrete,

    [NativeName("KARRAS")]
    Karras,

    [NativeName("N_SCHEDULES")]
    N_Schedules
}