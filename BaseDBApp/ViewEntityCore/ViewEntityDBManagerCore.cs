using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

//using System.Data.Entity.Migrations;
using DBAppCore.NotifierCore.ViewEntity;

namespace DBAppCore.ViewEntityCore
{
    internal interface IViewEntityDBManagerCore
    {
        void Refresh_Listeners();
    }
    public abstract class ViewEntityDBManagerCore<T> : ModelDBController<T> , IViewEntityDBManagerCore
        where T: ModelViewEntityCore 
    {
        #region Singleton


        public ViewEntityDBManagerCore(DBManagersControlCore dBManagerCore) : base(dBManagerCore)
        {
            pre_refresh();
        }
        private void pre_refresh()
        {
        }

        #endregion

        #region Listeners
        public void Refresh_Listeners()
        {
            OnRefresh_Listeners();
        }
        protected abstract void OnRefresh_Listeners();
        protected void Notify_View_Add(T model)
        {
            dBManagerCore.modelNotifierControlsManager.InsertNotiFy<T>(model);
        }
        protected void Notify_View_Update(T model)
        {
            dBManagerCore.modelNotifierControlsManager.UpdateNotiFy<T>(model);
        }
        protected void Notify_View_Delete(T model)
        {
            throw new Exception();
        }
        protected void Notify_View_Refresh()
        {
            dBManagerCore.modelNotifierControlsManager.RefreshNotiFy<T>();
        }

        #endregion


        #region For Methods get

        public T Get_Model_By_ObjectId(string ObjectId)
        {
            open_lock();
            var resultt = _Get_Model_By_ObjectId(ObjectId);
            close_lock();
            return resultt;
        }
        private T _Get_Model_By_ObjectId(string ObjectId)
        {
            //var temp_model = db_ModelEntity.Find(ObjectId);
            //var temp_model = db_ModelEntity.Select(m => m.ObjectId.Equals(ObjectId)).Distinct();
            var temp_model = db_Model.Where(m => m.ObjectId.Equals(ObjectId)).FirstOrDefault();
            return temp_model;
        }
        #endregion

        #region For Searching Methods 



        #endregion

    }




}
