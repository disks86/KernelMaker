using System.Text.Json;

namespace KernelMakerLibrary;

/// <summary>
/// The signature of a function in the type system.
/// </summary>
public class FunctionSignature
{
    /// <summary>
    /// The name of the function in the type system.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The name of the return type in the type system.
    /// </summary>
    public string ReturnType { get; set; } = string.Empty;

    /// <summary>
    /// The arguments to the function.
    /// </summary>
    public List<FunctionArgument> FunctionArguments { get; set; } = new();
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}