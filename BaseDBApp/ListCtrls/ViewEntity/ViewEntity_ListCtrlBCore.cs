using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Mawa.Lock;

namespace DBAppCore.ListCtrls.ViewEntity
{
    public delegate TViewEntity[] OnTViewEntity_Arr_Delegate<TViewEntity>()
    where TViewEntity : ViewEntityCore.ModelViewEntityCore;

    public delegate TViewEntity[] OnViewEntity_SearchArr_Delegate<TViewEntity>(string searchStr)
    where TViewEntity : ViewEntityCore.ModelViewEntityCore;
    
    public abstract class ViewEntity_ListCtrlBCore<TViewEntity> : IDisposable, INotifyPropertyChanged
        where TViewEntity : ViewEntityCore.ModelViewEntityCore
    {

        #region Initilal
        protected readonly DBManagersControlCore dBManagersControlCore;
        public ViewEntity_ListCtrlBCore(DBManagersControlCore dBManagersControlCore)
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
        protected Dictionary<string, TViewEntity> viewEntity_dic { private set; get; }
        public TViewEntity[] ViewEntities => viewEntity_dic.Values.ToArray();
        void pre_refresh_Dic()
        {
            viewEntity_dic = new Dictionary<string, TViewEntity>();
        }

        void OnModelsChanged()
        {
            OnPropertyChanged(nameof(ViewEntities));
            OnPropertyChanged(nameof(RowsCounter));
        }


        void _Add_To_Dic(TViewEntity[] viewEntitys)
        {
            foreach (var mv in viewEntitys)
            {
                if (!_isExist_ViewEntity(mv.ObjectId))
                {
                    viewEntity_dic.Add(mv.ObjectId, mv);
                }
                else
                {
                    throw new Exception();
                }
            }
            OnModelsChanged();
        }
        void _Add_To_Dic(TViewEntity viewEntity)
        {
            if (viewEntity_dic.ContainsKey(viewEntity.ObjectId))
            {
                viewEntity_dic.Add(viewEntity.ObjectId, viewEntity);
                OnModelsChanged();
            }
            else
            {
                throw new Exception();
            }
        }
        void _Remove_To_Dic(string ObjectId)
        {
            if(viewEntity_dic.ContainsKey(ObjectId))
            {
                viewEntity_dic.Remove(ObjectId);
                OnModelsChanged();
            }
            else
            {
                throw new Exception();
            }
        }
        void _Update_To_Dic(TViewEntity viewEntity)
        {
            viewEntity_dic[viewEntity.ObjectId] = viewEntity;
            OnModelsChanged();
        }


        #endregion

        #region Counters

        int _ViewEntitiesCounter;
        public int ViewEntitiesCounter
        {
            get => _ViewEntitiesCounter;
            protected set
            {
                _ViewEntitiesCounter = value;
                OnPropertyChanged(nameof(ViewEntitiesCounter));
            }
        }
        public int RowsCounter => viewEntity_dic.Count;

        #endregion


        #region Load

        //public void LoadModels(IQueryable<TViewEntity> iQueryable)
        //{
        //    open_Lock();
        //    Open_Load();
        //    _LoadModels(iQueryable);
        //    Close_Load();
        //    close_Lock();
        //}
        //protected void _LoadModels(IQueryable<TViewEntity> iQueryable)
        //{
        //    viewEntity_dic.Clear();
        //    //dBManagersControlCore.open_Lock();
        //    _Add_To_Dic(iQueryable.ToArray());
        //    //dBManagersControlCore.close_Lock();
        //}
        protected void _LoadModels(TViewEntity[] viewEntitys)
        {
            //Open_Load();
            viewEntity_dic.Clear();
            _Add_To_Dic(viewEntitys);
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


        #region For Add 

        //adding
        protected void _Add_ViewEntity(TViewEntity viewEntity)
        {
            if (_isExist_ViewEntity(viewEntity.ObjectId))
            {
                // for update with new Model
                _Update_ViewEntity(viewEntity);
            }
            else
            {
                _Add_To_Dic(viewEntity);
            }
        }
        public void Add_ViewEntity(TViewEntity viewEntity)
        {
            open_Lock();
            _Add_ViewEntity(viewEntity);
            close_Lock();
        }

        #endregion

        #region For Remove

        protected void _Remove_ViewEntity(TViewEntity viewEntity)
        {
            if (!_isExist_ViewEntity(viewEntity.ObjectId))
            {
                throw new Exception();
                // for update with new Model
                // may model is not exist from db
            }
            else
            {
                _Remove_To_Dic(viewEntity.ObjectId);
            }
        }
        protected void _Remove_ViewEntity(string viewEntity_ObjectId)
        {
            if (!_isExist_ViewEntity(viewEntity_ObjectId))
            {
                // for update with new Model
                // may model is not exist from db
                // as temp
                //throw new Exception();
            }
            else
            {
                _Remove_To_Dic(viewEntity_ObjectId);
            }
        }
        public void Remove_ViewEntity(string viewEntity_ObjectId)
        {
            open_Lock();
            _Remove_ViewEntity(viewEntity_ObjectId);
            close_Lock();
        }
        public void Remove_ViewEntity(TViewEntity model)
        {
            open_Lock();
            _Remove_ViewEntity(model);
            close_Lock();
        }

        #endregion

        #region For Update

        protected void _Update_ViewEntity(TViewEntity viewEntity)
        {
            if (_isExist_ViewEntity(viewEntity.ObjectId))
            {
                _Update_To_Dic(viewEntity);
            }
            else
            {
                //pass
                // may it is not in db
                //_Add_Model(model);
                throw new Exception();
            }
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
        protected bool _isExist_ViewEntity(string objectId)
        {
            return viewEntity_dic.Keys.Contains(objectId);
        }
        public bool isExist_ViewEntity(string model_ObjectId)
        {
            bool resultt = false;
            open_Lock();
            resultt = _isExist_ViewEntity(model_ObjectId);
            close_Lock();
            return resultt;
        }

        #endregion


        #region For Searching

        public event OnViewEntity_SearchArr_Delegate<TViewEntity> OnSearch_ViewEntity;
        protected virtual TViewEntity[] Search_ViewEntity(string searchStr)
        {
            return OnSearch_ViewEntity?.Invoke(searchStr);
            //if (OnSearch_ViewEntity != null)
            //{

            //}
            //return null;
        }

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

        public void Search(string searchStr)
        {
            Task.Run(() =>
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
            });
        }
        private void _Search(string searchStr)
        {
            TViewEntity[] temp_list;
            if (searchStr == "")
            {
                temp_list = _GetMainRefreshList();
                //isSearchingMod = false;
                //return;
            }
            else
            {
                isSearchingMod = true;
                if (OnSearch_ViewEntity != null)
                {
                    temp_list = Search_ViewEntity(searchStr);
                }
                else
                    temp_list = Search_ViewModel(searchStr);
            }
            
            _LoadModels(temp_list);
        }

        protected abstract TViewEntity[] Search_ViewModel(string searchStr);

        #endregion

        #region Refresh List

        public event OnTViewEntity_Arr_Delegate<TViewEntity> OnMainRefreshList;

        void pre_refresh_MainRefreshList()
        {

        }

        protected abstract TViewEntity[] OnGetMainRefreshList();
        private TViewEntity[] _GetMainRefreshList()
        {
            TViewEntity[] temp_arr;
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
            ViewEntitiesCounter = RowsCounter;
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

            OnSearch_ViewEntity = null;
            OnMainRefreshList = null;

            viewEntity_dic.Clear();
            viewEntity_dic  = null;

            objectLock.Dispose();
            lockForLast.Dispose();

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ViewEntity_ListCtrlBCore()
        {
            Dispose(false);
        }

        protected abstract void Dispose_FreeManaged();
        protected abstract void Dispose_FreeUnManaged();

        #endregion
    }
}
