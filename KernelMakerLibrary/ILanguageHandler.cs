namespace KernelMakerLibrary;

public interface ILanguageHandler
{
    public void GenerateAssembly(UserOptions userOptions, KernelDefinition kernelDefinition,
        FunctionDefinition functionDefinition);
}