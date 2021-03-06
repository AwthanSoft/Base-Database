using Mawa.BaseDBCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.DBCore.NotifierCore
{
    internal interface IModelNotifierControl
    {
        void moveList_ToTemp();
        void refreshOperation();
    }
    class ModelNotifierControl<T, TId> : IModelNotifierControl, IDisposable
        where T : class, IDBModelCore
    {
        #region Initail
        //readonly public Type EntityType = typeof(T);
        readonly ModelEventNotifier<T, TId> modelEventNotifier;
        internal ModelEventNotifier<T, TId> ModelEventNotifier => modelEventNotifier;
        public ModelNotifierControl(ModelEventNotifier<T, TId> modelEventNotifier)
        {
            this.modelEventNotifier = modelEventNotifier;
            pre_refresh();
        }
        private void pre_refresh()
        {
            pre_refresh_NotifyList();
            pre_refresh_Operations();
        }


        #endregion

        #region NotifyList

        List<ModelNotifierArgsCore<T, TId>> modelNotifierArgs_list;
        void pre_refresh_NotifyList()
        {
            modelNotifierArgs_list = new List<ModelNotifierArgsCore<T, TId>>();
        }
        internal void AddNotify_ToTemp(ModelNotifierArgsCore<T, TId> modelNotifierArgs)
        {
            modelNotifierArgs_list.Add(modelNotifierArgs);
        }
        internal void AddNotify_ToTemp(ModelNotifierArgsCore<T, TId>[] modelNotifierArgs)
        {
            modelNotifierArgs_list.AddRange(modelNotifierArgs);
        }

        #endregion

        #region Operation
        List<ModelNotifierArgsCore<T, TId>> tempList_modelNotifierArgs;
        void pre_refresh_Operations()
        {
            tempList_modelNotifierArgs = new List<ModelNotifierArgsCore<T, TId>>();
        }

        public void moveList_ToTemp()
        {
            tempList_modelNotifierArgs.AddRange(modelNotifierArgs_list.ToArray());
            modelNotifierArgs_list.Clear();
        }
        public void refreshOperation()
        {
            foreach (var arg in tempList_modelNotifierArgs)
            {
                //as temp : out of the try
                try
                {
                    _DoNotifiyEntity(arg);
                }
                catch(Exception ex)
                {
                    //as temp
                    throw ex;
                }
            }
            tempList_modelNotifierArgs.Clear();
        }


        void _DoNotifiyEntity(ModelNotifierArgsCore<T, TId> modelNotifierArgs)
        {
            modelEventNotifier.Model_Notify(modelNotifierArgs.notifierType, modelNotifierArgs.modelId, modelNotifierArgs.model);

            //switch (modelNotifierArgs.notifierType)
            //{
            //    case DBModelNotifierType.Insert:
            //        {
            //            modelEventNotifier.GetHashCode(modelNotifierArgs.model);
            //            break;
            //        }
            //    case DBModelNotifierType.Update:
            //        {
            //            modelEventNotifier.Model_Update(modelNotifierArgs.model);
            //            break;
            //        }
            //    case DBModelNotifierType.Delete:
            //        {
            //            modelEventNotifier.Model_Remove(modelNotifierArgs.model);
            //            break;
            //        }
            //    case DBModelNotifierType.Refresh:
            //        {
            //            modelEventNotifier.Model_Refresh(modelNotifierArgs.model);
            //            break;
            //        }
            //    default:
            //        {
            //            throw new Exception();
            //            //break;
            //        }
            //}
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

            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ModelNotifierControl()
        {
            Dispose(false);
        }

        #endregion

    }

}
