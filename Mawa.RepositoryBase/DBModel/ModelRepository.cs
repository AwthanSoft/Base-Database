using AppMe.ComponentModel.Waiting;
using Mawa.DBCore.NotifierCore;
using Mawa.RepositoryBase.DBs;
using Mawa.RepositoryBase.DBs.Results;
using Mawa.RepositoryBase.Extensions;
using Mawa.RepositoryBase.Helpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mawa.RepositoryBase
{


    public class ModelRepository<TModel, TModelCore, TId> : IModelRepository<TModel, TModelCore, TId>
         where TModelCore : class, BaseDBCore.IDBModelCore
         where TModel : class, TModelCore
    {
        #region Initial 

        readonly IDatabaseService dbService;

        
        public ModelRepository(IDatabaseService dbService)
        {
            //
            this.dbService = dbService;
            
            //
            _modelEventNotifier = new ModelEventNotifier<TModelCore, TId>();
            defaultNull = (TId)get_defaultNull<TId>();
            dbService.RegistModelNotifier(_modelEventNotifier);
            
            //
            pre_initial();
        }

        void pre_initial()
        {

        }


        #endregion

        #region Add

        public async Task<AddModelOperationDBResult<TModelCore>> AddAsync(TModel newModel)
        {
            var resultt = await dbService.AddAsync<TModel, TModelCore>(newModel);
            if(resultt.State == EntityState.Added)
            {
                dbService.modelNotifierControlsManager.ModelNotify<TModelCore, TId>(DBModelNotifierType.Insert, resultt.Entity, defaultNull);
            }
            return resultt;
        }

        public Task<AddModelOperationDBResult<TModelCore>> AddAsync(TModelCore newModelCore)
        {
            var newModel = ParentChildCopyingHelper.CopyToChild<TModelCore, TModel>(newModelCore);
            return AddAsync(newModel);
        }


        #endregion

        #region Delete



        #endregion

        #region All

        public TModelCore[] All()
        {
            return dbService.All<TModel>()
                .Select(b => (TModelCore)b)
                .ToArray();
        }
        public async Task<TModelCore[]> AllAsync()
        {
            return (await dbService.AllAsync<TModel>())
                .Select(b => (TModelCore)b)
                .ToArray();
        }


        #endregion

        #region Count
        public int Count()
        {
            return dbService.Count<TModel>();
        }

        #endregion

        #region Q Struct

        //
        public TModelCore Q_FirstOrDefault(Expression<Func<TModel, bool>> predicate)
        {
            return dbService.Q_FirstOrDefault<TModel>(predicate);
        }

        public TModelCore Q_FirstOrDefault(Expression<Func<TModelCore, bool>> predicate)
        {
            //https://stackoverflow.com/questions/821365/how-to-convert-a-string-to-its-equivalent-linq-expression-tree
            //var tt = predicate.Simplify();
            //var exp = tt.ToString();

            //var predicate2 = tt as Expression<Func<TModel, bool>>;

            //var p = Expression.Parameter(typeof(TModelCore), "b");
            ////var e = System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { p }, null, exp);
            //var e = System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { p }, null, exp);

            //var lambda = Expression.Lambda<Func<TModel, bool>>(tt, Expression.Parameter(typeof(TModel), "b"));

            //var exp2 = lambda.ToString();
            //var Simplify22 = lambda.Simplify();

            //return Q_FirstOrDefault(e);

            return dbService.Q_FirstOrDefault<TModel, TModelCore>(predicate);

        }

        public Task<TModelCore> Q_FirstOrDefaultAsync(Expression<Func<TModelCore, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Notify Events
        //as temp
        object get_defaultNull<T>()
        {
            if(typeof(T) == typeof(string))
            {
                return null;
            }

            if (typeof(T) == typeof(int))
            {
                return int.MinValue;
            }

            return null;
        }

        protected readonly TId defaultNull;
        private readonly ModelEventNotifier<TModelCore, TId> _modelEventNotifier;
        public ModelEventNotifier<TModelCore, TId> modelEventNotifier => _modelEventNotifier;

        #endregion

        public Task<DeleteModelOperationDBResult<TModelCore>> DeleteAsync(TModel Model)
        {
            throw new NotImplementedException();
        }


        

        public Task<DeleteModelOperationDBResult<TModelCore>> DeleteAsync(TModelCore Model)
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



    //    #endregion



    //}


}
