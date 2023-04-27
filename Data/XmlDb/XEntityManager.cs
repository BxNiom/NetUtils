using System.Reflection;

namespace BxNiom.Data.XmlDb;

public record XEntityPropertyInfo(PropertyInfo PropertyInfo, XPropertyAttribute PropertyAttribute);

public record XEntityInfo(XEntityAttribute Attribute, XEntityPropertyInfo[] Properties, ConstructorInfo Constructor);

public class XEntityManager {
    private static Dictionary<Type, XEntityInfo> _infos = new();

    public static XEntityInfo GetInfo<T>() where T : XEntity {
        if (!_infos.ContainsKey(typeof(T))) {
            var entityType = typeof(T);

            var ctor = entityType.GetConstructor(Array.Empty<Type>());
            if (ctor == null) {
                throw new InvalidDataException($"\"{entityType}\" has no empty constructor");
            }

            var entityAttribute = entityType.GetCustomAttribute<XEntityAttribute>();
            if (entityAttribute == null) {
                throw new InvalidDataException($"\"{entityType}\" has no XEntity attribute");
            }

            List<XEntityPropertyInfo> propertyInfos = new();
            foreach (var property in entityType.GetProperties()) {
                var propertyAttribute = property.GetCustomAttribute<XPropertyAttribute>();
                if (propertyAttribute == null) {
                    continue;
                }

                propertyInfos.Add(new XEntityPropertyInfo(property, propertyAttribute));
            }

            _infos.Add(entityType, new XEntityInfo(entityAttribute, propertyInfos.ToArray(), ctor));
        }

        return _infos[typeof(T)];
    }
}