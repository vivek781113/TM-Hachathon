using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var projectPath = @"C:\Users\vivektiwary\workspace\TM-Hachathon\todo-csharp-sql";

        //read all the folders in projectPath & find files with .csproj extension & package.json
        var packageJsonFiles = Directory.GetFiles(projectPath, "package.json", SearchOption.AllDirectories);

        // read file with .sln extension and return it's absolute path

        var solutionFiles = Directory.GetFiles(projectPath, "*.sln", SearchOption.AllDirectories);
        if (solutionFiles.Length > 0)
        {
            var solutionFile = solutionFiles[0];
            ApplicationInformation projectDependecies = ComputeCsProjDependencies(solutionFile);
            string json = JsonConvert.SerializeObject(projectDependecies);
            Console.WriteLine(json);
        }
        else
        {
            Console.WriteLine("No solution file found.");
        }


        ComputePackageJsonDependencies(packageJsonFiles);
    }


    static ApplicationInformation ComputeCsProjDependencies(string solutionFile)
    {
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

        var packages = new HashSet<string>();
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
                packages.Add($"{packageName}, Version: {version}");
            }
        }

        packages.RemoveWhere(x => x.StartsWith("System", StringComparison.OrdinalIgnoreCase) || x.StartsWith("Swashbuckle", StringComparison.OrdinalIgnoreCase));

        return new ApplicationInformation()
        {
            ProjectType = ProjectType.DotNet,
            ProjectName = solutionName,
            AppDependencies = packages.ToList()
        };

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
                            ProjectType = ProjectType.React,
                            ProjectName = projectName,
                            //AppDependencies = ComputeCurrentPackageDependencies(packageJsonFile)
                        });
                    }

                }
            }
        }

        return applicationInfos;
    }

    private static List<ApplicationInformation> ComputeCurrentPackageDependencies(string packageJsonFile)
    {
        return [];
    }
}

class ApplicationInformation
{
    public ProjectType ProjectType { get; set; }
    public string ProjectName { get; set; }
    public List<string> AppDependencies { get; set; }

}


enum ProjectType
{
    React,
    Angular,
    Vue,
    DotNet
}
