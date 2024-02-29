using System.ComponentModel;

public class ApplicationInformation
{
    public Guid ApplicationId { get; set; }
    public string ProjectType { get; set; }
    public string ProjectName { get; set; }
    public string? BinaryFileName { get; set;}
    public string Version { get; set; }
    public string BindingDirection { get; set; }
    public Guid ParentAppId { get; internal set; }
}

public class PackageInformation
{
    public string PackageName { get; set; }
    public string Version { get; set; }
}