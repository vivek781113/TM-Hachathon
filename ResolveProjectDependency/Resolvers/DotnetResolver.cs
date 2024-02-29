﻿using System.Diagnostics;

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
        var solutionFile = Directory.GetFiles(projectPath, "*.sln", SearchOption.AllDirectories)[0];
        
        var applicationInfos = new List<ApplicationInformation>();
        var solutionName = Path.GetFileNameWithoutExtension(solutionFile);

        RunDotnetRestore(solutionFile);
        Console.WriteLine($"Restored packages for solution: {solutionName}");

        var packages = FetchPackagesFromSolution(solutionFile);

        var appId = Guid.NewGuid();
        applicationInfos.Add(new ApplicationInformation
        {
            ApplicationId = appId,
            ProjectType = nameof(ProjectType.DotNet),
            ProjectName = solutionName,
            Version = "8.0",
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
                    ProjectName = $"{solutionName}_{value.ProjectName}",
                    ProjectType = value.ProjectType,
                    BindingDirection = value.BindingDirection,
                    Version = ap.Version,
                    BinaryFileName = ap.PackageName
                });
            }
        }

        return applicationInfos;
    }

    static void RunDotnetRestore(string solutionFile)
    {
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
        restoreProcess.WaitForExit();
    }

    static List<PackageInformation> FetchPackagesFromSolution(string solutionFile)
    {
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

            if (!readPackages && line.Contains("Top-level Package"))
            {
                readPackages = true;
                continue;
            }

            if (readPackages && string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            if (readPackages)
            {
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

        return packages;
    }

}