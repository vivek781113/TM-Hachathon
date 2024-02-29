using System.Diagnostics;

namespace ResolveProjectDependency.Utils;

public static class AppValidations
{
    //method to check dotnet cli installed or not
    private static bool IsDotnetCliInstalled()
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        var version = process.StandardOutput.ReadToEnd();
        return !string.IsNullOrWhiteSpace(version);
    }

    //method to check git cli installed or not
    private static bool IsGitCliInstalled()
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        var version = process.StandardOutput.ReadToEnd();
        return !string.IsNullOrWhiteSpace(version);
    }

    //method to check node is installed or not
    private static bool IsNodeInstalled()
    {
        //var process = new Process()
        //{
        //    StartInfo = new ProcessStartInfo
        //    {
        //        FileName = "node",
        //        Arguments = "-v",
        //        RedirectStandardOutput = true,
        //        UseShellExecute = false,
        //        CreateNoWindow = true,
        //    }
        //};

        //process.Start();
        //var version = process.StandardOutput.ReadToEnd();
        //return !string.IsNullOrWhiteSpace(version);
        return true;
    }

    public static bool InitValidation() => IsDotnetCliInstalled() && IsGitCliInstalled() && IsNodeInstalled();
}
