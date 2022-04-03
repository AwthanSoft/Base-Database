using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using Mawa.DBCore.ViewEntityCore;
using System.Threading.Tasks;
using Mawa.Lock;
using Mawa.BaseDBCore.ViewEntityCore;

namespace Mawa.DBCore.ListCtrls.ViewEntity
{
    public delegate TViewEntityModelView[] OnTViewEntityModelView_Arr_Delegate<TViewEntity ,TViewEntityModelView>()
        where TViewEntity : ModelViewEntityCore
        where TViewEntityModelView : ViewEntityViewModel<TViewEntity>;
    
    public delegate TViewEntityModelView[] OnViewEntityModelView_SearchArr_Delegate<TViewEntity, TViewEntityModelView>(string searchStr)
        where TViewEntity : ModelViewEntityCore
        where TViewEntityModelView : ViewEntityViewModel<TViewEntity>;
     
    //public delegate TViewEntity[] OnViewEntity_SearchArr_Delegate<TViewEntity>(string searchStr)
    //where TViewEntity : ViewEntity;

    public abstract class ViewEntityViewModel_ListCtrlBCore<TViewEntity, TViewEntityModelView> : IDisposable, INotifyPropertyChanged
        where TViewEntity : ModelViewEntityCore
        where TViewEntityModelView : ViewEntityViewModel<TViewEntity>
    {

        #region Initilal
        protected readonly DBManagersControlCore dBManagersControlCore;
        public ViewEntityViewModel_ListCtrlBCore(DBManagersControlCore dBManagersControlCore)
        {
            this.dBManagersControlCore = dBManagersControlCore;
            objectLock = new ObjectLock();

            pre_refresh();
        }
        //public EntityViewModel_ListCtrlB()
        //{
        //    objectLock = new ObjectLock();
        //    pre_refresh();
        //}
        private void pre_refresh()
        {
            pre_refresh_Dic();
            pre_refresh_Searching();
            pre_refresh_MainRefreshList();

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


        #region Dic
        public Dictionary<string, TViewEntityModelView> modelViews_dic { private set; get; }
        public TViewEntityModelView[] modelViews => modelViews_dic.Values.ToArray();
        void pre_refresh_Dic()
        {
            modelViews_dic = new Dictionary<string, TViewEntityModelView>();
        }

        void OnModelsChanged()
        {
            OnPropertyChanged(nameof(modelViews));
            OnPropertyChanged(nameof(RowsCounter));
        }


        void _Add_To_Dic(TViewEntityModelView[] modelViews)
        {
            foreach (var mv in modelViews)
            {
                modelViews_dic.Add(mv.ObjectId, mv);
            }
            OnModelsChanged();
        }
        void _Add_To_Dic(TViewEntityModelView modelView)
        {
            if (modelViews_dic.ContainsKey(modelView.ObjectId))
            {
                modelViews_dic.Add(modelView.ObjectId, modelView);
                OnModelsChanged();
            }
            else
            {
                //throw new Exception();
                _Update_To_Dic(modelView);
            }
        }
        void _Remove_To_Dic(string ObjectId)
        {
            if(modelViews_dic.ContainsKey(ObjectId))
            {
                modelViews_dic.Remove(ObjectId);
                OnModelsChanged();
            }
            else
            {
                throw new Exception();
            }
        }
        void _Update_To_Dic(TViewEntityModelView modelView)
        {
            modelViews_dic[modelView.ObjectId] = modelView;
            OnModelsChanged();
        }


        #endregion

        #region Counters

        int _ModelViewsCounter;
        public int ModelViewsCounter
        {
            get => _ModelViewsCounter;
            protected set
            {
                _ModelViewsCounter = value;
                OnPropertyChanged(nameof(ModelViewsCounter));
            }
        }
        public int RowsCounter => modelViews_dic.Count;

        #endregion


        #region Load

        public void LoadModels(IQueryable<TViewEntityModelView> iQueryable)
        {
            open_Lock();
            Open_Load();
            _LoadModels(iQueryable);
            Close_Load();
            close_Lock();
        }
        protected void _LoadModels(IQueryable<TViewEntityModelView> iQueryable)
        {
            modelViews_dic.Clear();
            //dBManagersControlCore.open_Lock();
            _Add_To_Dic(iQueryable.ToArray());
            //dBManagersControlCore.close_Lock();
        }
        protected void _LoadModels(TViewEntityModelView[] viewModels)
        {
            //Open_Load();
            modelViews_dic.Clear();
            _Add_To_Dic(viewModels);
            //Close_Load();
        }

        #endregion

        #region LoadingState

        bool _LoadingState;
        public bool LoadingState
        {
            get => _LoadingState;
            private set
            {
                _LoadingState = value;
                OnPropertyChanged(nameof(LoadingState));
            }
        }

        void Open_Load()
        {
            LoadingState = true;
        }
        void Close_Load()
        {
            LoadingState = false;
        }

        #endregion

        #region WaitingState

        bool _WaitingState;
        public bool WaitingState
        {
            get => _WaitingState;
            private set
            {
                _WaitingState = value;
                OnPropertyChanged(nameof(WaitingState));
            }
        }

        public void Open_Wait()
        {
            WaitingState = true;
        }
        public void Close_Wait()
        {
            WaitingState = false;
        }

        #endregion


        #region For Add 

        //adding
        protected void _Add_ModelView(TViewEntity viewEntity)
        {
            if (_isExist_ModelView(viewEntity.ModelObjectId))
            {
                // for update with new Model
                _Update_Model(viewEntity);
            }
            else
            {
                TViewEntityModelView modelView = To_ModelView(viewEntity);
                _Add_To_Dic(modelView);
            }
        }
        public void Add_Model(TViewEntity viewEntity)
        {
            open_Lock();
            _Add_ModelView(viewEntity);
            close_Lock();
        }

        #endregion

        #region For Remove

        protected void _Remove_ViewEntity(TViewEntity viewEntity)
        {
            if (!_isExist_ModelView(viewEntity.ModelObjectId))
            {
                throw new Exception();
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                _Remove_To_Dic(viewEntity.ModelObjectId);
            }
        }
        protected void _Remove_ModelView(string modelView_ObjectId)
        {
            if (!_isExist_ModelView(modelView_ObjectId))
            {
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                _Remove_To_Dic(modelView_ObjectId);
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
            _Remove_ViewEntity(model);
            close_Lock();
        }

        #endregion

        #region For Update
        //Update_ModelView
        public void Update_ModelView(TViewEntityModelView modelView)
        {
            open_Lock();
            _Update_ModelView(modelView);
            close_Lock();
        }
        protected void _Update_ModelView(TViewEntityModelView modelView)
        {
            if(_isExist_ModelView(modelView.ObjectId))
            {
                _Update_To_Dic(modelView);
            }
            else
            {
                throw new Exception();
            }
        }
        //Update_Model
        public void Update_Model(TViewEntity model)
        {
            open_Lock();
            _Update_Model(model);
            close_Lock();
        }
        protected void _Update_Model(TViewEntity model)
        {
            if (_isExist_ModelView(model.ModelObjectId))
            {
                TViewEntityModelView modelView = modelViews_dic[model.ModelObjectId];
                modelView.viewEntity = model;
                _Update_To_Dic(modelView);
            }
            else
            {
                //pass or add // may is not in the list of view like
                // user view only some entity
                // so refresh only exist
            }
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


        #region For Searching
        public event OnViewEntityModelView_SearchArr_Delegate<TViewEntity, TViewEntityModelView> OnSearch_ViewEntityModelView;
        public event OnViewEntity_SearchArr_Delegate<TViewEntity> OnSearch_ViewEntity;
        
        protected virtual TViewEntityModelView[] Search_ViewEntityModelView(string searchStr)
        {
            if(OnSearch_ViewEntityModelView != null)
            {
                return OnSearch_ViewEntityModelView?.Invoke(searchStr);
            }
            return null;
        }
        protected virtual TViewEntityModelView[] Search_ViewEntity(string searchStr)
        {
            if (OnSearch_ViewEntity != null)
            {
                List<TViewEntityModelView> temp = new List<TViewEntityModelView>();
                foreach (var md in OnSearch_ViewEntity?.Invoke(searchStr))
                {
                    temp.Add(To_ModelView(md));
                }
                return temp.ToArray();
            }
            return null;
        }

        LockForLast lockForLast;
        string _Last_SearchStr;
        public string Last_SearchStr => _Last_SearchStr;

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

        public void Search(string searchStr)
        {
            if (lockForLast.isContinue())
            {
                open_Lock();
                Open_Load();
                _Search(searchStr);
                Close_Load();
                close_Lock();
                lockForLast.close_lock();
            }
        }
        public async Task SearchAsync(string searchStr)
        {
            await Task.Run(() =>
            {
                Search(searchStr);
            });
        }
        private void _Search(string searchStr)
        {
            _Last_SearchStr = searchStr;
            TViewEntityModelView[] temp_list;
            if (searchStr == "")
            {
                temp_list = _GetMainRefreshList();
                //isSearchingMod = false;
                //return;
            }
            else
            {
                isSearchingMod = true;
                if (OnSearch_ViewEntityModelView != null)
                {
                    temp_list = Search_ViewEntityModelView(searchStr);
                }
                else
                {
                    if(OnSearch_ViewEntity != null)
                    {
                        temp_list = Search_ViewEntity(searchStr);
                    }
                    else
                        temp_list = Search_ViewModel(searchStr);
                }
            }
            
            _LoadModels(temp_list);
        }

        protected abstract TViewEntityModelView[] Search_ViewModel(string searchStr);

        #endregion

        #region Refresh List

        public event OnTViewEntityModelView_Arr_Delegate<TViewEntity,TViewEntityModelView> OnMainRefreshList;

        void pre_refresh_MainRefreshList()
        {

        }

        protected abstract TViewEntityModelView[] OnGetMainRefreshList();
        private TViewEntityModelView[] _GetMainRefreshList()
        {
            TViewEntityModelView[] temp_arr;
            if (OnMainRefreshList != null)
            {
                temp_arr = OnMainRefreshList?.Invoke();
                //if (temp_arr == null)
                //{
                //    temp_arr = new TViewEntityModelView[0];
                //}
            }
            else
            {
                temp_arr = OnGetMainRefreshList();
            }
            
            return temp_arr;
        }
        public void RefreshList()
        {
            open_Lock();
            Open_Load();
            _LoadModels(_GetMainRefreshList());
            Close_Load();
            ModelViewsCounter = RowsCounter;
            close_Lock();
        }

        #endregion


        #region DBLiteners


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
                Dispose_FreeManaged();
            }

            // Free any unmanaged objects here.
            Dispose_FreeUnManaged();

            //OnGetSelectedModel = null;
            //SelectModelArgs = null;

            OnSearch_ViewEntityModelView = null;
            OnMainRefreshList = null;

            modelViews_dic.Clear();
            modelViews_dic  = null;

            objectLock.Dispose();
            lockForLast.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ViewEntityViewModel_ListCtrlBCore()
        {
            Dispose(false);
        }

        protected abstract void Dispose_FreeManaged();
        protected abstract void Dispose_FreeUnManaged();

        #endregion
    }
}
