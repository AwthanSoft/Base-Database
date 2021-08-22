using CommonAppCore.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAppCore.NotifierCore
{
   
    class ModelNotifierControlsManager : IDisposable
    {
        #region Initail
        readonly DBManagersControlCore dbManagerCore;
        public ModelNotifierControlsManager(DBManagersControlCore dbManagersControlCore)
        {
            this.dbManagerCore = dbManagersControlCore;
            pre_refresh();
        }
        private void pre_refresh()
        {
            objectLock = new ObjectLock();
            pre_refresh_Notifiers();
            pre_refresh_NotifyList();
            pre_refresh_Operations();
        }

        #region Lock Base Properties
        private ObjectLock objectLock;

        public void open_Lock()
        {
            //appControl.open_Lock();
            objectLock.open_lock();
        }
        public void close_Lock()
        {
            //appControl.close_Lock();
            objectLock.close_lock();
        }

        #endregion

        #endregion

        #region Notifiers

        Dictionary<Type, object> notifiers_Dic { set; get; }

        void pre_refresh_Notifiers()
        {
            notifiers_Dic = new Dictionary<Type, object>();
        }
        internal void AddNotifier<T>(ModelEventNotifier<T> modelEventNotifier)
            where T : DBModelCore
        {
            if (!notifiers_Dic.ContainsKey(typeof(T)))
            {
                notifiers_Dic.Add(typeof(T), new ModelNotifierControl<T>(modelEventNotifier));
            }
            else
            {
                throw new Exception();
            }
        }

        ModelNotifierControl<T> getModelNotifierCtrl<T>()
            where T : DBModelCore
        {
            return notifiers_Dic[typeof(T)] as ModelNotifierControl<T>;
        }

        #endregion

        #region NotifyList

        ObjectLock addToTemp_Lock;
        void pre_refresh_NotifyList()
        {
            addToTemp_Lock = new ObjectLock();
        }
        void _AddNotify_ToTemp<T>(ModelNotifierArgsCore<T> modelNotifierArgs)
            where T : DBModelCore
        {
            addToTemp_Lock.open_lock();
            getModelNotifierCtrl<T>().AddNotify_ToTemp(modelNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }
        void _AddNotify_ToTemp<T>(ModelNotifierArgsCore<T>[] modelNotifierArgs)
            where T : DBModelCore
        {
            addToTemp_Lock.open_lock();
            getModelNotifierCtrl<T>().AddNotify_ToTemp(modelNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }

        #endregion

        #region NotifyList Operation

        //add
        public void InsertNotiFy<T>(T model)
            where T : DBModelCore
        {
            open_Lock();
            _InsertNotiFy<T>(model);
            close_Lock();
        }
        void _InsertNotiFy<T>(T model)
            where T : DBModelCore
        {
            _AddNotify_ToTemp(new InsertModelNotifierArgs<T>(model));
        }
        public void InsertNotiFy<T>(T[] models)
            where T : DBModelCore
        {
            open_Lock();
            _InsertNotiFy<T>(models);
            close_Lock();
        }
        void _InsertNotiFy<T>(T[] entities)
            where T : DBModelCore
        {
            //var temp_args_list = new List<InsertModelNotifierArgs<T>>();
            //foreach(var model in entities)
            //{
            //    temp_args_list.Add(new InsertModelNotifierArgs<T>(model));
            //}
            //_AddNotify_ToTemp(temp_args_list.ToArray());
            _AddNotify_ToTemp((from b in entities select new InsertModelNotifierArgs<T>(b)).ToArray());
        }
        //Update
        public void UpdateNotiFy<T>(T model)
            where T : DBModelCore
        {
            open_Lock();
            _UpdateNotiFy<T>(model);
            close_Lock();
        }
        void _UpdateNotiFy<T>(T model)
            where T : DBModelCore
        {
            _AddNotify_ToTemp(new UpdateModelNotifierArgs<T>(model));
        }

        //Deleted
        public void DeleteNotiFy<T>(T model)
            where T : DBModelCore
        {
            open_Lock();
            _DeleteNotiFy<T>(model);
            close_Lock();
        }
        void _DeleteNotiFy<T>(T model)
            where T : DBModelCore
        {
            _AddNotify_ToTemp(new DeleteModelNotifierArgs<T>(model));
        }
        public void DeleteNotiFy<T>(T[] models)
            where T : DBModelCore
        {
            open_Lock();
            _DeleteNotiFy<T>(models);
            close_Lock();
        }
        void _DeleteNotiFy<T>(T[] models)
            where T : DBModelCore
        {
            _AddNotify_ToTemp((from b in models select new DeleteModelNotifierArgs<T>(b)).ToArray());
        }



        //Refresh
        public void RefreshNotiFy<T>()
            where T : DBModelCore
        {
            open_Lock();
            _RefreshNotiFy<T>();
            close_Lock();
        }
        void _RefreshNotiFy<T>()
            where T : DBModelCore
        {
            _AddNotify_ToTemp(new RefreshModelNotifierArgs<T>());
        }


        #endregion

        #region Operation
        LockForLast operation_LockForLast;
        void pre_refresh_Operations()
        {
            operation_LockForLast = new LockForLast();
        }

        void refreshOperation()
        {
            //if (!operation_LockForLast.isContinue())
            //    return;

            Task tsk = Task.Run(() =>
            {
                if (!operation_LockForLast.isContinue())
                    return;

                foreach (var ctrl in notifiers_Dic)
                {
                    try
                    {
                        //as temp
                        var temp_ctrl = ctrl.Value as IModelNotifierControl;

                        addToTemp_Lock.open_lock();
                        temp_ctrl.moveList_ToTemp();
                        addToTemp_Lock.close_lock();

                        temp_ctrl.refreshOperation();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Write("DB Notify Error : \n" + ex.ToString());
                    }
                }
                
                operation_LockForLast.close_lock();
            });
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
            foreach (var ctrl in notifiers_Dic.Values)
            {
            }*/
            notifiers_Dic.Clear();

            addToTemp_Lock.Dispose();
            operation_LockForLast.Dispose();
            objectLock.Dispose();

            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~ModelNotifierControlsManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
