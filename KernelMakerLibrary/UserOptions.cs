using System.Runtime.Serialization;
using System.Text.Json;

namespace KernelMakerLibrary;

/// <summary>
/// Settings provided by the user.
/// </summary>
[DataContract]
public class UserOptions
{
    /// <summary>
    /// The remote packages provided by the user.
    /// </summary>
    [DataMember]
    public List<RemotePackage> RemotePackages { get; set; } = new List<RemotePackage>();

    /// <summary>
    /// The root location to start looking for code files.
    /// </summary>
    public string? RootPath { get; set; }

    /// <summary>
    /// The path to the local copy of remote packages.
    /// </summary>
    public string? RemotePackageCachePath { get; set; }
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}