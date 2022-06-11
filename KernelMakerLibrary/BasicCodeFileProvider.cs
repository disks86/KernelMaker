namespace KernelMakerLibrary;

public class BasicCodeFileProvider
    : ICodeFileProvider
{
    public CodeFileSet GetCodeFiles(UserOptions userOptions)
    {
        CodeFileSet codeFileSet = new();

        var rootPath = userOptions.RootPath;
        if (rootPath != null && Directory.Exists(rootPath))
        {
            var typeFiles = Directory.GetFiles(rootPath, "*.tdef", SearchOption.AllDirectories);
            foreach (var typeFile in typeFiles)
            {
                var name = typeFile.Replace(rootPath, "").Replace(Path.PathSeparator, ':').Split(".")[0];
                codeFileSet.TypeCodeFiles.Add(new CodeFile(name, File.ReadAllText(typeFile)));
            }

            var functionFiles = Directory.GetFiles(rootPath, "*.fdef", SearchOption.AllDirectories);
            foreach (var functionFile in functionFiles)
            {
                var name = functionFile.Replace(rootPath, "").Replace(Path.PathSeparator, ':').Split(".")[0];
                codeFileSet.FunctionCodeFiles.Add(new CodeFile(name, File.ReadAllText(functionFile)));
            }
        }

        var remotePackageCachePath = userOptions.RemotePackageCachePath;
        if (remotePackageCachePath != null && Directory.Exists(remotePackageCachePath))
        {
            var remoteTypeFiles = Directory.GetFiles(remotePackageCachePath, "*.tdef", SearchOption.AllDirectories);
            foreach (var remoteTypeFile in remoteTypeFiles)
            {
                var name = remoteTypeFile.Replace(remotePackageCachePath, "").Replace(Path.PathSeparator, ':')
                    .Split(".")[0];
                codeFileSet.TypeCodeFiles.Add(new CodeFile(name, File.ReadAllText(remoteTypeFile)));
            }

            var remoteFunctionFiles =
                Directory.GetFiles(remotePackageCachePath, "*.fdef", SearchOption.AllDirectories);
            foreach (var remoteFunctionFile in remoteFunctionFiles)
            {
                var name = remoteFunctionFile.Replace(remotePackageCachePath, "").Replace(Path.PathSeparator, ':')
                    .Split(".")[0];
                codeFileSet.FunctionCodeFiles.Add(new CodeFile(name, File.ReadAllText(remoteFunctionFile)));
            }
        }

        return codeFileSet;
    }
}