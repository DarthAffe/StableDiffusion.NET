namespace StableDiffusion.NET;

public enum LogLevel
{
    [NativeName("DEBUG")]
    Debug,

    [NativeName("INFO")]
    Info,

    [NativeName("WARN")]
    Warn,

    [NativeName("ERROR")]
    Error
}