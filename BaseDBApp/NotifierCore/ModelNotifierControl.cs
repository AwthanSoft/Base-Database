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
    class ModelNotifierControl<T> : IModelNotifierControl, IDisposable
        where T : DBModelCore
    {
        #region Initail
        //readonly public Type EntityType = typeof(T);
        readonly ModelEventNotifier<T> modelEventNotifier;
        public ModelNotifierControl(ModelEventNotifier<T> modelEventNotifier)
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

        List<ModelNotifierArgsCore<T>> modelNotifierArgs_list;
        void pre_refresh_NotifyList()
        {
            modelNotifierArgs_list = new List<ModelNotifierArgsCore<T>>();
        }
        internal void AddNotify_ToTemp(ModelNotifierArgsCore<T> modelNotifierArgs)
        {
            modelNotifierArgs_list.Add(modelNotifierArgs);
        }
        internal void AddNotify_ToTemp(ModelNotifierArgsCore<T>[] modelNotifierArgs)
        {
            modelNotifierArgs_list.AddRange(modelNotifierArgs);
        }

        #endregion

        #region Operation
        List<ModelNotifierArgsCore<T>> tempList_modelNotifierArgs;
        void pre_refresh_Operations()
        {
            tempList_modelNotifierArgs = new List<ModelNotifierArgsCore<T>>();
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
                _DoNotifiyEntity(arg);
                /*try
                {

                }
                catch
                {

                }*/
            }
            tempList_modelNotifierArgs.Clear();
        }


        void _DoNotifiyEntity(ModelNotifierArgsCore<T> modelNotifierArgs)
        {
            switch (modelNotifierArgs.notifierType)
            {
                case DBModelNotifierType.Insert:
                    {
                        modelEventNotifier.Model_Add(modelNotifierArgs.model);
                        break;
                    }
                case DBModelNotifierType.Update:
                    {
                        modelEventNotifier.Model_Update(modelNotifierArgs.model);
                        break;
                    }
                case DBModelNotifierType.Delete:
                    {
                        modelEventNotifier.Model_Remove(modelNotifierArgs.model);
                        break;
                    }
                case DBModelNotifierType.Refresh:
                    {
                        modelEventNotifier.Model_Refresh(modelNotifierArgs.model);
                        break;
                    }
                default:
                    {
                        throw new Exception();
                        //break;
                    }
            }
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
