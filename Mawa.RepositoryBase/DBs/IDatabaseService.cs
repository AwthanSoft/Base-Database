using AppMe.ComponentModel.Waiting;
using Mawa.BaseDBCore;
using Mawa.DBCore.NotifierCore;
using Mawa.RepositoryBase.DBs.Results;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mawa.RepositoryBase.DBs
{
    public interface IDatabaseService
    {
        #region Add

        Task<AddModelOperationDBResult<TModel>> AddAsync<TModel>(TModel newModel)
            where TModel : class, IDBModelCore;

        OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating<TModel>(TModel newModel)
            where TModel : class, IDBModelCore;

        //Task<OperationWatingResult<AddModelOperationDBResult<TEntity>>> AddWatingAsync<TEntity>(TEntity newModel)
        //    where TEntity : IModelEntityCore;

        #endregion

        #region Delete

        Task<DeleteModelOperationDBResult<TModel>> DeleteAsync<TModel>(TModel newModel)
            where TModel : class, IDBModelCore;

        #endregion

        #region All

        TModel[] All<TModel>() where TModel : class, IDBModelCore;

        #endregion

        #region Q Struct

        TModel Q_FirstOrDefault<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class, IDBModelCore;
        Task<TModel> Q_FirstOrDefaultAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class, IDBModelCore;

        #endregion


        #region Notifier
        void RegistModelNotifier<T, TId>(ModelEventNotifier<T, TId> modelEventNotifier)
            where T : class, BaseDBCore.IDBModelCore;

        #endregion
    }

    public abstract class DatabaseServiceCore : IDatabaseService, IDisposable
    {
        #region Initial
        public virtual Lock.ObjectLock dbLocker { get; }

        public DatabaseServiceCore()
        {
            modelNotifierControlsManager = new ModelNotifierControlsManager();
            eventNotifierHolders_dic = new Dictionary<Type, IEventNotifierHolder>();

            pre_initial();
        }

        private void pre_initial()
        {
            pre_initial_Notifier();

        }

        #endregion

        #region Add
        public async Task<AddModelOperationDBResult<TModel>> AddAsync<TModel>(TModel newModel) where TModel : class, IDBModelCore
        {
            var resultt = await _AddAsync(newModel);
            if (resultt.State == EntityState.Added)
            {
                _ModelNotify<TModel>(DBModelNotifierType.Insert, resultt.Entity);
            }
            else
            {
                throw new Exception();
            }
            return resultt;
        }

        protected abstract Task<AddModelOperationDBResult<TModel>> _AddAsync<TModel>(TModel newModel) where TModel : class, IDBModelCore;

        public OperationWatingResult<AddModelOperationDBResult<TModel>> AddWating<TModel>(TModel newModel) where TModel : class, IDBModelCore
        {
            return _AddWating(newModel);
        }

        protected virtual OperationWatingResult<AddModelOperationDBResult<TModel>> _AddWating<TModel>(TModel newModel) where TModel : class, IDBModelCore
        {
            return new OperationWatingResult<AddModelOperationDBResult<TModel>>(async () =>
            {
                return await _AddAsync(newModel);
            });
        }

        #endregion

        #region Delete
        public async Task<DeleteModelOperationDBResult<TModel>> DeleteAsync<TModel>(TModel model) where TModel : class, IDBModelCore
        {
            var resultt = await _DeleteAsync(model);
            if (resultt.State == EntityState.Added)
            {
                _ModelNotify<TModel>(DBModelNotifierType.Delete, resultt.Entity);
            }
            else
            {
                throw new Exception();
            }
            return resultt;
        }

        protected abstract Task<DeleteModelOperationDBResult<TModel>> _DeleteAsync<TModel>(TModel model) where TModel : class, IDBModelCore;


        #endregion

        #region All
        public TModel[] All<TModel>() where TModel : class, IDBModelCore
        {
            lock (this.dbLocker.opening_Lock)
            {
                return _All<TModel>();
            }
        }
        protected abstract TModel[] _All<TModel>() where TModel : class, IDBModelCore;


        #endregion

        #region Q Struct

        //
        public TModel Q_FirstOrDefault<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class, IDBModelCore
        {
            lock(this.dbLocker.opening_Lock)
            {
                return _Q_FirstOrDefault<TModel>(predicate);
            }
        }

        protected abstract TModel _Q_FirstOrDefault<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class, IDBModelCore;
        //
        public Task<TModel> Q_FirstOrDefaultAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class, IDBModelCore
        {
            lock (this.dbLocker.opening_Lock)
            {
                return _Q_FirstOrDefaultAsync<TModel>(predicate);
            }
        }

        protected abstract Task<TModel> _Q_FirstOrDefaultAsync<TModel>(Expression<Func<TModel, bool>> predicate) where TModel : class, IDBModelCore;

        #endregion



        #region Notifier

        private interface IEventNotifierHolder
        {
            Type ModelType { get; }
        }
        private interface IEventNotifierHolder<T> : IEventNotifierHolder
            where T : class, BaseDBCore.IDBModelCore
        {
            void ModelNotify(DBModelNotifierType notifierType, T model);
            void ModelNotify(DBModelNotifierType notifierType, T[] models);
        }
        private class EventNotifierHolder<T, TId> : IEventNotifierHolder<T>
            where T : class, BaseDBCore.IDBModelCore
        {
            public readonly TId defaultNull;
            public Type ModelType => typeof(T);
            public readonly ModelEventNotifier<T, TId> notifier;
            public EventNotifierHolder(TId defaultNull, ModelEventNotifier<T, TId> notifier, ModelNotifierControlsManager modelNotifierControlsManager)
            {
                this.defaultNull = defaultNull;
                this.notifier = notifier;
                this.modelNotifierControlsManager = modelNotifierControlsManager;
            }

            #region For Notify Events

            readonly ModelNotifierControlsManager modelNotifierControlsManager;


            public void ModelNotify(DBModelNotifierType notifierType, T model)
            {
                modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, model, defaultNull);
            }
            //public void ModelNotify(DBModelNotifierType notifierType, TId modelId)
            //{
            //    modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, modelId);
            //}
            //public void ModelNotify(DBModelNotifierType notifierType, TId modelId, T model = null)
            //{
            //    modelNotifierControlsManager.ModelNotify(notifierType, modelId, model);
            //}
            //public void ModelNotify(DBModelNotifierType notifierType, Dictionary<TId, T> dic)
            //{
            //    modelNotifierControlsManager.ModelNotify(notifierType, dic);
            //}
            //public void ModelNotify(DBModelNotifierType notifierType, TId[] modelIds)
            //{
            //    modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, modelIds);
            //}
            public void ModelNotify(DBModelNotifierType notifierType, T[] models)
            {
               modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, models, defaultNull);
            }

            #endregion
        }

        readonly internal ModelNotifierControlsManager modelNotifierControlsManager;
        readonly Dictionary<Type, IEventNotifierHolder> eventNotifierHolders_dic;
        private void pre_initial_Notifier()
        {

        }
       
        public ModelEventNotifier<T, TId> RegistModelNotifier<T, TId>(TId defaultNull)
            where T : class, BaseDBCore.IDBModelCore
        {
            var holder = new EventNotifierHolder<T, TId>(defaultNull, new ModelEventNotifier<T, TId>(), modelNotifierControlsManager);
            eventNotifierHolders_dic.Add(holder.ModelType, holder);
            modelNotifierControlsManager.AddNotifier<T, TId>(holder.notifier);
            return holder.notifier;
        }

        public void RegistModelNotifier<T, TId>(ModelEventNotifier<T, TId> modelEventNotifier)
            where T : class, BaseDBCore.IDBModelCore
        {
            modelNotifierControlsManager.AddNotifier<T, TId>(modelEventNotifier);
        }

        //Get
        #region Get Notifiers

        public ModelEventNotifier<T, TId> GetModelNotifier<T, TId>()
            where T : class, BaseDBCore.IDBModelCore
        {
            return eventNotifierHolders_dic[typeof(T)] as ModelEventNotifier<T, TId>;
        }
        private IEventNotifierHolder<T> GetModelNotifier<T>()
            where T : class, BaseDBCore.IDBModelCore
        {
            return eventNotifierHolders_dic[typeof(T)] as IEventNotifierHolder<T>;
        }

        #endregion


        #region For Notify Events

        public void ModelNotify<T>(DBModelNotifierType notifierType, T model)
            where T : class, BaseDBCore.IDBModelCore
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, model);
            }
        }
        protected void _ModelNotify<T>(DBModelNotifierType notifierType, T model)
            where T : class, BaseDBCore.IDBModelCore
        {
            GetModelNotifier<T>().ModelNotify(notifierType, model);
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId)
            where T : class, BaseDBCore.IDBModelCore
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify<T, TId>(notifierType, modelId);
            }
        }
        protected void _ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId)
            where T : class, BaseDBCore.IDBModelCore
        {
            modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, modelId);
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId, T model = null)
            where T : class, BaseDBCore.IDBModelCore
        {
            //lock(dbLocker.opening_Lock) // because it loacked by notifier manager
            {
                _ModelNotify<T, TId>(notifierType, modelId, model);
            }
        }
        protected void _ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId, T model = null)
            where T : class, BaseDBCore.IDBModelCore
        {
            modelNotifierControlsManager.ModelNotify(notifierType, modelId, model);
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, Dictionary<TId, T> dic)
            where T : class, BaseDBCore.IDBModelCore
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, dic);
            }
        }
        protected void _ModelNotify<T, TId>(DBModelNotifierType notifierType, Dictionary<TId, T> dic)
            where T : class, BaseDBCore.IDBModelCore
        {
            modelNotifierControlsManager.ModelNotify(notifierType, dic);
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, TId[] modelIds)
            where T : class, BaseDBCore.IDBModelCore
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify<T, TId>(notifierType, modelIds);
            }
        }
        protected void _ModelNotify<T, TId>(DBModelNotifierType notifierType, TId[] modelIds)
            where T : class, BaseDBCore.IDBModelCore
        {
            modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, modelIds);
        }

        public void ModelNotify<T>(DBModelNotifierType notifierType, T[] models)
            where T : class, BaseDBCore.IDBModelCore
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, models);
            }
        }
        protected void _ModelNotify<T>(DBModelNotifierType notifierType, T[] models)
            where T : class, BaseDBCore.IDBModelCore
        {
            GetModelNotifier<T>().ModelNotify(notifierType, models);
        }

        #endregion

        #endregion

        #region Dispose

        protected virtual void Dispose_OnFreeOtherManaged()
        {

        }
        protected virtual void Dispose_OnFreeUnManaged()
        {

        }

        private bool _disposed = false;
        public event EventHandler BeforeDispose;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (BeforeDispose != null)
                BeforeDispose?.Invoke(this, new EventArgs());

            if (disposing)
            {
                // Free any other managed objects here.
                Dispose_OnFreeOtherManaged();
            }

            // Free any unmanaged objects here.
            Dispose_OnFreeUnManaged();
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

       

        ~DatabaseServiceCore()
        {
            Dispose(false);
        }

        #endregion
    }
}
