using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using Mawa.DBCore.ViewEntityCore;
using Mawa.Lock;
using Mawa.BaseApp;
using Mawa.BaseDBCore.ViewEntityCore;

namespace Mawa.DBCore.ViewEntityListCtrls
{
    
    public abstract class EntityViewModel_ListCtrl<TViewEntity, TViewEntityModelView> : IDisposable, INotifyPropertyChanged
        where TViewEntity : ModelViewEntityCore
        where TViewEntityModelView : ViewEntityViewModel<TViewEntity>
    {

        #region Initilal

        public EntityViewModel_ListCtrl()
        {
            objectLock = new ObjectLock();

            pre_refresh_Coll();
            pre_refresh_Counters();

            pre_refresh_Searching();


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
        public ObservableCollection<TViewEntityModelView> modelViews_coll { private set; get; }
        public Dictionary<string, TViewEntityModelView> modelViews_dic { private set; get; }

        void pre_refresh_Coll()
        {
            modelViews_coll = new ObservableCollection<TViewEntityModelView>();
            modelViews_coll.CollectionChanged += ModelViews_coll_CollectionChanged;

            modelViews_dic = new Dictionary<string, TViewEntityModelView>();
        }

        private void ModelViews_coll_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _Add_To_Observable(string key, TViewEntityModelView modelView)
        {
            //Device.InvokeOnMainThreadAsync(() =>
            MainAppThread.Invoke(() =>
            {
                modelViews_dic.Add(key, modelView);
                modelViews_coll.Add(modelView);
                Coll_ModelViews_Change();
            });
        }
        void _Remove_To_Observable(string key, TViewEntityModelView modelView)
        {
            //Device.BeginInvokeOnMainThread(() =>
            MainAppThread.Invoke(() =>
            {
                modelViews_dic.Remove(key);
                if (modelViews_coll.Contains(modelView))
                {
                    modelViews_coll.Remove(modelView);
                }
                Coll_ModelViews_Change();
            });
        }
        void _Update_To_Observable(string key, TViewEntityModelView modelView)
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

        void Coll_ModelViews_Change()
        {
            OnPropertyChanged(nameof(ModelViewsCounter));
            OnPropertyChanged(nameof(RowsCounter));
        }

        #endregion


        #region For Add 

        // events
        public event EventHandler<TViewEntityModelView> AddModelView;
        protected virtual void OnAddModelView(TViewEntityModelView modelView)
        {
            if (AddModelView != null)
            {
                AddModelView?.Invoke(this, modelView);
            }
        }

        //adding
        private void _Add_ModelView(TViewEntity model)
        {
            if (_isExist_ModelView(model.ObjectId))
            {
                // for update with new Model
                _Update_ModelView(model);
            }
            else
            {
                TViewEntityModelView modelView = To_ModelView(model);
                _Add_To_Observable(modelView.ObjectId, modelView);
                OnAddModelView(modelView);
            }
        }
        public void Add_Model(TViewEntity model)
        {
            open_Lock();
            _Add_ModelView(model);
            close_Lock();
        }

        #endregion

        #region For Remove

        public event EventHandler<TViewEntityModelView> RemoveModelView;
        protected virtual void OnRemoveModelView(TViewEntityModelView modelView)
        {
            if (RemoveModelView != null)
            {
                RemoveModelView(this, modelView);
            }
        }
        // remove
        private void _Remove_ModelView(TViewEntity model)
        {
            if (!_isExist_ModelView(model.ObjectId))
            {
                throw new Exception();
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                TViewEntityModelView modelView = modelViews_dic[model.ObjectId];
                _Remove_To_Observable(model.ObjectId , modelView);
                OnRemoveModelView(modelView);
            }
        }
        private void _Remove_ModelView(string modelView_ObjectId)
        {
            if (!_isExist_ModelView(modelView_ObjectId))
            {
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                var temp_modelView = modelViews_dic[modelView_ObjectId];
                _Remove_To_Observable(modelView_ObjectId, temp_modelView);
                OnRemoveModelView(temp_modelView);
            }
        }
        public void Remove_ModelView(string modelView_ObjectId)
        {
            open_Lock();
            _Remove_ModelView(modelView_ObjectId);
            close_Lock();
        }
        public void Remove_ModelView(TViewEntity model)
        {
            open_Lock();
            _Remove_ModelView(model);
            close_Lock();
        }

        #endregion

        #region For Update

        public event EventHandler<TViewEntityModelView> UpdateModelView;
        protected virtual void OnUpdateModelView(TViewEntityModelView modelView)
        {
            if (UpdateModelView != null)
            {
                UpdateModelView(this, modelView);
            }
            //CountModels_Change();
        }
        private void _Update_ModelView(TViewEntity model)
        {
            if (!modelViews_dic.Keys.Contains(model.ObjectId))
            {
                //pass
                // may it is not in db
                //_Add_Model(model);
                //throw new Exception();
            }
            else
            {
                modelViews_dic[model.ObjectId].viewEntity = model;
                //models_dic[model.ObjectId].Refresh_ObjModel();
                OnUpdateModelView(modelViews_dic[model.ObjectId]);
            }
        }
        public void Update_ModelView(TViewEntity model)
        {
            open_Lock();
            _Update_ModelView(model);
            close_Lock();
        }
        private void _Update_ModelView(TViewEntityModelView modelView)
        {
            _Update_ModelView(modelView.viewEntity);
        }

        #endregion


        #region For Checking
        // base Exisiting
        private bool _isExist_ModelView(string objectId)
        {
            return modelViews_dic.Keys.Contains(objectId);
        }
        public bool isExist_ModelView(string model_ObjectId)
        {
            bool resultt = false;
            open_Lock();
            resultt = _isExist_ModelView(model_ObjectId);
            close_Lock();
            return resultt;
        }
        
        #endregion


        #region Counters
        public int ModelViewsCounter => modelViews_dic.Count;
        public int RowsCounter => modelViews_coll.Count;
        void pre_refresh_Counters()
        {

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
                modelViews_coll.Clear();
                foreach (var obj in modelViews_dic.Values.ToArray())
                {
                    modelViews_coll.Add(obj);
                }
                //isSearchingMod = false;
                return;
            }
            isSearchingMod = true;
            modelViews_coll.Clear();
            foreach (var obj in Search_ViewModel(name))
            {
                modelViews_coll.Add(obj);
            }
        }

        protected abstract List<TViewEntityModelView> Search_ViewModel(string name);

        #endregion

        #region Refresh List

        public void RefreshList(TViewEntity[] modelViews_list)
        {
            open_Lock();
            _RefreshList(modelViews_list);
            close_Lock();
        }
        void _RefreshList(TViewEntity[] modelViews_list)
        {
            run_waitingState();

            modelViews_dic.Clear();
            MainAppThread.Invoke(() =>
            {
                modelViews_coll.Clear();
            });
            
            foreach(var model in modelViews_list)
            {
                _Add_ModelView(model);
            }

            stop_waitingState();
        }

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



        #region To ModelView

        protected abstract TViewEntityModelView To_ModelView(TViewEntity model);

        #endregion

        #region For Waiting

        bool __isWaiting_State = false;
        private bool _isWaiting_State
        {
            get
            {
                return __isWaiting_State;
            }
            set
            {
                __isWaiting_State = value;
                OnPropertyChanged(nameof(isWaiting_State));
            }
        }
        public bool isWaiting_State => _isWaiting_State;

        public void run_waitingState()
        {
            _isWaiting_State = true;
        }

        public void stop_waitingState()
        {
            _isWaiting_State = false;
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


            //OnGetSelectedModel = null;
            //SelectModelArgs = null;

            AddModelView = null;
            RemoveModelView = null;
            UpdateModelView = null;
            //OnCountModels_Change = null;

            modelViews_dic.Clear();
            modelViews_dic  = null;

            //modelViews_coll.Clear();
            //modelViews_coll = null;
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
    }
}
