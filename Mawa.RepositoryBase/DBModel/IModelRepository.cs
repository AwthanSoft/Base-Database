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
        Task<AddModelOperationDBResult<TModel>> AddAsync(TModel newModel);
        ModelOperationDBResult<TModel> AddOrUpdate(TModel newModel);
        //OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating(TModel newModel);
        #endregion

        #region Update
        UpdateModelOperationDBResult<TModel>[] UpdateRange(TModel[] models);

        #endregion

        #region Delete

        //Delete
        Task<DeleteModelOperationDBResult<TModel>> DeleteAsync(TModel Model);
        //OperationWatingResult<AddModelOperationDBResult<TModel>> DeleteWating(TModel Model);

        DeleteModelOperationDBResult<TModel>[] RemoveRange(TModel[] Models);


        #endregion

        #region All

        TModel[] All();
        Task<TModel[]> AllAsync();

        #endregion

        #region Count

        int Count();
        //int CountAsync();

        #endregion

        #region Q FirstOrDefault

        TModel Q_FirstOrDefault(Expression<Func<TModel, bool>> predicate);
        Task<TModel> Q_FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate);
        //int Q_QWhere_Count(Expression<Func<TModel, bool>> predicate);

        #endregion

        #region Q Where

        TModel[] Q_Where(Expression<Func<TModel, bool>> predicate);

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
        new Task<AddModelOperationDBResult<TModelCore>> AddAsync(TModelCore newModelCore);
        new ModelOperationDBResult<TModelCore> AddOrUpdate(TModelCore newModelCore);

        //OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating(TModel newModel);
        #endregion

        #region Update

        new UpdateModelOperationDBResult<TModelCore>[] UpdateRange(TModelCore[] ModelsCore);

        #endregion

        #region Delete

        //Delete
        Task<DeleteModelOperationDBResult<TModelCore>> DeleteAsync(TModel Model);
        new DeleteModelOperationDBResult<TModelCore>[] RemoveRange(TModelCore[] ModelsCore);

        #endregion

        #region All
        new TModelCore[] All();

        #endregion

        #region Q FirstOrDefault

        TModelCore Q_FirstOrDefault(Expression<Func<TModel, bool>> predicate);
        Task<TModelCore> Q_FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate);

        //int Q_QWhere_Count(Expression<Func<TModel, bool>> predicate);


        #endregion

        #region Q Where

        new TModelCore[] Q_Where(Expression<Func<TModel, bool>> predicate);

        #endregion

    }
}
