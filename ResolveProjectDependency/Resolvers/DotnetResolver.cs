using System.Collections.Concurrent;
using System.Xml.Linq;
namespace ResolveProjectDependency.Resolvers;

public class DotnetResolver
{
    static readonly Dictionary<string, ApplicationInformation> packageResourceDict = new()
{
#region Messaging services
        {"Azure.Messaging.ServiceBus", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "Azure.Messaging.ServiceBus",
            ProjectType = "Messaging Service",
            BindingDirection = nameof(BindingDirection.Bidirectional)
        } },
        {
        "MassTransit",
        new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "MassTransit",
            ProjectType = "Messaging Service",
            BindingDirection = BindingDirection.Bidirectional.ToString()
        }
    },
        {
            "NServiceBus",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "NServiceBus",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
        {
            "RabbitMQ.Client",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "RabbitMQ.Client",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
        {
            "EasyNetQ",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "EasyNetQ",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
        {
            "Amazon.SQS",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "Amazon.SQS",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
        {
            "StackExchange.Redis",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "StackExchange.Redis",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
        {
            "MQTTnet",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "MQTTnet",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
        {
            "SignalR",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "SignalR",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
        {
            "SignalR.Client",
            new ApplicationInformation
            {
                ApplicationId = Guid.NewGuid(),
                ProjectName = "SignalR.Client",
                ProjectType = "Messaging Service",
                BindingDirection = BindingDirection.Bidirectional.ToString()
            }
        },
#endregion
        #region secret mgmt
        { "Azure.Extensions.AspNetCore.Configuration.Secrets", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Az KeyVault",
        ProjectType = "Azure Key Vault",
        BindingDirection = nameof(BindingDirection.Inbound)
    }},
    { "Amazon.SecretsManager", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "AWS Secrets Manager",
            ProjectType = "AWS Secrets Manager",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "Microsoft.Extensions.Configuration.AzureKeyVault", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "Microsoft.Extensions.Configuration.AzureKeyVault",
            ProjectType = "Azure Key Vault",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "Microsoft.Extensions.Configuration.KeyPerFile", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "Key Per File",
            ProjectType = "Secret Management",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "Microsoft.Extensions.Configuration.Json", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "JSON File",
            ProjectType = "Secret Management",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "Amazon.Extensions.Configuration.SystemsManager", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "AWS Systems Manager Parameter Store",
            ProjectType = "AWS Secrets Manager",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "VaultSharp", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "VaultSharp",
            ProjectType = "Hashicorp Vault",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "SecretManager.ACS", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "Azure Container Secrets",
            ProjectType = "Azure Container Secrets",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "KeepassSharp", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "KeepassSharp",
            ProjectType = "KeePass Password Manager",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
    { "ConfKey", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "ConfKey",
            ProjectType = "Secret Management",
            BindingDirection = BindingDirection.Inbound.ToString()
        }
    },
#endregion

        { "Azure.Identity", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Azure.Identity",
        ProjectType = "Identity server",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    }},

        #region Monitoring
        {
        "Microsoft.ApplicationInsights.AspNetCore", new ApplicationInformation
        {
            ApplicationId = Guid.NewGuid(),
            ProjectName = "AppInsights",
            ProjectType = "Application Insights",
            BindingDirection = nameof(BindingDirection.Outbound)
        } },

        {"NSwag.AspNetCore", new ApplicationInformation
        {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "NSwag",
        ProjectType = "API Documentation (Swagger)",
        BindingDirection = "N/A" // Optional, as NSwag doesn't directly interact with monitoring
        }},
        {    "OpenTelemetry.Collector", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "OpenTelemetry Collector",
        ProjectType = "Telemetry Collector",
        BindingDirection = "Inbound"
    }},

        {  "Prometheus.AspNetCore", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Prometheus",
        ProjectType = "Metrics Monitoring",
        BindingDirection = "Outbound"
    }},
        { "Serilog.Sinks.Datadog", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Datadog Serilog Sink",
        ProjectType = "Datadog Monitoring",
        BindingDirection = "Outbound"
    }},{ "AppMetrics", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "AppMetrics",
        ProjectType = "Metrics Monitoring",
        BindingDirection = "Outbound"
    }},
#endregion

        #region DB Providers
        { "Microsoft.Data.SqlClient", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Persistence Layer",
        ProjectType = "SQL Server Database",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    }},

        { "Dapper", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Data Access Layer",
        ProjectType = "Micro ORM",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },

    // 2. Dapper.Contrib
    { "Dapper.Contrib", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Data Access Extensions",
        ProjectType = "Micro ORM Extensions",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },

    // 3. Entity Framework Core (EF Core)
    { "Microsoft.EntityFrameworkCore", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Data Access Layer",
        ProjectType = "Object-Relational Mapper (ORM)",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },

    // 4. Npgsql
    { "Npgsql", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "PostgreSQL Data Access",
        ProjectType = "ADO.NET Data Provider for PostgreSQL",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },

    // 6. MySqlConnector
    { "MySqlConnector", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "MySQL Data Access",
        ProjectType = "ADO.NET Data Provider for MySQL",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },

    // 7. ServiceStack.OrmLite
    { "ServiceStack.OrmLite", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Data Access Layer",
        ProjectType = "Lightweight ORM",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },

    // 8. Massive
    { "Massive", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Micro ORM Data Access",
        ProjectType = "Micro ORM",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },

    // 9. FreeSql
    { "FreeSql", new ApplicationInformation
    {
        ApplicationId = Guid.NewGuid(),
        ProjectName = "Data Access Layer",
        ProjectType = "Object-Relational Mapper (ORM)",
        BindingDirection = nameof(BindingDirection.Bidirectional)
    } },


#endregion
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
