using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Mawa.DBCore.ViewEntityCore;

using Mawa.DBCore.NotifierCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mawa.Lock;
using Mawa.BaseDBCore.EntityCore;
using Mawa.BaseDBCore.ViewEntityCore;

namespace Mawa.DBCore
{
    public abstract class DBManagersControlCore
    {
        #region Singleton

        readonly internal ModelNotifierControlsManager modelNotifierControlsManager;


        readonly DB_OperCore _dB_oper;
        protected DB_OperCore dB_oper => _dB_oper;

        public DbContext GetNew_dbContext() => dB_oper.GetNew_dbContext();

        internal DbContext db => dB_oper.db;
        public ObjectLock dbLocker => dB_oper.dbLocker;

        public DBManagersControlCore(DB_OperCore dB_oper)
        {
            this._dB_oper = dB_oper;
            modelNotifierControlsManager = new ModelNotifierControlsManager();
        }

        protected void startInitial()
        {
            initial_AppDBManagersControls();
            pre_Refresh_EntityTypes();
            pre_Refresh_ViewTypes();

            pre_Refresh_Views_Listensers();
        }


        #region Lock
        //public CommonAppCore.Locks.ObjectLock DBLock => _dB_oper.DBLock;
        public object DBOpeningLock => _dB_oper.DBOpeningLock;
        public void open_Lock()
        {
            _dB_oper.open_Lock();
        }

        public void close_Lock()
        {
            _dB_oper.close_Lock();
        }

        #endregion

        #endregion

        #region Abstract Methods

        protected abstract void initial_AppDBManagersControls();
        protected abstract List<AppDBManagersControlCore> getList_AppDBManagersControls();

        #endregion

        #region For Entities types

        private Dictionary<Type, object> _EntityManager_types_dic;
        private void pre_Refresh_EntityTypes()
        {
            var apps_list = getList_AppDBManagersControls();
            _EntityManager_types_dic = new Dictionary<Type, object>();
            foreach (var app in apps_list)
            {
                foreach (var kk in app.EntityManager_types_list)
                {
                    _EntityManager_types_dic.Add(kk.Key, kk.Value);
                }
            }
        }


        #endregion

        #region For Views types

        private Dictionary<Type, object> _ViewEntityManager_types_dic;
        private void pre_Refresh_ViewTypes()
        {
            var apps_list = getList_AppDBManagersControls();
            _ViewEntityManager_types_dic = new Dictionary<Type, object>();
            foreach (var app in apps_list)
            {
                foreach (var kk in app.ViewEntityManager_types_list)
                {
                    _ViewEntityManager_types_dic.Add(kk.Key, kk.Value);
                }
            }
        }
        //internal ViewEntityDBManagerCore<TViewEntity> GetViewEntityDBManager<TViewEntity>()
        //    where TViewEntity : IModelViewEntityCore
        //{
        //    return _ViewEntityManager_types_dic[typeof(TViewEntity)] as ViewEntityDBManagerCore<TViewEntity>;
        //}

        #endregion

        #region ViewsListeners
        private void pre_Refresh_Views_Listensers()
        {
            foreach(var dbManager in _ViewEntityManager_types_dic.Values)
            {
                (dbManager as IViewEntityDBManagerCore).Refresh_Listeners();
            }
        }

        #endregion

        #region Safe Executions

        public void ExecuteInsideTrans(Action<IDbContextTransaction> action, out Exception exception_result)
        {
            exception_result = null;
            this.open_Lock();
            using (var dbTrns = db.Database.BeginTransaction())
            {
                try
                {
                    action.Invoke(dbTrns);
                    //dbTrns.Commit();
                }
                catch (Exception ex)
                {
                    exception_result = ex;
                    dbTrns.Rollback();
                }
            }
            this.close_Lock();
        }

        //public void ExecuteInsideTrans<TArgs>(Action<IDbContextTransaction, TArgs> action, TArgs args, out Exception exception_result)
        //{
        //    exception_result = null;
        //    this.open_Lock();
        //    using (var dbTrns = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            action.Invoke(args);
        //            dbTrns.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            exception_result = ex;
        //            dbTrns.Rollback();
        //        }
        //    }
        //    this.close_Lock();
        //}


        public void ExecuteInsideTrans<TResult>(Func<TResult> predicate, out TResult Result, out Exception exception_result)
            where TResult : class
        {
            exception_result = null;
            Result = null;
            this.open_Lock();
            using (var dbTrns = db.Database.BeginTransaction())
            {
                try
                {
                    Result = predicate.Invoke();
                    if (Result != null)
                    {
                        dbTrns.Commit();
                    }
                    else
                    {
                        dbTrns.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    exception_result = ex;
                    dbTrns.Rollback();
                }
            }
            this.close_Lock();
        }

        public void ExecuteInsideTrans<TResult>(Func<IDbContextTransaction, TResult> predicate, out TResult Result, out Exception exception_result)
           where TResult : class
        {
            exception_result = null;
            Result = null;
            this.open_Lock();
            using (var dbTrns = db.Database.BeginTransaction())
            {
                try
                {
                    Result = predicate.Invoke(dbTrns);
                }
                catch (Exception ex)
                {
                    exception_result = ex;
                    dbTrns.Rollback();
                }
            }
            this.close_Lock();
        }

        #endregion

    }
}
