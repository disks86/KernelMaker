namespace KernelMakerLibrary;

/// <summary>
/// A single property/field of an object definition.
/// </summary>
public class TypePropertyDefinition
{
    /// <summary>
    /// The name of the property/field.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The data type of the property.field.
    /// </summary>
    public string Type { get; set; }

    TypePropertyDefinition(string name, string type)
    {
        Name = name;
        Type = type;
    }
}