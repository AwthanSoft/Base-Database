//using Syncfusion.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace DBAppCore
{
    public abstract class AppDBManagersControlCore
    {
        #region Singleton

        protected DBManagersControlCore dBManagerCore;
        //AppDbContext db { get { return dBManager.db; } }

        public AppDBManagersControlCore(DBManagersControlCore dBManager)
        {
            this.dBManagerCore = dBManager;
            EntityManager_types_dic = new Dictionary<Type, object>();
            ViewEntityManager_types_dic = new Dictionary<Type, object>();
            
            Initial_DBManagers();
        }
        #endregion

        #region Initial DBMangers
        void Initial_DBManagers()
        {
            Initial_ModelDBControllers();

            Initial_EntityDBManagers();
            Initial_EntityCtrlsCore();
            Initial_ViewEntityDBManagers();

            add_EntityManagers_Types();
            add_ViewEntityManagers_Types();
        }

        protected abstract void Initial_ModelDBControllers();

        protected abstract void Initial_EntityDBManagers();
        protected abstract void Initial_ViewEntityDBManagers();
        protected abstract void Initial_EntityCtrlsCore();

        #endregion

        #region For Openning

        protected void open_lock()
        {
            dBManagerCore.open_Lock();
        }
        protected void close_lock()
        {
            dBManagerCore.close_Lock();
        }

        #endregion


        #region For EntityDBControlls

        private Dictionary<Type, object> EntityManager_types_dic;
        internal KeyValuePair<Type, object>[] EntityManager_types_list => EntityManager_types_dic.ToArray();

        protected void add_EntityManager_Type(Type T , object entityDBManager)
        {
            EntityManager_types_dic.Add(T, entityDBManager);
        }
        
        protected abstract void add_EntityManagers_Types();

        #endregion

        #region For ViewEntityDBControlls

        private Dictionary<Type, object> ViewEntityManager_types_dic;
        internal KeyValuePair<Type, object>[] ViewEntityManager_types_list => ViewEntityManager_types_dic.ToArray();

        protected void add_ViewEntityManager_Type(Type T, object viewEntityDBManager)
        {
            ViewEntityManager_types_dic.Add(T, viewEntityDBManager);
        }

        protected abstract void add_ViewEntityManagers_Types();

        #endregion

    }
}
