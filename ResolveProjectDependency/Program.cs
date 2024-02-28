using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

internal class Program
{
    static readonly Dictionary<string, ApplicationInformation> packageResourceDict = new()
    {

        { "Azure.Extensions.AspNetCore.Configuration.Secrets", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "Az KV Secrets",
            ProjectType = "Azure Key Value",
            BindingDirection = System.ComponentModel.BindingDirection.OneWay
        }},

        { "Azure.Identity", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "Azure.Identity",
            ProjectType = "Idenity server",
            BindingDirection = System.ComponentModel.BindingDirection.TwoWay
        }}
,
        { "Microsoft.Data.SqlClient", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "Persistance Layer",
            ProjectType = "Sql Server Database",
            BindingDirection = System.ComponentModel.BindingDirection.TwoWay
        }}
    };
    private static void Main(string[] args)
    {
        var projectPath = @"C:\Users\vivektiwary\workspace\TM-Hachathon\todo-csharp-sql";

        //read all the folders in projectPath & find files with .csproj extension & package.json
        var packageJsonFiles = Directory.GetFiles(projectPath, "package.json", SearchOption.AllDirectories);

        // read file with .sln extension and return it's absolute path

        var solutionFiles = Directory.GetFiles(projectPath, "*.sln", SearchOption.AllDirectories);

        var packageJsonParsedResponse = ComputePackageJsonDependencies(packageJsonFiles);
        var dotnetparsedResponse = ComputeCsProjDependencies(solutionFiles[0]);

        packageJsonParsedResponse.AddRange(dotnetparsedResponse);

        File.WriteAllText("output.json", JsonConvert.SerializeObject(packageJsonParsedResponse, Formatting.Indented));

    }


    static List<ApplicationInformation> ComputeCsProjDependencies(string solutionFile)
    {
        var applicationInfos = new List<ApplicationInformation>();
        var solutionName = Path.GetFileNameWithoutExtension(solutionFile);
        // Run dotnet restore
        var restoreProcess = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"restore {solutionFile}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        restoreProcess.Start();
        restoreProcess.WaitForExit(); // Wait for the restore process to complete

        Console.WriteLine($"Restored packages for solution: {solutionName}");

        // write C# code to fetch all dependencies from solution file file using dotnet cli via Process class

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"list {solutionFile} package",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();

        var packages = new List<PackageInformation>();
        bool readPackages = false;

        while (!process.StandardOutput.EndOfStream)
        {
            var line = process.StandardOutput.ReadLine();

            // Start reading packages when the line with 'Top-level Package' is encountered
            if (!readPackages && line.Contains("Top-level Package"))
            {
                readPackages = true;
                continue;
            }

            // Stop reading packages when an empty line is encountered
            if (readPackages && string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            if (readPackages)
            {
                // Extract the package name from the line
                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var packageName = parts[1];
                var version = parts[2];
                var packageInfo = new PackageInformation
                {
                    PackageName = packageName,
                    Version = version
                };
                packages.Add(packageInfo);
            }
        }

        //packages.RemoveWhere(x => x.StartsWith("System", StringComparison.OrdinalIgnoreCase) || x.StartsWith("Swashbuckle", StringComparison.OrdinalIgnoreCase));

        var appId = Guid.NewGuid();
        applicationInfos.Add(new ApplicationInformation
        {
            ApplicationId = appId,
            ProjectType = nameof(ProjectType.DotNet),
            ProjectName = solutionName,
            AppDependencies = packages
        });
        foreach (var ap in packages)
        {
            if (packageResourceDict.TryGetValue(ap.PackageName, out ApplicationInformation? value))
            {
                applicationInfos.Add(new ApplicationInformation
                {
                    ParentAppId = appId,
                    ProjectName = solutionName +  value.ProjectName,
                    ProjectType = solutionName + value.ProjectType,
                    Version = ap.Version 
                });
            }
        }
        return applicationInfos;

    }



    static List<ApplicationInformation> ComputePackageJsonDependencies(string[] packageJsonFiles)
    {
        var applicationInfos = new List<ApplicationInformation>();
        for (int i = 0; i < packageJsonFiles.Length; i++)
        {
            string? packageJsonFile = packageJsonFiles[i];
            var packageJson = File.ReadAllText(packageJsonFile);
            var packageJsonDependencies = JsonConvert.DeserializeObject<JObject>(packageJson);
            var projectName = packageJsonDependencies["name"]?.ToString();
            if (projectName == null)
            {
                continue;
            }
            var devDependencies = packageJsonDependencies["devDependencies"];
            if (devDependencies != null)
            {
                foreach (JProperty devDependency in devDependencies.Cast<JProperty>())
                {
                    if (string.Equals(devDependency.Name, "@types/react", StringComparison.InvariantCultureIgnoreCase))
                    {
                        applicationInfos.Add(new ApplicationInformation
                        {
                            ProjectType = nameof(ProjectType.React),
                            ProjectName = projectName,
                            AppDependencies = ([])
                        });
                    }

                }
            }
        }

        return applicationInfos;
    }


    //Dictionary of package to resource type

}
