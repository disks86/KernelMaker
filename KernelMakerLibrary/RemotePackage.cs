using System.Runtime.Serialization;
using System.Text.Json;

namespace KernelMakerLibrary;


/// <summary>
/// A 3rd party package which can be fetched from a remote server if it is not cached locally.
/// </summary>
[DataContract]
public class RemotePackage
{
    /// <summary>
    /// The name of the remote package
    /// </summary>
    [DataMember]
    public string Name { get; set; }
    /// <summary>
    /// The url where the remote package can be downloaded.
    /// </summary>
    [DataMember]
    public string Url { get; set; }

    public RemotePackage(string name, string url)
    {
        Name = name;
        Url = url;
    }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}