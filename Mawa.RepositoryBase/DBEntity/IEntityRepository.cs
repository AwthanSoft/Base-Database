using Mawa.BaseDBCore.EntityCore;
using Mawa.RepositoryBase.DBs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mawa.RepositoryBase.DBEntity
{
    //public interface IEntityRepository : IModelRepository
    //{

    //}
    public interface IEntityRepository<TEntity> : IModelRepository<TEntity>
        where TEntity : class, IModelEntityCore
    {
        

    }


    public class EntityRepository<TEntity> : ModelRepository<TEntity>, IEntityRepository<TEntity>
        where TEntity : class, IModelEntityCore
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
