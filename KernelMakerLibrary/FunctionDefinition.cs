using System.Text.Json;

namespace KernelMakerLibrary;

/// <summary>
/// The definition of a function in the type system.
/// </summary>
public class FunctionDefinition
{
    public FunctionSignature FunctionSignature { get; set; } = new();
    public string RawMethodBody { get; set; } = string.Empty;
    public string FunctionLanguage { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}