internal class Program
{
    private static void Main(string[] args)
    {
        var projectPath = "";

        //read all the folders in projectPath & find files with .csproj extension & package.json
        var projectFiles = Directory.GetFiles(projectPath, "*.csproj", SearchOption.AllDirectories);
        var packageJsonFiles = Directory.GetFiles(projectPath, "package.json", SearchOption.AllDirectories);
        foreach (var projectFile in projectFiles)
        {

        }
    }


    static void ComputeCsProjDependencies(string projectFile)
    {
        
    }   


    static void ComputePackageJsonDependencies(string packageJsonFile)
    {
        
    }
}