using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using DBAppCore.EntityCore;
using CommonAppCore.Locks;
using CommonAppCore.BaseApp;

namespace DBAppCore.ListCtrls
{
    
    public abstract class EntityViewModel_ListCtrl<TEntity, TEntityArgs , TEntityViewModel> : IDisposable, INotifyPropertyChanged
        where TEntity : ModelEntityCore 
        where TEntityArgs : EntityArgsCore<TEntity>
        where TEntityViewModel : EntityViewModelCore<TEntity , TEntityArgs>
     
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
                dBManagersControlCore.GetAppDBManagersControl<TEntity,TEntityArgs>().OnModelArgs_Add -= ModelDBManager_OnModel_Add;
            if (removeListener)
                dBManagersControlCore.GetAppDBManagersControl<TEntity,TEntityArgs>().OnModelArgs_Remove -= ModelDBManager_OnModel_Remove;
            if (updateListener)
                dBManagersControlCore.GetAppDBManagersControl<TEntity,TEntityArgs>().OnModelArgs_Update -= ModelDBManager_OnModel_Update;
            */

            //OnGetSelectedModel = null;
            //SelectModelArgs = null;

            AddModelArgs = null;
            RemoveModelArgs = null;
            UpdateModelArgs = null;
            //OnCountModels_Change = null;

            _models_dic.Clear();
            _models_dic = null;

            _models_coll.Clear();
            _models_coll = null;
            //dBManagersControl = null;
            objectLock.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EntityViewModel_ListCtrl()
        {
            Dispose(false);
        }

        #endregion

        #region Initial

        public readonly DBManagersControlCore dBManagersControlCore;

        bool addListener = true;
        bool removeListener = true;
        bool updateListener = true;
        public EntityViewModel_ListCtrl(
            DBManagersControlCore dBManagersCtrl,
            bool addListener = true,
            bool removeListener = true,
            bool updateListener = true
            )
        {
            this.dBManagersControlCore = dBManagersCtrl;
            this.addListener = addListener;
            this.removeListener = removeListener;
            this.updateListener = updateListener;

            refresh_pre();
        }



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

        // For properties
        #region Base Properties

        ObservableCollection<TEntityViewModel> _models_coll;
        public ObservableCollection<TEntityViewModel> models_coll => _models_coll;

        //ObservableDictionary<string, TEntityViewModel> _models_dic;
        //public ObservableDictionary<string, TEntityViewModel> models_dic => _models_dic;

        Dictionary<string, TEntityViewModel> _models_dic;
        public Dictionary<string, TEntityViewModel> models_dic => _models_dic;
        //public Dictionary<string, TEntityViewModel> models_dic => _models_dic;

        //public Dispatcher Dispatcher { get; set; }

        private void refresh_pre()
        {
            objectLock = new ObjectLock();
            //Dispatcher = Dispatcher.CurrentDispatcher;

            _models_coll = new ObservableCollection<TEntityViewModel>();
            _models_dic = new Dictionary<string, TEntityViewModel>();
            //_models_dic = new ObservableDictionary<string, TEntityViewModel>();

            refresh_pre_Adding();
            refresh_pre_DBEvents();
            pre_refresh_Searching();
        }
        #endregion

        #region Coll

        void _Add_To_Observable(string key , TEntityViewModel modelArg)
        {
            //Device.InvokeOnMainThreadAsync(() =>
            MainAppThread.Invoke(()=>
            {
                _models_dic.Add(key, modelArg);
                models_coll.Add(modelArg);
                CountModels_Change();
            });
        }
        void _Remove_To_Observable(string key, TEntityViewModel modelArg)
        {
            //Device.BeginInvokeOnMainThread(() =>
            MainAppThread.Invoke(() =>
            {
                _models_dic.Remove(key);
                if(models_coll.Contains(modelArg))
                {
                    models_coll.Remove(modelArg);
                }
                CountModels_Change();
            });
        }
        void _Update_To_Observable(string key, TEntityViewModel modelArg)
        {
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

        #endregion

        #region For Add 
        private void refresh_pre_Adding()
        {

        }

        // events
        public event EventHandler<TEntityViewModel> AddModelArgs;
        protected virtual void OnAddModelArgs(TEntityViewModel modelArgs)
        {
            if (AddModelArgs != null)
            {
                AddModelArgs(this, modelArgs);
            }
        }

        //adding
        private void _Add_Model(TEntity model)
        {
            _Add_Model(dBManagersControlCore.GetEntityDBManager<TEntity,TEntityArgs>().To_ModelArgs(model) as TEntityArgs);
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
                TEntityViewModel modelView = To_ViewModel(modelArgs);
                _Add_To_Observable(modelArgs.ObjectId, modelView);
                OnAddModelArgs(modelView);
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

        public event EventHandler<TEntityViewModel> RemoveModelArgs;
        protected virtual void OnRemoveModelArgs(TEntityViewModel modelArgs)
        {
            if (RemoveModelArgs != null)
            {
                RemoveModelArgs(this, modelArgs);
            }
        }
        // remove
        private void _Remove_Model(TEntityArgs modelArgs)
        {
            if (!_isExist_Model(modelArgs.ObjectId))
            {
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                TEntityViewModel modelView = _models_dic[modelArgs.ObjectId];
                _Remove_To_Observable(modelArgs.ObjectId , modelView);
                OnRemoveModelArgs(modelView);
            }
        }
        private void _Remove_Model(string model_ObjectId)
        {
            if (!_isExist_Model(model_ObjectId))
            {
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                var temp_arg = _models_dic[model_ObjectId];
                _Remove_To_Observable(model_ObjectId, temp_arg);
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

        public event EventHandler<TEntityViewModel> UpdateModelArgs;
        protected virtual void OnUpdateModelArgs(TEntityViewModel modelArgs)
        {
            if (UpdateModelArgs != null)
            {
                UpdateModelArgs(this, modelArgs);
            }
            //CountModels_Change();
        }
        private void _Update_Model(TEntity model)
        {
            if (!_models_dic.Keys.Contains(model.ObjectId))
            {
                //pass
                // may it is not in db
                //_Add_Model(model);
                //throw new Exception();
            }
            else
            {
                _models_dic[model.ObjectId].modelArgs.model_entity = model;
                //models_dic[model.ObjectId].Refresh_ObjModel();
                OnUpdateModelArgs(_models_dic[model.ObjectId]);
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
            return _models_dic.Keys.Contains(objectId);
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
        //private TEntityArgs _get_ModelArgs(string objectId)
        //{
        //    if (_isExist_Model(objectId))
        //    {
        //        return models_dic[objectId];
        //    }
        //    return null;
        //}
        //public TEntityArgs get_ModelArgs(string objectId)
        //{
        //    open_Lock();
        //    TEntityArgs temp_args = _get_ModelArgs(objectId);
        //    close_Lock();

        //    return temp_args;
        //}
        //public List<TEntityArgs> _All_Items()
        //{
        //    return models_coll.ToList();
        //}
        //public List<TEntityArgs> All_Items()
        //{
        //    open_Lock();
        //    var temp_args = _All_Items();
        //    close_Lock();

        //    return temp_args;
        //}

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
                return _models_dic.Count;
            }
        }
        public int Rows_Count
        {
            get
            {
                return _models_coll.Count;
            }
        }

        #endregion

        #region For DBEvents
        private void refresh_pre_DBEvents()
        {
            /*
            if (addListener)
                dBManagersControlCore.GetAppDBManagersControl<TEntity,TEntityArgs>().OnModelArgs_Add += ModelDBManager_OnModel_Add;
            if (removeListener)
                dBManagersControlCore.GetAppDBManagersControl<TEntity,TEntityArgs>().OnModelArgs_Remove += ModelDBManager_OnModel_Remove;
            if (updateListener)
                dBManagersControlCore.GetAppDBManagersControl<TEntity,TEntityArgs>().OnModelArgs_Update += ModelDBManager_OnModel_Update;
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
        LockForLast lockForLast;
        void pre_refresh_Searching()
        {
            lockForLast = new LockForLast();
        }

        private bool _isSearchingMod = false;
        public bool isSearchingMod
        {
            protected set
            {
                if (_isSearchingMod != value)
                {
                    _isSearchingMod = value;
                    OnPropertyChanged(nameof(isSearchingMod));
                }
            }
            get => _isSearchingMod;
        }

        private void exit_SearchingMod()
        {
            if (isSearchingMod)
            {

            }
            else
            {
                // refresh list
            }
            isSearchingMod = false;
        }
        public void Exit_SearchingMod()
        {
            open_Lock();
            exit_SearchingMod();
            close_Lock();
        }

        public void Search(string name)
        {
            if(lockForLast.isContinue())
            {
                open_Lock();
                _Search(name);
                close_Lock();
                lockForLast.close_lock();
            }
        }
        private void _Search(string name)
        {
            if(name == "")
            {
                models_coll.Clear();
                foreach (var obj in models_dic.Values.ToArray())
                {
                    models_coll.Add(obj);
                }
                isSearchingMod = false;
                return;
            }
            isSearchingMod = true;
            models_coll.Clear();
            foreach (var obj in Search_ViewModel(name))
            {
                models_coll.Add(obj);
            }
        }

        protected abstract List<TEntityViewModel> Search_ViewModel(string name);

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

        //// like : on double click
        //public event EventHandler<TEntityArgs> SelectModelArgs;
        //protected virtual void OnSelectModelArgs(TEntityArgs modelArgs)
        //{
        //    if (SelectModelArgs != null)
        //    {
        //        SelectModelArgs(this, modelArgs);
        //    }
        //}
        //public void OnModelArgs_Selected(TEntityArgs modelArgs)
        //{
        //    //open_Lock();
        //    OnSelectModelArgs(modelArgs);
        //    //close_Lock();
        //}

        //// on get selected model
        //public delegate List<TEntityArgs> OnGetSelectedModel_Delegate();
        //public event OnGetSelectedModel_Delegate OnGetSelectedModel;
        //protected virtual List<TEntityArgs> GetSelectedModel()
        //{
        //    if (OnGetSelectedModel != null)
        //    {
        //        return OnGetSelectedModel();
        //    }
        //    return null;
        //}

        //public List<TEntityArgs> GetSelectedModels()
        //{
        //    return GetSelectedModel();
        //}


        #endregion

        #region fiers

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region To ModelView

        protected abstract TEntityViewModel To_ViewModel(TEntity model);
        protected abstract TEntityViewModel To_ViewModel(TEntityArgs modelArgs);

        #endregion

        #region For Waiting

        bool _isWaiting_State = false;
        public bool isWaiting_State
        {
            get
            {
                return _isWaiting_State;
            }
            private set
            {
                _isWaiting_State = value;
                OnPropertyChanged(nameof(isWaiting_State));
            }
        }

        public void run_waitingState()
        {
            isWaiting_State = true;
        }

        public void stop_waitingState()
        {
            isWaiting_State = false;
        }

        #endregion

        #region OnStartFirs

        public Action LoadStarting_Action { get; set; }
        public void OnLoadStarting()
        {
            if (LoadStarting_Action != null)
            {
                LoadStarting_Action?.Invoke();
            }
        }

        #endregion
    }
}
