using AppMe.ComponentModel.Waiting;
using Mawa.BaseDBCore.EntityCore;
using Mawa.RepositoryBase.DBs;
using Mawa.RepositoryBase.DBs.Results;
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
        where TEntity : IModelEntityCore
    {
        

    }


    public class EntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : IModelEntityCore
    {
        #region Initial 

        readonly IDatabaseService dbService;
        public EntityRepository(IDatabaseService dbService)
        {
            this.dbService = dbService;
        }

        #endregion

        #region Add

        public Task<AddModelOperationDBResult<TEntity>> AddAsync(TEntity newModel)
        {
            return dbService.AddAsync(newModel);
        }

        public Task<OperationWatingResult<AddModelOperationDBResult<TEntity>>> AddWating(TEntity newModel)
        {
            return dbService.AddWating(newModel);
        }

        #endregion

    }
}
