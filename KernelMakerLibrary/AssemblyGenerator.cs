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
        var invalidPathChars = Path.GetInvalidPathChars();
        
        Parallel.ForEach(kernelDefinition.FunctionDefinitions,
            functionDefinition =>
            {
                if (functionDefinition.FunctionSignature.Name.IndexOfAny(invalidPathChars) >= 0)
                {
                    throw new ArgumentException($"Function name '{functionDefinition.FunctionSignature.Name}' contains invalid characters.");
                }
      
                if (functionDefinition.FunctionSignature.ReturnType.IndexOfAny(invalidPathChars) >= 0)
                {
                    throw new ArgumentException($"Function return type '{functionDefinition.FunctionSignature.ReturnType}' contains invalid characters.");
                }

                foreach (var functionArgument in functionDefinition.FunctionSignature.FunctionArguments)
                {
                    if (functionArgument.ArgumentType.IndexOfAny(invalidPathChars) >= 0)
                    {
                        throw new ArgumentException($"Function argument type '{functionArgument.ArgumentType}' contains invalid characters.");
                    } 
                }
                
                foreach (var languageHandler in LanguageHandlers)
                {
                    languageHandler.GenerateAssembly(userOptions, kernelDefinition, functionDefinition);
                }
            });
    }
}