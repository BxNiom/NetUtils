namespace BxNiom.Data.XmlDb;

[AttributeUsage(AttributeTargets.Class)]
public class XEntityAttribute : Attribute {
    public XEntityAttribute(string rootPath, string elementName) {
        RootPath    = rootPath;
        ElementName = elementName;
    }

    public string RootPath    { get; }
    public string ElementName { get; }
}