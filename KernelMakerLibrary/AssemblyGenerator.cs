using Serilog;

namespace KernelMakerLibrary;

public class AssemblyGenerator
{
    private static readonly List<ILanguageHandler> LanguageHandlers = new();

    static AssemblyGenerator()
    {
        var type = typeof(ILanguageHandler);
        var languageHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && type != p);
        foreach (var languageHandlerType in languageHandlerTypes)
        {
            if (Activator.CreateInstance(languageHandlerType) is ILanguageHandler languageHandler)
            {
                LanguageHandlers.Add(languageHandler);
            }
        }
    }
    
    /// <summary>
    /// The options provided by the user.
    /// </summary>
    public UserOptions UserOptions { get; set; }
    
    /// <summary>
    /// The script runner for executing user provided scripts.
    /// </summary>
    public ScriptRunner ScriptRunner { get; set; }
    
    public AssemblyGenerator(UserOptions userOptions, ScriptRunner scriptRunner)
    {
        UserOptions = userOptions;
        ScriptRunner = scriptRunner;
    }
    
    public void GenerateAssembly(KernelDefinition kernelDefinition)
    {
        var invalidPathChars = Path.GetInvalidPathChars();

        Log.Information("Generating assembly for functions");
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
                    languageHandler.GenerateAssembly(UserOptions, kernelDefinition, functionDefinition);
                }
            });
    }
}