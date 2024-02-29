using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResolveProjectDependency.Resolvers;
using ResolveProjectDependency.Utils;
using System.Diagnostics;

internal class Program
{
    static void Main(string[] args)
    {
        if (!AppValidations.InitValidation())
            Console.Error.WriteLine("Please install the required tools [dotnet cli, git, node] and try again.");

        var projectPath = args.Length == 0 ? CloneGitRepo() : CloneGitRepo(args[0]);
  
        var packageJsonParsedResponse = NodeResolver.ComputePackageJsonDependencies(projectPath);
        var dotnetParsedResponse = DotnetResolver.ComputeCsProjDependencies(projectPath);

        packageJsonParsedResponse.AddRange(dotnetParsedResponse);

        File.WriteAllText("output.json", JsonConvert.SerializeObject(packageJsonParsedResponse, Formatting.Indented));
    }


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
        return tempDirectory;
    }



}
