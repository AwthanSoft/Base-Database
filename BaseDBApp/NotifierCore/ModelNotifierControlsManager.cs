using Mawa.BaseDBCore;
using Mawa.Lock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mawa.DBCore.NotifierCore
{
    //class ModelNotifierControlsManager : IDisposable
    public class ModelNotifierControlsManager : IDisposable // as temp public
    {
        #region Initail
        
        private readonly ObjectLock objectLock;
        public ModelNotifierControlsManager()
        {
            objectLock = new ObjectLock();
            pre_refresh();
        }
        private void pre_refresh()
        {
            pre_refresh_Notifiers();
            pre_refresh_NotifyList();
            pre_refresh_Operations();
        }
        #endregion

        #region Notifiers

        Dictionary<Type, object> notifiers_Dic { set; get; }

        void pre_refresh_Notifiers()
        {
            notifiers_Dic = new Dictionary<Type, object>();
        }
        //void AddNotifier<T, TId>(ModelEventNotifier<T, TId> modelEventNotifier)
        public void AddNotifier<T, TId>(ModelEventNotifier<T, TId> modelEventNotifier) // as temp public
            where T : class, IDBModelCore
 
        {
            if (!notifiers_Dic.ContainsKey(typeof(T)))
            {
                notifiers_Dic.Add(typeof(T), new ModelNotifierControl<T, TId>(modelEventNotifier));
            }
            else
            {
                throw new Exception();
            }
        }


        ModelNotifierControl<T, TId> getModelNotifierCtrl<T, TId>()
            where T : class, IDBModelCore
        {
            return notifiers_Dic[typeof(T)] as ModelNotifierControl<T, TId>;
        }

        //public ModelEventNotifier<T, TId> GetModelEventNotifier<T, TId>()
        // where T : class, IDBModelCore
        //{
        //    return getModelNotifierCtrl<T, TId>().ModelEventNotifier;
        //}
        #endregion

        #region NotifyList

        ObjectLock addToTemp_Lock;
        void pre_refresh_NotifyList()
        {
            addToTemp_Lock = new ObjectLock();
        }
        void _AddNotify_ToTemp<T, TId>(ModelNotifierArgsCore<T, TId> modelNotifierArgs)
            where T : class, IDBModelCore
        {
            addToTemp_Lock.open_lock();
            getModelNotifierCtrl<T, TId>().AddNotify_ToTemp(modelNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }
        void _AddNotify_ToTemp<T, TId>(ModelNotifierArgsCore<T, TId>[] modelNotifierArgs)
            where T : class, IDBModelCore
 
        {
            addToTemp_Lock.open_lock();
            getModelNotifierCtrl<T, TId>().AddNotify_ToTemp(modelNotifierArgs);
            addToTemp_Lock.close_lock();
            refreshOperation();
        }

        #endregion

        #region NotifyList Operation


        //General Notify
        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId)
          where T : class, IDBModelCore
        {
            lock (objectLock.opening_Lock)
            {
                _ModelNotify<T, TId>(notifierType, modelId);
            }
        }
        void _ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId)
            where T : class, IDBModelCore
        {
            _AddNotify_ToTemp(new ModelNotifierArgs<T, TId>(notifierType, modelId, null));
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, T model, TId defualtNull)
          where T : class, IDBModelCore
        {
            lock (objectLock.opening_Lock)
            {
                _ModelNotify<T, TId>(notifierType, model, defualtNull);
            }
        }
        void _ModelNotify<T, TId>(DBModelNotifierType notifierType, T model, TId defualtNull)
            where T : class, IDBModelCore
        {
            _AddNotify_ToTemp(new ModelNotifierArgs<T, TId>(notifierType, defualtNull, model));
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId, T model = null)
            where T : class, IDBModelCore
        {
            lock(objectLock.opening_Lock)
            {
                _ModelNotify(notifierType, modelId, model);
            }
        }


        void _ModelNotify<T, TId>(DBModelNotifierType notifierType, TId modelId, T model = null)
            where T : class, IDBModelCore
        {
            _AddNotify_ToTemp(new ModelNotifierArgs<T, TId>(notifierType, modelId, model));
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, Dictionary<TId, T> dic)
           where T : class, IDBModelCore
        {
            lock (objectLock.opening_Lock)
            {
                _ModelNotify(notifierType, dic);
            }
        }
        void _ModelNotify<T, TId>(DBModelNotifierType notifierType, Dictionary<TId,T> dic)
            where T : class, IDBModelCore
        {
            _AddNotify_ToTemp(dic.Select(m => new ModelNotifierArgs<T, TId>(notifierType, m.Key, m.Value)).ToArray());
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, T[] models, TId defualtNull)
            where T : class, IDBModelCore
        {
            lock (objectLock.opening_Lock)
            {
                _ModelNotify<T, TId>(notifierType, models, defualtNull);
            }
        }
        void _ModelNotify<T, TId>(DBModelNotifierType notifierType, T[] models, TId defualtNull)
            where T : class, IDBModelCore
 
        {
            _AddNotify_ToTemp(models.Select(m => new ModelNotifierArgs<T, TId>(notifierType, defualtNull, m)).ToArray());
        }

        public void ModelNotify<T, TId>(DBModelNotifierType notifierType, TId[] modelIds)
            where T : class, IDBModelCore
 
        {
            lock (objectLock.opening_Lock)
            {
                _ModelNotify<T, TId>(notifierType, modelIds);
            }
        }
        void _ModelNotify<T, TId>(DBModelNotifierType notifierType, TId[] modelIds)
            where T : class, IDBModelCore
 
        {
            _AddNotify_ToTemp(modelIds.Select(m => new ModelNotifierArgs<T, TId>(notifierType, m, null)).ToArray());
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
