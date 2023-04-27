using System.Xml.Linq;

namespace BxNiom.Data.XmlDb;

public static class XElementExtensions {
    public static string AttributeValue(this XElement? element, string attribute, string defaultValue = "") {
        if (element == null) {
            return defaultValue;
        }

        var elementAttribute = element.Attribute(attribute);
        return elementAttribute == null ? defaultValue : elementAttribute.Value;
    }

    public static bool TryGetAttributeValue(this XElement? element, string attribute, out string value) {
        value = "";
        if (element == null) {
            return false;
        }

        var elementAttribute = element.Attribute(attribute);
        if (elementAttribute == null) {
            return false;
        }

        value = elementAttribute.Value;
        return true;
    }
}