using System.Collections.Concurrent;
using System.Xml.Linq;
namespace ResolveProjectDependency.Resolvers;

public class DotnetResolver
{
    static readonly Dictionary<string, ApplicationInformation> packageResourceDict = new()
{
    { "Azure.Extensions.AspNetCore.Configuration.Secrets", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Az KeyVault",
        ProjectType = "Azure Key Vault",
        BindingDirection = nameof(BindingDirection.Inbound)
    }},
    { "Azure.Identity", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Azure.Identity",
        ProjectType = "Identity server",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    }},
    {
        "Microsoft.ApplicationInsights.AspNetCore", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "AppInsights",
            ProjectType = "Application Insights",
            BindingDirection = nameof(BindingDirection.Outbound)
        } },
    { "Microsoft.Data.SqlClient", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Persistence Layer",
        ProjectType = "SQL Server Database",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    }}
};

    public static List<ApplicationInformation> ComputeCsProjDependencies(string projectPath)
    {
        var applicationInfos = new List<ApplicationInformation>();
        var csprojFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.AllDirectories);

        foreach (var csprojFile in csprojFiles)
        {
            var projectName = Path.GetFileNameWithoutExtension(csprojFile);

            var (targetFramework, project, packages) = FetchPackagesFromSolution(csprojFile);
            var appId = Guid.NewGuid();
            applicationInfos.Add(new ApplicationInformation
            {
                ApplicationId = appId,
                ProjectType = project,
                ProjectName = projectName,
                Version = targetFramework,
                BinaryFileName = ".net"
            });

            foreach (var ap in packages)
            {
                if (packageResourceDict.TryGetValue(ap.PackageName, out ApplicationInformation? value))
                {
                    applicationInfos.Add(new ApplicationInformation
                    {
                        ParentAppId = appId,
                        ApplicationId = Guid.NewGuid(),
                        ProjectName = $"{projectName}_{value.ProjectName}",
                        ProjectType = value.ProjectType,
                        BindingDirection = value.BindingDirection,
                        Version = ap.Version,
                        BinaryFileName = ap.PackageName
                    });
                }
            }
        }




        return applicationInfos;
    }

    static (string, string, List<PackageInformation>) FetchPackagesFromSolution(string csprojFile)
    {
        var packages = new ConcurrentBag<PackageInformation>();
        string targetFramework = string.Empty;
        string project = string.Empty;
        var xDocument = XDocument.Load(csprojFile);
        var projectNode = xDocument.Root;
        if (projectNode != null)
        {
            XNamespace ns = projectNode.GetDefaultNamespace();
            var sdk = projectNode.Attribute("Sdk")?.Value;

            var outputType = projectNode.Descendants(ns + "OutputType").FirstOrDefault()?.Value;
            if (outputType != null)
            {
            }

            var framework = projectNode.Descendants(ns + "TargetFramework").FirstOrDefault()?.Value;
            if (framework != null)
            {
                targetFramework = framework;
            }

            if (sdk == "Microsoft.NET.Sdk.Web")
            {
                project = nameof(DotNetProjectType.Web);
            }
            else if (outputType == "Exe")
            {
                project = nameof(DotNetProjectType.Console);
            }
            else if (outputType == "Library")
            {
                project = nameof(DotNetProjectType.Library);
            }

        }
        var packageReferences = xDocument.Descendants()
            .Where(d => d.Name.LocalName == "PackageReference");

        Parallel.ForEach(packageReferences, packageReference =>
        {
            if (packageReference.Attributes("Include").FirstOrDefault() is XAttribute includeAttribute &&
                packageReference.Attributes("Version").FirstOrDefault() is XAttribute versionAttribute)
            {
                packages.Add(new PackageInformation
                {
                    PackageName = includeAttribute.Value,
                    Version = versionAttribute.Value
                });
            }
        });

        return (targetFramework, project, packages.ToList());
    }

    private enum DotNetProjectType
    {
        Web,
        Console,
        Library
    }

}
