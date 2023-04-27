using System.Xml.Linq;

namespace BxNiom.Data.XmlDb;

public class XRepository<TEntity> : IXRepository<TEntity> where TEntity : XEntity {
    private readonly XEntityInfo _entityInfo;
    private readonly XElement    _rootElement;

    internal XRepository(XElement rootElement) {
        _rootElement = rootElement;
        _entityInfo  = XEntityManager.GetInfo<TEntity>();
    }

    public IEnumerable<TEntity?> Select(Func<TEntity?, bool>? filter = null) {
        return from xe in _rootElement.Descendants()
               where filter?.Invoke(ElementToEntity(xe)) ?? true
               select ElementToEntity(xe);
    }

    public TEntity? Scalar(Func<TEntity?, bool> func) {
        return ElementToEntity((from xe in _rootElement.Descendants()
                                where func(ElementToEntity(xe))
                                select xe).FirstOrDefault());
    }

    public bool Insert(TEntity entity) {
        if (entity.Element != null) {
            throw new InvalidDataException("Entity already inserted. Use update");
        }

        try {
            var element = new XElement(_entityInfo.Attribute.ElementName);
            ApplyChanges(entity, element);
            _rootElement.Add(element);
            entity.Element = element;
            return true;
        }
        catch (Exception) {
            return false;
        }
    }

    public bool Insert(IEnumerable<TEntity> entities) {
        foreach (var entity in entities) {
            if (!Insert(entity)) {
                return false;
            }
        }

        return true;
    }

    public bool Update(TEntity entity) {
        if (entity.Element == null) {
            throw new InvalidDataException("Could not update an entity that is not inserted");
        }

        ApplyChanges(entity, entity.Element);
        return true;
    }

    public bool Update(IEnumerable<TEntity> entities) {
        foreach (var entity in entities) {
            Update(entity);
        }

        return true;
    }

    public bool Delete(TEntity entity) {
        if (entity.Element == null) {
            throw new InvalidDataException("Could not delete an entity that is not inserted");
        }

        entity.Element.Remove();
        return true;
    }

    public bool Delete(Func<TEntity?, bool> func) {
        var removeList =
            from xe in _rootElement.Descendants()
            where func.Invoke(ElementToEntity(xe))
            select xe;

        foreach (var element in removeList) {
            element.Remove();
        }

        return true;
    }

    public async Task<IEnumerable<TEntity?>> SelectAsync(Func<TEntity?, bool>? filter = null,
                                                         CancellationToken cancellationToken = default) {
        return await Task.Run(() => Select(filter), cancellationToken);
    }

    public async Task<TEntity?> ScalarAsync(Func<TEntity?, bool> selectFunc,
                                            CancellationToken cancellationToken = default) {
        return await Task.Run(() => Scalar(selectFunc), cancellationToken);
    }

    public async Task<bool> InsertAsync(TEntity entity, CancellationToken cancellationToken = default) {
        return await Task.Run(() => Insert(entity), cancellationToken);
    }

    public async Task<bool> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
        return await Task.Run(() => Insert(entities), cancellationToken);
    }

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default) {
        return await Task.Run(() => Update(entity), cancellationToken);
    }

    public async Task<bool> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) {
        return await Task.Run(() => Update(entities), cancellationToken);
    }

    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default) {
        return await Task.Run(() => Delete(entity), cancellationToken);
    }

    public async Task<bool> DeleteAsync(Func<TEntity?, bool> func, CancellationToken cancellationToken = default) {
        return await Task.Run(() => Delete(func), cancellationToken);
    }

    public static Func<TEntity?, bool> HashFilter(string hash) {
        return element => (element?.Hash ?? "").Equals(hash, StringComparison.OrdinalIgnoreCase);
    }

    private TEntity? ElementToEntity(XElement? element) {
        if (element == null) {
            return null;
        }

        var entity = (TEntity)_entityInfo.Constructor.Invoke(Array.Empty<object?>());

        foreach (var info in _entityInfo.Properties) {
            object? value = info.PropertyAttribute.Type switch {
                XPropertyType.Element   => element.Element(info.PropertyAttribute.Name)?.Value ?? null,
                XPropertyType.Attribute => element.AttributeValue(info.PropertyAttribute.Name),
                _                       => null
            };

            if (value == null) {
                continue;
            }

            if (info.PropertyInfo.PropertyType == typeof(string)) {
                info.PropertyInfo.SetValue(entity, value);
            } else if (info.PropertyInfo.PropertyType == typeof(short)
                       && short.TryParse(value.ToString(), out var sh)) {
                info.PropertyInfo.SetValue(entity, sh);
            } else if (info.PropertyInfo.PropertyType == typeof(int)
                       && int.TryParse(value.ToString(), out var i)) {
                info.PropertyInfo.SetValue(entity, i);
            } else if (info.PropertyInfo.PropertyType == typeof(long)
                       && long.TryParse(value.ToString(), out var lo)) {
                info.PropertyInfo.SetValue(entity, lo);
            } else if (info.PropertyInfo.PropertyType == typeof(float)
                       && float.TryParse(value.ToString(), out var fl)) {
                info.PropertyInfo.SetValue(entity, fl);
            } else if (info.PropertyInfo.PropertyType == typeof(double)
                       && double.TryParse(value.ToString(), out var d)) {
                info.PropertyInfo.SetValue(entity, d);
            } else if (info.PropertyInfo.PropertyType == typeof(DateTime)
                       && DateTime.TryParse(value.ToString(), out var dt)) {
                info.PropertyInfo.SetValue(entity, dt);
            } else if (info.PropertyInfo.PropertyType == typeof(bool)
                       && bool.TryParse(value.ToString(), out var b)) {
                info.PropertyInfo.SetValue(entity, b);
            }
        }

        if (!string.IsNullOrEmpty(entity.Hash)) {
            entity.Element = element;
        }

        return entity;
    }

    private void ApplyChanges(TEntity entity, XElement element) {
        foreach (var info in _entityInfo.Properties) {
            switch (info.PropertyAttribute.Type) {
                case XPropertyType.Attribute:
                    element.SetAttributeValue(info.PropertyAttribute.Name, info.PropertyInfo.GetValue(entity));
                    break;
                case XPropertyType.Element:
                    element.SetElementValue(info.PropertyAttribute.Name, info.PropertyInfo.GetValue(entity));
                    break;
                default: break;
            }
        }
    }
}