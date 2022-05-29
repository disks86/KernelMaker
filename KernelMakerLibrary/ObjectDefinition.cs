using System.Globalization;

namespace KernelMakerLibrary;

public class ObjectDefinition
{
    public string Name { get; set; }

    public List<ObjectPropertyDefinition> ObjectPropertyDefinitions { get; set; } =
        new List<ObjectPropertyDefinition>();

    ObjectDefinition(string name)
    {
        Name = name;
    }
}