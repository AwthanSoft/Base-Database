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


    public class EntityRepository<TModel> : IEntityRepository<TModel>
        where TModel : Mawa.BaseDBCore.EntityCore.IModelEntityCore
    {
        #region Initial 

        public EntityRepository(IDatabaseService dbService)
        {

        }


        #endregion


        public Task<TModel> AddAsync(TModel newModel)
        {
            throw new NotImplementedException();
        }
    }
}
