using System.Runtime.Serialization;
using System.Text.Json;

namespace KernelMakerLibrary;

/// <summary>
/// A single code file containing code or objects to be parsed.
/// </summary>
[DataContract]
public class CodeFile
{
    /// <summary>
    /// The name of the code file.
    /// </summary>
    [DataMember]
    public string Name { get; set; }
    
    /// <summary>
    /// The contents of the code file.
    /// </summary>
    [DataMember]
    public string Content { get; set; }

    public CodeFile(string name, string content)
    {
        Name = name;
        Content = content;
    }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}