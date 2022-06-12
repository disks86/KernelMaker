namespace KernelMakerLibrary;

public class AssemblyGenerator
{
    private readonly List<ILanguageHandler> LanguageHandlers = new();

    public AssemblyGenerator()
    {
        var type = typeof(ILanguageHandler);
        var languageHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && type != p);
        foreach (var languageHandlerType in languageHandlerTypes)
        {
            var languageHandler = Activator.CreateInstance(languageHandlerType) as ILanguageHandler;
            if (languageHandler!=null)
            {
                LanguageHandlers.Add(languageHandler);
            }
        }
    }
    
    public void GenerateAssembly(UserOptions userOptions, KernelDefinition kernelDefinition)
    {
        foreach (var functionDefinition in kernelDefinition.FunctionDefinitions)
        {
            foreach (var languageHandler in LanguageHandlers)
            {
                languageHandler.GenerateAssembly(userOptions, kernelDefinition, functionDefinition);
            }
        }
    }
}