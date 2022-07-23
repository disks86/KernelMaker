namespace KernelMakerLibrary;

public class AssemblerFactory
{
    public static IAssembler GetAssembler(UserOptions userOptions)
    {
        return new NasmAssembler();
    }
}