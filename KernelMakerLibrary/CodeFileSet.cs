using System.Runtime.Serialization;
using System.Text.Json;

namespace KernelMakerLibrary;

[DataContract]
public class CodeFileSet
{
    /// <summary>
    /// The object files related to the kernel.
    /// </summary>
    [DataMember]
    public List<CodeFile> ObjectCodeFiles { get; set; }
    /// <summary>
    /// The function files related to the kernel.
    /// </summary>
    [DataMember]
    public List<CodeFile> FunctionCodeFiles { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}