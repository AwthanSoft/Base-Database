using Mawa.BaseDBCore.EntityCore;
using Mawa.RepositoryBase.DBs;

namespace Mawa.RepositoryBase.DBEntity
{
    public interface IEntityRepository<TEntity> : IModelRepository<TEntity>
        where TEntity : class, IModelEntityCore
    {

    }
    public interface IEntityRepository<TEntity, TId> : IEntityRepository<TEntity>, IModelRepository<TEntity, TId>
        where TEntity : class, IModelEntityCore
    {

    }
    public interface IEntityRepository<TEntity, TEntityCore, TId> : IEntityRepository<TEntityCore, TId>, IModelRepository<TEntity, TEntityCore, TId>
        where TEntityCore : class, IModelEntityCore
        where TEntity : TEntityCore
    {

    }


    public class EntityRepository<TEntity, TEntityCore, TId> : ModelRepository<TEntity, TEntityCore, TId>, IEntityRepository<TEntity, TEntityCore, TId>
        where TEntityCore : class, IModelEntityCore
        where TEntity : class, TEntityCore
    {

        #region Initial 

        readonly IDatabaseService dbService;
        public EntityRepository(IDatabaseService dbService) : base(dbService)
        {
            this.dbService = dbService;
        }

        #endregion

    }
}
