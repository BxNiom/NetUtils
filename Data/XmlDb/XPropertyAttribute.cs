namespace BxNiom.Data.XmlDb;

[AttributeUsage(AttributeTargets.Property)]
public class XPropertyAttribute : Attribute {
    public XPropertyAttribute(string name, XPropertyType type) {
        Name = name;
        Type = type;
    }

    public string        Name { get; }
    public XPropertyType Type { get; }
}

public enum XPropertyType {
    Element,
    Attribute
}