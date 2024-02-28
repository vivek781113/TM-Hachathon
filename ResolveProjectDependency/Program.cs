using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        var projectPath = @"C:\Users\vivektiwary\workspace\TM-Hachathon\todo-csharp-sql";

        //read all the folders in projectPath & find files with .csproj extension & package.json
        var projectFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.AllDirectories);
        var packageJsonFiles = Directory.GetFiles(projectPath, "package.json", SearchOption.AllDirectories);

        ComputePackageJsonDependencies(packageJsonFiles);
    }


    static void ComputeCsProjDependencies(string[] projectFiles)
    {

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
                            ProjectName = projectName
                        });
                    }

                }
            }
        }

        return applicationInfos;
    }
}

class ApplicationInformation
{
    public ProjectType ProjectType { get; set; }
    public string ProjectName { get; set; }

}


enum ProjectType
{
    React,
    Angular,
    Vue,
    DotNet
}
