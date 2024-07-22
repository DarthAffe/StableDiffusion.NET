using System.Diagnostics;

namespace StableDiffusion.NET;

internal static class ProcessHelper
{
    public static string? RunCommand(string command)
    {
        try
        {
            using Process process = new();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = command;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        catch
        {
            return null;
        }
    }
}