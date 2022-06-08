using System.Text.Json;

namespace KernelMakerLibrary;

/// <summary>
/// An argument to a function.
/// </summary>
public class FunctionArgument
{
    /// <summary>
    /// The name of the argument.
    /// </summary>
    public string ArgumentName { get; set; } = string.Empty;

    /// <summary>
    /// The name of the type of the argument in the type system.
    /// </summary>
    public string ArgumentType { get; set; } = string.Empty;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}