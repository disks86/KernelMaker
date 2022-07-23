namespace KernelMakerLibrary;

public interface ILanguageHandler
{
    public string GenerateAssembly(UserOptions userOptions, KernelDefinition kernelDefinition,
        FunctionDefinition functionDefinition);
}