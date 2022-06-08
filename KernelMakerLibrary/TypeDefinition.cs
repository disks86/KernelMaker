using System.Globalization;

namespace KernelMakerLibrary;

/// <summary>
/// The definition of a type in the type system.
/// </summary>
public class TypeDefinition
{
    /// <summary>
    /// The name of the type in the type system.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The basic type this type is based on.
    /// </summary>
    public TypeType BaseType { get; set; }

    /// <summary>
    /// The in-memory sizes of the type based on platform.
    /// </summary>
    public Dictionary<PlatformType, int> TypeSizes { get; set; } = new();
  
    /// <summary>
    /// The in-memory endianness of the type based on platform.
    /// </summary>
    public Dictionary<PlatformType, EndianType> Endianness { get; set; } = new();
    
    /// <summary>
    /// The properties of a type in the type system.
    /// </summary>
    public List<TypePropertyDefinition> ObjectPropertyDefinitions { get; set; } = new();

    TypeDefinition(string name, TypeType baseType)
    {
        Name = name;
        BaseType = baseType;
    }
}