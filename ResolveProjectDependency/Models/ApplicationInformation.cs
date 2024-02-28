using System.ComponentModel;

public class ApplicationInformation
{
    public Guid ApplicationId { get; set; }
    public string ProjectType { get; set; }
    public string ProjectName { get; set; }
    public List<PackageInformation> AppDependencies { get; set; }
    public string Version { get; set; }
    public BindingDirection BindingDirection { get; set; }
    public Guid ParentAppId { get; internal set; }
}

public class PackageInformation
{
    public string PackageName { get; set; }
    public string Version { get; set; }
}
