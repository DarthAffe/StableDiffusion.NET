namespace StableDiffusion.NET;

public class StableDiffusionProgressEventArgs(int step, int steps, float time)
{
    #region Properties & Fields

    public int Step { get; } = step;
    public int Steps { get; } = steps;
    public float Time { get; } = time;

    public double Progress => (double)Step / Steps;
    public float IterationsPerSecond => 1f / Time;

    #endregion
}