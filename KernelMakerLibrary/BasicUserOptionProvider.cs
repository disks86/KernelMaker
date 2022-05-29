namespace KernelMakerLibrary;

public class BasicUserOptionProvider
    : IUserOptionProvider
{
    public UserOptions GetUserOptions()
    {
        return new UserOptions();
    }
}