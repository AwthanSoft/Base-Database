//using System;
//using System.Collections.Generic;
//using System.Text;
//using Mawa.DBCore.ViewEntityCore;

//namespace Mawa.DBCore.NotifierCore.ViewEntity
//{
//    internal interface IViewEntityNotifierControl
//    {
//        void moveList_ToTemp();
//        void refreshOperation();
//    }
//    class ViewEntityNotifierControl<T> : IViewEntityNotifierControl, IDisposable
//        where T : ViewEntityCore.ModelViewEntityCore
//    {
//        #region Initail
//        //readonly public Type EntityType = typeof(T);
//        readonly ViewEntityEventNotifier<T> viewEntityEventNotifier;
//        public ViewEntityNotifierControl(ViewEntityEventNotifier<T> viewEntityEventNotifier)
//        {
//            this.viewEntityEventNotifier = viewEntityEventNotifier;
//            pre_refresh();
//        }
//        private void pre_refresh()
//        {
//            pre_refresh_NotifyList();
//            pre_refresh_Operations();
//        }


//        #endregion

//        #region NotifyList

//        List<ViewEntityNotifierArgsCore<T>> viewEntityNotifierArgs_list;
//        void pre_refresh_NotifyList()
//        {
//            viewEntityNotifierArgs_list = new List<ViewEntityNotifierArgsCore<T>>();
//        }
//        internal void AddNotify_ToTemp(ViewEntityNotifierArgsCore<T> viewEntityNotifierArgs)
//        {
//            viewEntityNotifierArgs_list.Add(viewEntityNotifierArgs);
//        }
//        internal void AddNotify_ToTemp(ViewEntityNotifierArgsCore<T>[] viewEntityNotifierArgs)
//        {
//            viewEntityNotifierArgs_list.AddRange(viewEntityNotifierArgs);
//        }

//        #endregion

//        #region Operation
//        List<ViewEntityNotifierArgsCore<T>> tempList_viewEntityNotifierArgs;
//        void pre_refresh_Operations()
//        {
//            tempList_viewEntityNotifierArgs = new List<ViewEntityNotifierArgsCore<T>>();
//        }

//        public void moveList_ToTemp()
//        {
//            tempList_viewEntityNotifierArgs.AddRange(viewEntityNotifierArgs_list.ToArray());
//            viewEntityNotifierArgs_list.Clear();
//        }
//        public void refreshOperation()
//        {
//            foreach (var arg in tempList_viewEntityNotifierArgs)
//            {
//                //as temp : out of the try
//                _DoNotifiyEntity(arg);
//                /*try
//                {

//                }
//                catch
//                {

//                }*/
//            }
//            tempList_viewEntityNotifierArgs.Clear();
//        }


//        void _DoNotifiyEntity(ViewEntityNotifierArgsCore<T> viewEntityNotifierArgs)
//        {
//            switch (viewEntityNotifierArgs.notifierType)
//            {
//                case DBModelNotifierType.Insert:
//                    {
//                        viewEntityEventNotifier.Model_Add(viewEntityNotifierArgs.entity);
//                        break;
//                    }
//                case DBModelNotifierType.Refresh:
//                    {
//                        viewEntityEventNotifier.Model_Refresh(viewEntityNotifierArgs.entity);
//                        break;
//                    }
//                default:
//                    {
//                        throw new Exception();
//                        //break;
//                    }
//            }
//        }

//        #endregion

//        #region Dispose

//        private bool _disposed = false;
//        protected virtual void Dispose(bool disposing)
//        {
//            if (_disposed)
//                return;

//            if (disposing)
//            {
//                // Free any other managed objects here.
//            }

//            // Free any unmanaged objects here.

//            _disposed = true;
//        }
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        ~ViewEntityNotifierControl()
//        {
//            Dispose(false);
//        }

//        #endregion

//    }

//}
