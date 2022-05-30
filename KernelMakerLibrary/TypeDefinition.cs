using System.Globalization;

namespace KernelMakerLibrary;

public class TypeDefinition
{
    public string Name { get; set; }

    public List<TypePropertyDefinition> ObjectPropertyDefinitions { get; set; } = new();

    TypeDefinition(string name)
    {
        Name = name;
    }
}