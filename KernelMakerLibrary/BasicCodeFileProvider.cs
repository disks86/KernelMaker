namespace KernelMakerLibrary;

public class BasicCodeFileProvider
    : ICodeFileProvider
{
    public CodeFileSet GetCodeFiles(UserOptions userOptions)
    {
        var rootPath = userOptions.RootPath ?? Environment.CurrentDirectory;

        CodeFileSet codeFileSet = new();

        var objectFiles = Directory.GetFiles(rootPath, "*.odef", SearchOption.AllDirectories);
        foreach (var objectFile in objectFiles)
        {
            var name = objectFile.Replace(rootPath, "").Replace(Path.PathSeparator, ':').Split(".")[0];
            codeFileSet.ObjectCodeFiles.Add(new CodeFile(name, File.ReadAllText(objectFile)));
        }

        var functionFiles = Directory.GetFiles(rootPath, "*.fdef", SearchOption.AllDirectories);
        foreach (var functionFile in functionFiles)
        {
            var name = functionFile.Replace(rootPath, "").Replace(Path.PathSeparator, ':').Split(".")[0];
            codeFileSet.ObjectCodeFiles.Add(new CodeFile(name, File.ReadAllText(functionFile)));
        }

        var remotePackageCachePath = userOptions.RemotePackageCachePath;
        if (remotePackageCachePath != null)
        {
            var remoteObjectFiles = Directory.GetFiles(remotePackageCachePath, "*.odef", SearchOption.AllDirectories);
            foreach (var remoteObjectFile in remoteObjectFiles)
            {
                var name = remoteObjectFile.Replace(remotePackageCachePath, "").Replace(Path.PathSeparator, ':').Split(".")[0];
                codeFileSet.ObjectCodeFiles.Add(new CodeFile(name, File.ReadAllText(remoteObjectFile)));
            }

            var remoteFunctionFiles = Directory.GetFiles(remotePackageCachePath, "*.fdef", SearchOption.AllDirectories);
            foreach (var remoteFunctionFile in remoteFunctionFiles)
            {
                var name = remoteFunctionFile.Replace(remotePackageCachePath, "").Replace(Path.PathSeparator, ':').Split(".")[0];
                codeFileSet.ObjectCodeFiles.Add(new CodeFile(name, File.ReadAllText(remoteFunctionFile)));
            }
        }

        return codeFileSet;
    }
}