using System;
using System.Linq;
using Mawa.BaseDBCore.ViewEntityCore;

//using System.Data.Entity.Migrations;

namespace Mawa.DBCore.ViewEntityCore
{
    internal interface IViewEntityDBManagerCore
    {
        void Refresh_Listeners();
    }
    public abstract class ViewEntityDBManagerCore<T, TId> : ModelDBController<T, TId> , IViewEntityDBManagerCore
        where T: class, IModelViewEntityCore
    {
        #region Singleton

        public ViewEntityDBManagerCore(DBManagersControlCore dBManagerCore, TId defualtNull) : base(dBManagerCore, defualtNull)
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


        //protected void Notify_View_Add(T model)
        //{
        //    dBManagerCore.modelNotifierControlsManager.InsertNotiFy<T, TId>(model);
        //}
        //protected void Notify_View_Update(T model)
        //{
        //    dBManagerCore.modelNotifierControlsManager.UpdateNotiFy<T>(model);
        //}
        //protected void Notify_View_Delete(T model)
        //{
        //    throw new Exception();
        //}
        //protected void Notify_View_Refresh()
        //{
        //    dBManagerCore.modelNotifierControlsManager.RefreshNotiFy<T>();
        //}

        #endregion



    }




}
