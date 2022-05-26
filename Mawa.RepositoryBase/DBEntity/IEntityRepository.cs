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
    public interface IEntityRepository<TModel> : IModelRepository<TModel>
        where TModel : Mawa.BaseDBCore.EntityCore.IModelEntityCore
    {


    }


    public class EntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : Mawa.BaseDBCore.EntityCore.IModelEntityCore
    {
        #region Initial 
        readonly IDatabaseService dbService;
        public EntityRepository(IDatabaseService dbService)
        {
            this.dbService = dbService;
        }


        #endregion


        public Task<TEntity> AddAsync(TEntity newModel)
        {
            throw new NotImplementedException();
        }
    }
}
