using AppMe.ComponentModel.Waiting;
using Mawa.DBCore.NotifierCore;
using Mawa.RepositoryBase.DBs;
using Mawa.RepositoryBase.DBs.Results;
using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mawa.RepositoryBase
{
    public class ModelRepository<TModel, TModelCore, TId> : IModelRepository<TModel, TModelCore, TId>
         where TModelCore : class, BaseDBCore.IDBModelCore
         where TModel : TModelCore
    {
        #region Initial 

        readonly IDatabaseService dbService;

        
        public ModelRepository(IDatabaseService dbService)
        {
            //
            this.dbService = dbService;
            
            //
            _modelEventNotifier = new ModelEventNotifier<TModelCore, TId>();
            defaultNull = get_defaultNull();
            dbService.RegistModelNotifier(_modelEventNotifier);
            
            //
            pre_initial();
        }

        void pre_initial()
        {

        }


        #endregion

        #region Add

        

        #endregion

        #region Delete

       

        #endregion

        #region All




        #endregion


        #region Q Struct

        


        #endregion


        #region Notify Events
        //as temp
        TId get_defaultNull()
        {
            throw new NotImplementedException();
        }

        protected readonly TId defaultNull;
        private readonly ModelEventNotifier<TModelCore, TId> _modelEventNotifier;
        public ModelEventNotifier<TModelCore, TId> modelEventNotifier => _modelEventNotifier;

        #endregion

        public Task<DeleteModelOperationDBResult<TModelCore>> DeleteAsync(TModel Model)
        {
            throw new NotImplementedException();
        }

        public TModelCore Q_FirstOrDefault(Expression<Func<TModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TModelCore> Q_FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteModelOperationDBResult<TModelCore>> DeleteAsync(TModelCore Model)
        {
            throw new NotImplementedException();
        }

        public TModelCore Q_FirstOrDefault(Expression<Func<TModelCore, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<TModelCore> Q_FirstOrDefaultAsync(Expression<Func<TModelCore, bool>> predicate)
        {
            throw new NotImplementedException();
        }

       


    }
















    //public class ModelRepository<TModel> : IModelRepository<TModel>
    //   where TModel : class, BaseDBCore.IDBModelCore
    //{
    //    #region Initial 

    //    readonly IDatabaseService dbService;
    //    public ModelRepository(IDatabaseService dbService)
    //    {
    //        this.dbService = dbService;
    //        pre_initial();
    //    }

    //    void pre_initial()
    //    {

    //    }


    //    #endregion

    //    #region Add

    //    public Task<AddModelOperationDBResult<TModel>> AddAsync(TModel newModel)
    //    {
    //        return dbService.AddAsync(newModel);
    //    }

    //    public OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating(TModel newModel)
    //    {
    //        return dbService.AddWating(newModel);
    //    }

    //    #endregion

    //    #region Delete

    //    public Task<DeleteModelOperationDBResult<TModel>> DeleteAsync(TModel Model)
    //    {
    //        return dbService.DeleteAsync(Model);
    //    }

    //    #endregion

    //    #region All

    //    public TModel[] All()
    //    {
    //        return dbService.All<TModel>();
    //    }



    //    #endregion


    //    #region Q Struct

    //    //
    //    public TModel Q_FirstOrDefault(Expression<Func<TModel, bool>> predicate)
    //    {
    //        return dbService.Q_FirstOrDefault(predicate);
    //    }
    //    public async Task<TModel> Q_FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate)
    //    {
    //        return await dbService.Q_FirstOrDefaultAsync(predicate);
    //    }

    //    #endregion



    //}


}
