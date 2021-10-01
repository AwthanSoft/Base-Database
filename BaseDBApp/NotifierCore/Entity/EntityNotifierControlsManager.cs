using Mawa.Lock;
using DBAppCore.EntityCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DBAppCore.NotifierCore.Entity
{
    class EntityNotifierControlsManager : IDisposable
    {
        #region Initail
        readonly DBManagersControlCore dbManagerCore;
        public EntityNotifierControlsManager(DBManagersControlCore dbManagersControlCore)
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
        internal void AddNotifier<T>(EntityEventNotifier<T> entityEventNotifier)
            where T : ModelEntityCore
        {
            if (!notifiers_Dic.ContainsKey(typeof(T)))
            {
                notifiers_Dic.Add(typeof(T), new EntityNotifierControl<T>(entityEventNotifier));
            }
            else
            {
                throw new Exception();
            }
        }

        EntityNotifierControl<T> getEntityNotifierCtrl<T>()
            where T : ModelEntityCore
        {
            return notifiers_Dic[typeof(T)] as EntityNotifierControl<T>;
        }

        #endregion

        #region NotifyList

        ObjectLock addToTemp_Lock;
        void pre_refresh_NotifyList()
        {
            addToTemp_Lock = new ObjectLock();
        }
        void _AddNotify_ToTemp<T>(EntityNotifierArgsCore<T> entityNotifierArgs)
            where T : ModelEntityCore
        {
            addToTemp_Lock.open_lock();
            getEntityNotifierCtrl<T>().AddNotify_ToTemp(entityNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }
        void _AddNotify_ToTemp<T>(EntityNotifierArgsCore<T>[] entityNotifierArgs)
            where T : ModelEntityCore
        {
            addToTemp_Lock.open_lock();
            getEntityNotifierCtrl<T>().AddNotify_ToTemp(entityNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }

        #endregion

        #region NotifyList Operation

        //add
        public void InsertNotiFy<T>(T entity)
            where T : ModelEntityCore
        {
            open_Lock();
            _InsertNotiFy<T>(entity);
            close_Lock();
        }
        void _InsertNotiFy<T>(T entity)
            where T : ModelEntityCore 
        {
            _AddNotify_ToTemp(new InsertEntityNotifierArgs<T>(entity));
        }
        public void InsertNotiFy<T>(T[] entities)
            where T : ModelEntityCore
        {
            open_Lock();
            _InsertNotiFy<T>(entities);
            close_Lock();
        }
        void _InsertNotiFy<T>(T[] entities)
            where T : ModelEntityCore
        {
            var temp_args_list = new List<InsertEntityNotifierArgs<T>>();
            foreach(var model in entities)
            {
                temp_args_list.Add(new InsertEntityNotifierArgs<T>(model));
            }
            _AddNotify_ToTemp(temp_args_list.ToArray());
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
                    var temp_ctrl = ctrl.Value as IEntityNotifierControl;

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
        ~EntityNotifierControlsManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
