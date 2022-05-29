using Serilog;

namespace KernelMakerLibrary;

public class Parser
{
    /// <summary>
    /// The options provided by the user.
    /// </summary>
    public UserOptions UserOptions { get; set; }
    /// <summary>
    /// The code file provider which is used to fetch local code files.
    /// </summary>
    public ICodeFileProvider CodeFileProvider { get; set; }
    /// <summary>
    /// The remote code file provider which is used to download remote code files.
    /// </summary>
    public IRemoteCodeFileProvider RemoteCodeFileProvider { get; set; }
    public Parser(UserOptions userOptions)
    {
        UserOptions = userOptions;
        CodeFileProvider = CodeFileProviderFactory.GetProvider(userOptions);
        RemoteCodeFileProvider = RemoteCodeFileProviderFactory.GetProvider(userOptions);
    }

    public KernelDefinition Parse()
    {
        RemoteCodeFileProvider.DownloadRemoteCodeFiles(UserOptions);
        
        var codeFiles = CodeFileProvider.GetCodeFiles(UserOptions);
        Log.Information("{CodeFiles}",codeFiles);

        KernelDefinition kernelDefinition = new();

        //TODO: do parse local files.

        return kernelDefinition;
    }
}