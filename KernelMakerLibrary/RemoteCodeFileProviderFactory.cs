namespace KernelMakerLibrary;

public static class RemoteCodeFileProviderFactory
{
    public static IRemoteCodeFileProvider GetProvider(UserOptions userOptions)
    {
        return new BasicRemoteCodeFileProvider();
    }
}