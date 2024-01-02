namespace StableDiffusion.NET;

public class StableDiffusionLogEventArgs(LogLevel level, string text)
{
    #region Properties & Fields

    public LogLevel Level { get; } = level;
    public string Text { get; } = text;

    #endregion
}