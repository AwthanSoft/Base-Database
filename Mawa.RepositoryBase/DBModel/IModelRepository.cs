using AppMe.ComponentModel.Waiting;
using Mawa.RepositoryBase.DBs;
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
        //Add
        Task<AddModelOperationDBResult<TModel>> AddAsync(TModel newModel);
        OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating(TModel newModel);


        //Delete
        Task<DeleteModelOperationDBResult<TModel>> DeleteAsync(TModel Model);
        OperationWatingResult<AddModelOperationDBResult<TModel>> DeleteWating(TModel Model);

        Task<DeleteModelOperationDBResult<TModel>> DeleteAsync(TModel Model);
        OperationWatingResult<AddModelOperationDBResult<TModel>> DeleteWating(TModel Model);


        //All
        TModel[] All();
        Task<TModel[]> AllAsync();


    }


    public class ModelRepository<TModel> : IModelRepository<TModel>
       where TModel : class, BaseDBCore.IDBModelCore
    {
        #region Initial 

        readonly IDatabaseService dbService;
        public ModelRepository(IDatabaseService dbService)
        {
            this.dbService = dbService;
        }

        #endregion

        #region Add

        public Task<AddModelOperationDBResult<TModel>> AddAsync(TModel newModel)
        {
            return dbService.AddAsync(newModel);
        }

        public OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating(TModel newModel)
        {
            return dbService.AddWating(newModel);
        }



        #endregion

        #region All

        public TModel[] All()
        {
            throw new System.NotImplementedException();
        }

        public Task<TModel[]> AllAsync()
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }


}
