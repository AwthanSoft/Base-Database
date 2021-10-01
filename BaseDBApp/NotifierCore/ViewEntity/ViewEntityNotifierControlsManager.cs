using Mawa.Lock;
using DBAppCore.ViewEntityCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DBAppCore.NotifierCore.ViewEntity
{
   
    class ViewEntityNotifierControlsManager : IDisposable
    {
        #region Initail
        readonly DBManagersControlCore dbManagerCore;
        public ViewEntityNotifierControlsManager(DBManagersControlCore dbManagersControlCore)
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
        internal void AddNotifier<T>(ViewEntityEventNotifier<T> viewEntityEventNotifier)
            where T : ViewEntityCore.ModelViewEntityCore
        {
            if (!notifiers_Dic.ContainsKey(typeof(T)))
            {
                notifiers_Dic.Add(typeof(T), new ViewEntityNotifierControl<T>(viewEntityEventNotifier));
            }
            else
            {
                throw new Exception();
            }
        }

        ViewEntityNotifierControl<T> getViewEntityNotifierCtrl<T>()
            where T : ViewEntityCore.ModelViewEntityCore
        {
            return notifiers_Dic[typeof(T)] as ViewEntityNotifierControl<T>;
        }

        #endregion

        #region NotifyList

        ObjectLock addToTemp_Lock;
        void pre_refresh_NotifyList()
        {
            addToTemp_Lock = new ObjectLock();
        }
        void _AddNotify_ToTemp<T>(ViewEntityNotifierArgsCore<T> viewEntityNotifierArgs)
            where T : ViewEntityCore.ModelViewEntityCore
        {
            addToTemp_Lock.open_lock();
            getViewEntityNotifierCtrl<T>().AddNotify_ToTemp(viewEntityNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }
        void _AddNotify_ToTemp<T>(ViewEntityNotifierArgsCore<T>[] viewEntityNotifierArgs)
            where T : ViewEntityCore.ModelViewEntityCore
        {
            addToTemp_Lock.open_lock();
            getViewEntityNotifierCtrl<T>().AddNotify_ToTemp(viewEntityNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }

        #endregion

        #region NotifyList Operation

        //add
        public void InsertNotiFy<T>(T entity)
            where T : ViewEntityCore.ModelViewEntityCore
        {
            open_Lock();
            _InsertNotiFy<T>(entity);
            close_Lock();
        }
        void _InsertNotiFy<T>(T entity)
            where T : ViewEntityCore.ModelViewEntityCore
        {
            _AddNotify_ToTemp(new InsertViewEntityNotifierArgs<T>(entity));
        }
        public void InsertNotiFy<T>(T[] entities)
            where T : ViewEntityCore.ModelViewEntityCore
        {
            open_Lock();
            _InsertNotiFy<T>(entities);
            close_Lock();
        }
        void _InsertNotiFy<T>(T[] entities)
            where T : ViewEntityCore.ModelViewEntityCore
        {
            var temp_args_list = new List<InsertViewEntityNotifierArgs<T>>();
            foreach(var model in entities)
            {
                temp_args_list.Add(new InsertViewEntityNotifierArgs<T>(model));
            }
            _AddNotify_ToTemp(temp_args_list.ToArray());
        }
        //Refresh
        public void RefreshNotiFy<T>()
            where T : ViewEntityCore.ModelViewEntityCore
        {
            open_Lock();
            _RefreshNotiFy<T>();
            close_Lock();
        }
        void _RefreshNotiFy<T>()
            where T : ViewEntityCore.ModelViewEntityCore
        {
            _AddNotify_ToTemp(new RefreshViewEntityNotifierArgs<T>());
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
            if (!operation_LockForLast.isContinue())
                return;

            Task tsk = Task.Run(() =>
            {
                foreach(var ctrl in notifiers_Dic)
                {
                    var temp_ctrl = ctrl.Value as IViewEntityNotifierControl;

                    addToTemp_Lock.open_lock();
                    temp_ctrl.moveList_ToTemp();
                    addToTemp_Lock.close_lock();

                    temp_ctrl.refreshOperation();
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
        ~ViewEntityNotifierControlsManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
