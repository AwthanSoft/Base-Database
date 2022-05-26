using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mawa.RepositoryBase
{
    //public interface IModelRepository
    //{


    //}

    public interface IModelRepository<TModel>
        where TModel : Mawa.BaseDBCore.IDBModelCore
    {
        Task<TModel> AddAsync(TModel newModel);


    }


}
