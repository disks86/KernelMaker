namespace KernelMakerLibrary;

public class UserOptionProviderFactory
{
    public static IUserOptionProvider GetProvider()
    {
        return new BasicUserOptionProvider();
    }
}