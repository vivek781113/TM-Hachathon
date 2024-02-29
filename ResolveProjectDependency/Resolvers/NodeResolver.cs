using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ResolveProjectDependency.Resolvers;

internal class NodeResolver
{
    public static List<ApplicationInformation> ComputePackageJsonDependencies(string projectpath)
    {
        var packageJsonFiles = Directory.GetFiles(projectpath, "package.json", SearchOption.AllDirectories);

        var applicationInfos = new List<ApplicationInformation>();

        foreach (var packageJsonFile in packageJsonFiles)
        {
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
                            ApplicationId = Guid.NewGuid(),
                            ProjectType = nameof(ProjectType.React),
                            ProjectName = projectName,
                            Version = devDependency.Value.ToString()
                        });
                    }
                }
            }
        }

        return applicationInfos;
    }

}
