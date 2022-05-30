using System.Runtime.Serialization;
using System.Text.Json;

namespace KernelMakerLibrary;

[DataContract]
public class CodeFileSet
{
    /// <summary>
    /// The type files related to the kernel.
    /// </summary>
    [DataMember]
    public List<CodeFile> TypeCodeFiles { get; set; } = new();
    /// <summary>
    /// The function files related to the kernel.
    /// </summary>
    [DataMember]
    public List<CodeFile> FunctionCodeFiles { get; set; } = new();
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}