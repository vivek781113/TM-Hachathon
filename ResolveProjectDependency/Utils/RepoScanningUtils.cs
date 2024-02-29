using Newtonsoft.Json;
using ResolveProjectDependency.Resolvers;
using System.Diagnostics;

namespace ResolveProjectDependency.Utils;

internal static class RepoScanningUtils
{

    //method to clone git repo & keep in temp directory & return the path
    static string CloneGitRepo(string repoUrl = "https://github.com/Azure-Samples/todo-csharp-sql")
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"clone {repoUrl} {tempDirectory}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        process.WaitForExit();
        Console.WriteLine(tempDirectory);
        return tempDirectory;
    }

    private static void ProduceOutput(List<ApplicationInformation> packageJsonParsedResponse)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(currentDirectory, $"{DateTime.Now.Ticks}_output.json");
        File.WriteAllText(filePath, JsonConvert.SerializeObject(packageJsonParsedResponse, Formatting.Indented));
        Console.WriteLine($"output file is at: {filePath}");
    }

    public static void RepositoryScanning(string[] args)
    {
        if (!AppValidations.InitValidation())
            Console.Error.WriteLine("Please install the required tools [dotnet cli, git, node] and try again.");

        var projectPath = args.Length == 0 ? CloneGitRepo() : CloneGitRepo(args[0]);

        var packageJsonParsedResponse = NodeResolver.ComputePackageJsonDependencies(projectPath);
        var dotnetParsedResponse = DotnetResolver.ComputeCsProjDependencies(projectPath);

        packageJsonParsedResponse.AddRange(dotnetParsedResponse);

        ProduceOutput(packageJsonParsedResponse);
    }

    //delete temp directory after use
    static void DeleteTempDirectory(string tempDirectory)
    {
        Directory.Delete(tempDirectory, true);
    }
}