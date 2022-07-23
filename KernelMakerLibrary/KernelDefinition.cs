using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace KernelMakerLibrary;

public class KernelDefinition
{
    public BlockingCollection<TypeDefinition> TypeDefinitions { get; set; } = new();
    public BlockingCollection<FunctionDefinition> FunctionDefinitions { get; set; } = new();

    public BlockingCollection<string> AssemblyFiles { get; set; } = new();
    
    public BlockingCollection<string> AssembledFiles { get; set; } = new();

    public string BootFilename { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}