namespace BxNiom.Data.XmlDb;

public interface IXRepository<TEntity> where TEntity : XEntity {
    IEnumerable<TEntity?> Select(Func<TEntity?, bool>? filter = null);
    TEntity?              Scalar(Func<TEntity?, bool> selectFunc);

    bool Insert(TEntity entity);
    bool Insert(IEnumerable<TEntity> entities);

    bool Update(TEntity entity);
    bool Update(IEnumerable<TEntity> entities);

    bool Delete(TEntity entity);
    bool Delete(Func<TEntity?, bool> func);

    Task<IEnumerable<TEntity?>> SelectAsync(Func<TEntity?, bool>? filter = null,
                                            CancellationToken cancellationToken = default);

    Task<TEntity?> ScalarAsync(Func<TEntity?, bool> selectFunc, CancellationToken cancellationToken = default);
    Task<bool>     InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool>     InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<bool>     UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool>     UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<bool>     DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool>     DeleteAsync(Func<TEntity?, bool> func, CancellationToken cancellationToken = default);
}