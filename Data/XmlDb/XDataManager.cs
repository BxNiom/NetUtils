using System.Xml.Linq;
using System.Xml.XPath;

namespace BxNiom.Data.XmlDb;

public abstract class XDataManager {
    protected XDataManager(XDocument document) {
        Document = document;
    }

    public XDocument Document { get; }

    public IXRepository<TEntity> CreateRepository<TEntity>() where TEntity : XEntity {
        var entityInfo = XEntityManager.GetInfo<TEntity>();
        var root       = Document.XPathSelectElement(entityInfo.Attribute.RootPath);

        if (root == null) {
            throw new InvalidDataException("Repository root path not found");
        }

        return new XRepository<TEntity>(root);
    }
}