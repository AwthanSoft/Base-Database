using Mawa.BaseApp;
using Mawa.Lock;
//using Syncfusion.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Mawa.BaseDBCore.ViewEntityCore;

namespace Mawa.DBCore.ViewEntityListCtrls
{

    public abstract class ViewEntity_ListCtrl<TViewEntity> where TViewEntity : ModelViewEntityCore ,IDisposable , INotifyPropertyChanged
    {
        #region Initilal

        public ViewEntity_ListCtrl()
        {
            objectLock = new ObjectLock();

            pre_refresh_Coll();
            pre_refresh_Counters();




            pre_refresh();



        }
        private void pre_refresh()
        {



        }

        #region Lock Base Properties
        readonly ObjectLock objectLock;

        public void open_Lock()
        {
            objectLock.open_lock();
        }
        public void close_Lock()
        {
            objectLock.close_lock();
        }

        #endregion

        #endregion

        #region Coll
        public ObservableCollection<TViewEntity> viewEntities_coll { private set; get; }
        public Dictionary<string, TViewEntity> viewEntities_dic { private set; get; }

        void pre_refresh_Coll()
        {
            //_TViewEntitys_coll == new ObservableCollection<TViewEntity>();
            viewEntities_coll.CollectionChanged += ViewModels_coll_CollectionChanged;

            viewEntities_dic = new Dictionary<string, TViewEntity>();
        }

        private void ViewModels_coll_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _Add_To_Observable(string key, TViewEntity TViewEntity)
        {
            //Device.InvokeOnMainThreadAsync(() =>
            MainAppThread.Invoke(() =>
            {
                viewEntities_dic.Add(key, TViewEntity);
                viewEntities_coll.Add(TViewEntity);
                Coll_ViewEntities_Change();
            });
        }
        void _Remove_To_Observable(string key, TViewEntity TViewEntity)
        {
            //Device.BeginInvokeOnMainThread(() =>
            MainAppThread.Invoke(() =>
            {
                viewEntities_dic.Remove(key);
                if (viewEntities_coll.Contains(TViewEntity))
                {
                    viewEntities_coll.Remove(TViewEntity);
                }
                Coll_ViewEntities_Change();
            });
        }
        void _Update_To_Observable(string key, TViewEntity TViewEntity)
        {
            throw new Exception();
            //Device.BeginInvokeOnMainThread(() =>
            //{
            //    _models_dic.Remove(key);
            //    if (models_coll.Contains(modelArg))
            //    {
            //        models_coll.Remove(modelArg);
            //    }
            //    CountModels_Change();
            //});
        }

        void Coll_ViewEntities_Change()
        {

        }

        #endregion

   
        #region For Add 

        // events
        public event EventHandler<TViewEntity> AddViewEntity;
        protected virtual void OnAddViewEntity(TViewEntity viewEntity)
        {
            if (AddViewEntity != null)
            {
                AddViewEntity?.Invoke(this, viewEntity);
            }
        }

        //adding
        private void _Add_ViewEntity(TViewEntity viewEntity)
        {
            if (_isExist_ViewEntity(viewEntity.ObjectId))
            {
                // for update with new Model
                _Update_ViewEntity(viewEntity);
            }
            else
            {
                _Add_To_Observable(viewEntity.ObjectId, viewEntity);
                OnAddViewEntity(viewEntity);
            }
        }

        public void Add_TViewEntity(TViewEntity TViewEntity)
        {
            open_Lock();
            _Add_ViewEntity(TViewEntity);
            close_Lock();
        }

        #endregion

        #region For Remove

        public event EventHandler<TViewEntity> RemoveViewEntity;
        protected virtual void OnRemoveViewEntity(TViewEntity viewEntity)
        {
            if (RemoveViewEntity != null)
            {
                RemoveViewEntity?.Invoke(this, viewEntity);
            }
        }
        // remove
        private void _Remove_ViewEntity(TViewEntity viewEntity)
        {
            if (!_isExist_ViewEntity(viewEntity.ObjectId))
            {
                //as temp
                throw new Exception();
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                _Remove_To_Observable(viewEntity.ObjectId, viewEntity);
                OnRemoveViewEntity(viewEntity);
            }
        }
        private void _Remove_ViewEntity(string viewEntity_ObjectId)
        {
            if (!_isExist_ViewEntity(viewEntity_ObjectId))
            {
                //as temp
                throw new Exception();
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                var temp_arg = viewEntities_dic[viewEntity_ObjectId];
                _Remove_To_Observable(viewEntity_ObjectId, temp_arg);
                OnRemoveViewEntity(temp_arg);
            }
        }
        public void Remove_TViewEntity(TViewEntity TViewEntity)
        {
            open_Lock();
            _Remove_ViewEntity(TViewEntity);
            close_Lock();
        }

        #endregion

        #region For Update

        public event EventHandler<TViewEntity> UpdateViewEntity;
        protected virtual void OnUpdateViewEntity(TViewEntity viewEntity)
        {
            if (UpdateViewEntity != null)
            {
                UpdateViewEntity?.Invoke(this, viewEntity);
            }
            //CountModels_Change();
        }
        private void _Update_ViewEntity(TViewEntity viewEntity)
        {
            //as temp
            throw new Exception();
            /*
            if (!TViewEntitys_dic.Keys.Contains(TViewEntity.ObjectId))
            {
                //as temp
                throw new Exception();
                //pass
                // may it is not in db
                //_Add_Model(model);
                //throw new Exception();
            }
            else
            {
                TViewEntitys_dic[TViewEntity.ObjectId].TViewEntity.obj_model = TViewEntity;
                //models_dic[model.ObjectId].Refresh_ObjModel();
                OnUpdateTViewEntity(_models_dic[TViewEntity.ObjectId]);
            }
            */
        }
        public void Update_ViewEntity(TViewEntity viewEntity)
        {
            open_Lock();
            _Update_ViewEntity(viewEntity);
            close_Lock();
        }

        #endregion


        #region For Checking
        // base Exisiting
        private bool _isExist_ViewEntity(string objectId)
        {
            return viewEntities_dic.Keys.Contains(objectId);
        }
        public bool isExist_ViewEntity(string viewEntity_ObjectId)
        {
            bool resultt = false;
            open_Lock();
            resultt = _isExist_ViewEntity(viewEntity_ObjectId);
            close_Lock();
            return resultt;
        }


        #endregion


        #region Counters
        public int viewEntitysCounter => viewEntities_dic.Count;
        public int RowsCounter => viewEntities_coll.Count;
        void pre_refresh_Counters()
        {

        }

        #endregion

        #region For Searching

        //public bool isSearchingMod = false;
        //private void exit_SearchingMod()
        //{
        //    if (isSearchingMod)
        //    {

        //    }
        //    else
        //    {
        //        // refresh list
        //    }
        //    isSearchingMod = false;
        //}
        //public void Exit_SearchingMod()
        //{
        //    open_Lock();
        //    exit_SearchingMod();
        //    close_Lock();
        //}

        //private void _Search_Models_by_fullName(string type_name)
        //{
        //    isSearchingMod = true;
        //    type_name = type_name.ToLower();
        //    //var args = models_dic.Values;
        //    var objs = from arg in models_dic.Values
        //               where arg.ResourceTitle.ToLower().Contains(type_name)
        //               select arg;

        //    models_coll.Clear();
        //    foreach (var obj in objs.ToList())
        //    {
        //        models_coll.Add(obj);
        //    }

        //    //refresh on the refresh
        //}
        //public void Search_Models_by_fullName(string type_name)
        //{
        //    open_Lock();
        //    _Search_Models_by_fullName(type_name);
        //    close_Lock();
        //}


        #endregion

        #region Refresh List

        //public delegate void OnRefreshModelsListDelegate();
        //public event OnRefreshModelsListDelegate OnRefreshModelsList;
        //protected virtual void RefreshModelsList()
        //{
        //    if (OnRefreshModelsList != null)
        //    {
        //        OnRefreshModelsList();
        //    }
        //}
        //public void refresh_list()
        //{
        //    open_Lock();
        //    RefreshModelsList();
        //    close_Lock();
        //}



        //public void Refresh_allItem_db()
        //{
        //    open_Lock();
        //    _Refresh_allItem_db();
        //    close_Lock();
        //}
        //private void _Refresh_allItem_db()
        //{
        //    var keys_list = new List<string>();
        //    foreach (var arg in models_dic.Values)
        //    {
        //        keys_list.Add(arg.Resource_Id);
        //    }

        //    foreach (var key in keys_list)
        //    {
        //        var arg = dBManagersControl.resourcesTextAppDBManagersControl.tempResourcesTextEntityDBManager.get_ModelArg_By_Id(key);
        //        if (arg != null)
        //        {
        //            Add_model(arg);
        //        }
        //        else
        //        {
        //            throw new Exception();
        //        }
        //    }
        //}

        #endregion

        #region For Select
        /*
        // like : on double click
        public event EventHandler<TViewEntity> SelectViewEntity;
        protected virtual void OnSelectViewEntity(TViewEntity viewEntity)
        {
            if (SelectViewEntity != null)
            {
                SelectViewEntity?.Invoke(this, viewEntity);
            }
        }
        public void OnModelArgs_Selected(TViewEntity modelArgs)
        {
            //open_Lock();
            OnSelectViewEntity(modelArgs);
            //close_Lock();
        }

        // on get selected model
        public delegate List<TViewEntity> OnGetSelectedModel_Delegate();
        public event OnGetSelectedModel_Delegate OnGetSelectedModel;
        protected virtual List<TViewEntity> GetSelectedModel()
        {
            if (OnGetSelectedModel != null)
            {
                return OnGetSelectedModel();
            }
            return null;
        }

        public List<TViewEntity> GetSelectedModels()
        {
            return GetSelectedModel();
        }
        */

        #endregion



        #region INotify fire

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Dispose
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.

           /*
            OnGetSelectedModel = null;
            SelectModelArgs = null;

            AddModelArgs = null;
            RemoveModelArgs = null;
            UpdateModelArgs = null;
            //OnCountModels_Change = null;

            models_dic.Clear();
            models_dic = null;

            models_coll.Clear();
            models_coll = null;
            //dBManagersControl = null;*/
            objectLock.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ViewEntity_ListCtrl()
        {
            Dispose(false);
        }

        #endregion
    }
}
