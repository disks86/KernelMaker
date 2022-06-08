using System.Collections.Concurrent;
using System.Text.Json;

namespace KernelMakerLibrary;

public class KernelDefinition
{
    public BlockingCollection<TypeDefinition> TypeDefinitions { get; set; } = new();
    public BlockingCollection<FunctionDefinition> FunctionDefinitions { get; set; } = new();
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}