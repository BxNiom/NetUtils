using System.Xml.Linq;

namespace BxNiom.Data.XmlDb;

public abstract class XEntity {
    private XElement? _element;

    [XProperty("hash", XPropertyType.Attribute)]
    public string Hash { get; set; } = "";

    public XElement? Element {
        get => _element;
        internal set {
            if (string.IsNullOrEmpty(Hash)) {
                throw new InvalidDataException("No hash set");
            }

            _element = value;
        }
    }
}