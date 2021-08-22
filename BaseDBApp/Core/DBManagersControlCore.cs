﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DBAppCore.ViewEntityCore;
using DBAppCore.EntityCore;

using DBAppCore.NotifierCore;

namespace DBAppCore
{
    public abstract class DBManagersControlCore
    {
        #region Singleton

        readonly internal ModelNotifierControlsManager modelNotifierControlsManager;
        //readonly internal EntityNotifierControlsManager entityNotifierControlsManager;
        //readonly internal ViewEntityNotifierControlsManager viewEntityNotifierControlsManager;
        

        readonly DB_OperCore _dB_oper;
        protected DB_OperCore dB_oper => _dB_oper;

        internal DbContext db => dB_oper.db;
        internal DbContextCore dbContextCore => dB_oper.dbContextCore;

        public DBManagersControlCore(DB_OperCore dB_oper)
        {
            this._dB_oper = dB_oper;
            modelNotifierControlsManager = new ModelNotifierControlsManager(this);
            //entityNotifierControlsManager = new EntityNotifierControlsManager(this);
            //viewEntityNotifierControlsManager = new ViewEntityNotifierControlsManager(this);
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

        //public EntityDBManager<TEntity, ModelEntityArgs<TEntity>> GetAppDBManagersControl<TEntity>()
        //    where TEntity : ModelEntity
        //{
        //    var resultt = _EntityManager_types_dic[typeof(TEntity)];
        //    return resultt as EntityDBManager<TEntity, ModelEntityArgs<TEntity>>;
        //}

        internal EntityDBManagerCore<TEntity, TEntityArgs> GetEntityDBManager<TEntity, TEntityArgs>()
            where TEntity : ModelEntityCore
            where TEntityArgs : EntityArgsCore<TEntity>
        {
            return _EntityManager_types_dic[typeof(TEntity)] as EntityDBManagerCore<TEntity, TEntityArgs>;
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
        internal ViewEntityDBManagerCore<TViewEntity> GetViewEntityDBManager<TViewEntity>()
            where TViewEntity : ViewEntityCore.ModelViewEntityCore
        {
            return _ViewEntityManager_types_dic[typeof(TViewEntity)] as ViewEntityDBManagerCore<TViewEntity>;
        }

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

    }
}