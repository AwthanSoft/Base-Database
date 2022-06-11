using Mawa.RepositoryBase.DBs.Results;
using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mawa.RepositoryBase
{
    public interface IModelRepository<TModel>
        where TModel : BaseDBCore.IDBModelCore
    {
        #region Add
        //Add
        //Task<AddModelOperationDBResult<TModel>> AddAsync(TModel newModel);
        //OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating(TModel newModel);
        #endregion

        #region Delete

        //Delete
        Task<DeleteModelOperationDBResult<TModel>> DeleteAsync(TModel Model);
        //OperationWatingResult<AddModelOperationDBResult<TModel>> DeleteWating(TModel Model);

        #endregion

        //All
        //TModel[] All();
        //Task<TModel[]> AllAsync();

        //Count
        //int Count();
        //int CountAsync();

        #region Struct Q

        TModel Q_FirstOrDefault(Expression<Func<TModel, bool>> predicate);
        Task<TModel> Q_FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate);

        //int Q_QWhere_Count(Expression<Func<TModel, bool>> predicate);


        #endregion
    }

    public interface IModelRepository<TModel, TId> : IModelRepository<TModel>
          where TModel : class, BaseDBCore.IDBModelCore
    {
        DBCore.NotifierCore.ModelEventNotifier<TModel, TId> modelEventNotifier { get; }
    }

    public interface IModelRepository<TModel, TModelCore, TId> : IModelRepository<TModelCore, TId>
        where TModelCore : class, BaseDBCore.IDBModelCore
        where TModel : TModelCore
    {
        #region Add
        //Add
        //Task<AddModelOperationDBResult<TModel>> AddAsync(TModel newModel);
        //OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating(TModel newModel);
        #endregion

        #region Delete

        //Delete
        Task<DeleteModelOperationDBResult<TModelCore>> DeleteAsync(TModel Model);

        #endregion


        //All
        //TModel[] All();
        //Task<TModel[]> AllAsync();

        //Count
        //int Count();
        //int CountAsync();


        #region Struct Q

        TModelCore Q_FirstOrDefault(Expression<Func<TModel, bool>> predicate);
        Task<TModelCore> Q_FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate);

        //int Q_QWhere_Count(Expression<Func<TModel, bool>> predicate);


        #endregion
    }
}
