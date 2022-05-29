namespace KernelMakerLibrary;

public static class CodeFileProviderFactory
{
    public static ICodeFileProvider GetProvider(UserOptions userOptions)
    {
        return new BasicCodeFileProvider();
    }
}