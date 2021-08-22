using CommonAppCore.Locks;
using DBAppCore.EntityCore;
//using Syncfusion.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;


namespace DBAppCore.ListCtrls
{
    public abstract class ModelEntity_ListCtrlEntityDBManager<TEntity, TEntityArgs> where TEntity : ModelEntityCore where TEntityArgs : EntityArgsCore<TEntity>, IDisposable , System.ComponentModel.INotifyPropertyChanging
    {
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
            if (addListener)
                dBManagersControl.GetAppDBManagersControl<TEntity, TEntityArgs>().OnModelArgs_Add -= ModelDBManager_OnModel_Add;
            if (removeListener)
                dBManagersControl.GetAppDBManagersControl<TEntity, TEntityArgs>().OnModelArgs_Remove -= ModelDBManager_OnModel_Remove;
            if (updateListener)
                dBManagersControl.GetAppDBManagersControl<TEntity, TEntityArgs>().OnModelArgs_Update -= ModelDBManager_OnModel_Update;
            */

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
            //dBManagersControl = null;
            objectLock.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ModelEntity_ListCtrlEntityDBManager()
        {
            Dispose(false);
        }

        #endregion

        #region Singleton

        public readonly DBManagersControlCore dBManagersControl;

        bool addListener = true;
        bool removeListener = true;
        bool updateListener = true;
        public ModelEntity_ListCtrlEntityDBManager(
            DBManagersControlCore dBManagersCtrl,
            bool addListener = true,
            bool removeListener = true,
            bool updateListener = true
            )
        {
            this.dBManagersControl = dBManagersCtrl;
            this.addListener = addListener;
            this.removeListener = removeListener;
            this.updateListener = updateListener;

            refresh_pre();
        }

        // For properties
        #region Base Properties

        public ObservableCollection<TEntityArgs> models_coll;
        public Dictionary<string, TEntityArgs> models_dic;

        //public Dispatcher Dispatcher { get; set; }

        private void refresh_pre()
        {
            objectLock = new ObjectLock();
            //Dispatcher = Dispatcher.CurrentDispatcher;

            models_coll = new ObservableCollection<TEntityArgs>();
            models_dic = new Dictionary<string, TEntityArgs>();

            refresh_pre_Adding();
            refresh_pre_DBEvents();
        }
        #endregion

        // For Lock
        #region Lock Base Properties
        private ObjectLock objectLock;

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

        #region For Add 
        private void refresh_pre_Adding()
        {

        }

        // events
        public event EventHandler<TEntityArgs> AddModelArgs;
        protected virtual void OnAddModelArgs(TEntityArgs modelArgs)
        {
            if (AddModelArgs != null)
            {
                AddModelArgs(this, modelArgs);
            }

            //try
            //{
            //    if (!models_coll.Contains(modelArgs))
            //    {
            //        //models_coll.Add(modelArgs);
            //        if (Dispatcher.CheckAccess())
            //        {
            //            models_coll.Add(modelArgs);
            //        }
            //        else
            //        {
            //            models_coll.Add(modelArgs);

            //            /*
            //            Dispatcher.Invoke(new Action(() =>
            //            {
            //                models_coll.Add(modelArgs);
            //            }));
            //        */
            //        }
            //    }
            //}
            //catch
            //{
            //    throw new Exception();
            //}
            CountModels_Change();
        }

        //adding
        private void _Add_Model(TEntity model)
        {
            _Add_Model(dBManagersControl.GetEntityDBManager<TEntity, TEntityArgs>().To_ModelArgs(model) as TEntityArgs);
        }
        private void _Add_Model(TEntityArgs modelArgs)
        {
            if (_isExist_Model(modelArgs.ObjectId))
            {
                // for update with new Model
                _Update_Model(modelArgs);
            }
            else
            {
                models_dic.Add(modelArgs.ObjectId, modelArgs);
                OnAddModelArgs(modelArgs);
            }
        }
        public void Add_Model(TEntityArgs modelArgs)
        {
            open_Lock();
            _Add_Model(modelArgs);
            close_Lock();
        }
        public void Add_Model(TEntity model)
        {
            open_Lock();
            _Add_Model(model);
            close_Lock();
        }


        #endregion

        #region For Remove
        public event EventHandler<TEntityArgs> RemoveModelArgs;
        protected virtual void OnRemoveModelArgs(TEntityArgs modelArgs)
        {
            if (models_coll.Contains(modelArgs))
            {
                models_coll.Remove(modelArgs);
            }

            if (RemoveModelArgs != null)
            {
                RemoveModelArgs(this, modelArgs);
            }

            CountModels_Change();
        }
        // remove
        private void _Remove_Model(TEntityArgs modelArgs)
        {
            if (!_isExist_Model(modelArgs.ObjectId))
            {
                // for update with new Model
            }
            else
            {
                models_dic.Remove(modelArgs.ObjectId);
                OnRemoveModelArgs(modelArgs);
            }
        }
        private void _Remove_Model(string model_ObjectId)
        {
            if (!_isExist_Model(model_ObjectId))
            {
                // for update with new Model
            }
            else
            {
                var temp_arg = models_dic[model_ObjectId];
                models_dic.Remove(model_ObjectId);
                OnRemoveModelArgs(temp_arg);
            }
        }
        public void Remove_Model(string model_ObjectId)
        {
            open_Lock();
            _Remove_Model(model_ObjectId);
            close_Lock();
        }
        public void Remove_Model(TEntityArgs modelArgs)
        {
            open_Lock();
            _Remove_Model(modelArgs);
            close_Lock();
        }

        #endregion

        #region For Update

        public event EventHandler<TEntityArgs> UpdateModelArgs;
        protected virtual void OnUpdateModelArgs(TEntityArgs modelArgs)
        {
            if (UpdateModelArgs != null)
            {
                UpdateModelArgs(this, modelArgs);
            }
            /*
            if (!models_coll.Contains(modelArgs))
            {
                models_coll.Add(modelArgs);
            }
            */
           // CountModels_Change();
        }
        private void _Update_Model(TEntity model)
        {
            if (!models_dic.Keys.Contains(model.ObjectId))
            {
                _Add_Model(model);
                //throw new Exception();
            }
            else
            {
                models_dic[model.ObjectId].model_entity = model;
                //models_dic[model.ObjectId].Refresh_ObjModel();
                OnUpdateModelArgs(models_dic[model.ObjectId]);
            }
        }
        public void Update_Model(TEntity model)
        {
            open_Lock();
            _Update_Model(model);
            close_Lock();
        }
        private void _Update_Model(TEntityArgs modelArgs)
        {
            _Update_Model(modelArgs.model_entity);
        }
        public void Update_Model(TEntityArgs modelArgs)
        {
            open_Lock();
            _Update_Model(modelArgs.model_entity);
            close_Lock();
        }

        #endregion

        #region For Checking
        // base Exisiting
        private bool _isExist_Model(string objectId)
        {
            return models_dic.Keys.Contains(objectId);
        }
        public bool isExist_Model(string model_ObjectId)
        {
            bool resultt = false;
            open_Lock();
            resultt = _isExist_Model(model_ObjectId);
            close_Lock();
            return resultt;
        }
        // get
        private TEntityArgs _get_ModelArgs(string objectId)
        {
            if (_isExist_Model(objectId))
            {
                return models_dic[objectId];
            }
            return null;
        }
        public TEntityArgs get_ModelArgs(string objectId)
        {
            open_Lock();
            TEntityArgs temp_args = _get_ModelArgs(objectId);
            close_Lock();

            return temp_args;
        }
        public List<TEntityArgs> _All_Items()
        {
            return models_coll.ToList();
        }
        public List<TEntityArgs> All_Items()
        {
            open_Lock();
            var temp_args = _All_Items();
            close_Lock();

            return temp_args;
        }

        #endregion

        #region For Count List

        //public delegate void OnCountModels_ChangeDelegate(int count_models);
        //public event OnCountModels_ChangeDelegate OnCountModels_Change;
        protected virtual void CountModels_Change()
        {
            OnPropertyChanged(nameof(Model_Counts));
            OnPropertyChanged(nameof(Rows_Count));
        }
        public int Model_Counts
        {
            get
            {
                return models_dic.Count;
            }
        }
        public int Rows_Count
        {
            get
            {
                return models_coll.Count;
            }
        }

        #endregion

        #region For DBEvents
        private void refresh_pre_DBEvents()
        {
            /*
            if (addListener)
                dBManagersControl.GetAppDBManagersControl<TEntity, TEntityArgs>().OnModelArgs_Add += ModelDBManager_OnModel_Add;
            if (removeListener)
                dBManagersControl.GetAppDBManagersControl<TEntity, TEntityArgs>().OnModelArgs_Remove += ModelDBManager_OnModel_Remove;
            if (updateListener)
                dBManagersControl.GetAppDBManagersControl<TEntity, TEntityArgs>().OnModelArgs_Update += ModelDBManager_OnModel_Update;
            */
        }

        private void ModelDBManager_OnModel_Update(EntityArgsCore<TEntity> modelArgs)
        {
            Update_Model(modelArgs as TEntityArgs);
        }
         void ModelDBManager_OnModel_Remove(EntityArgsCore<TEntity> modelArgs)
        {
            Remove_Model(modelArgs as TEntityArgs);
        }
        private void ModelDBManager_OnModel_Add(EntityArgsCore<TEntity> modelArgs)
        {
            Add_Model(modelArgs as TEntityArgs);
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

        // like : on double click
        public event EventHandler<TEntityArgs> SelectModelArgs;
        protected virtual void OnSelectModelArgs(TEntityArgs modelArgs)
        {
            if (SelectModelArgs != null)
            {
                SelectModelArgs(this, modelArgs);
            }
        }
        public void OnModelArgs_Selected(TEntityArgs modelArgs)
        {
            //open_Lock();
            OnSelectModelArgs(modelArgs);
            //close_Lock();
        }

        // on get selected model
        public delegate List<TEntityArgs> OnGetSelectedModel_Delegate();
        public event OnGetSelectedModel_Delegate OnGetSelectedModel;
        protected virtual List<TEntityArgs> GetSelectedModel()
        {
            if (OnGetSelectedModel != null)
            {
                return OnGetSelectedModel();
            }
            return null;
        }

        public List<TEntityArgs> GetSelectedModels()
        {
            return GetSelectedModel();
        }


        #endregion

        #region fiers

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
