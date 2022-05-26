using AppMe.ComponentModel.Waiting;
using Mawa.RepositoryBase.DBs.Results;
using System.Text;
using System.Threading.Tasks;

namespace Mawa.RepositoryBase
{
    //public interface IModelRepository
    //{


    //}

    public interface IModelRepository<TModel>
        where TModel : BaseDBCore.IDBModelCore
    {
        Task<AddModelOperationDBResult<TModel>> AddAsync(TModel newModel);

        Task<OperationWatingResult<AddModelOperationDBResult<TModel>>> AddWating(TModel newModel);

    }


}
