//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
//using System.Threading.Tasks;
//using Mawa.BaseDBCore.EntityCore;

//namespace Mawa.DBCore.NotifierCore.Entity
//{
//    internal interface IEntityNotifierControl
//    {
//        void moveList_ToTemp();
//        void refreshOperation();
//    }
//    class EntityNotifierControl<T, TId> : IEntityNotifierControl, IDisposable
//        where T : class, IModelEntityCore
//    {
//        #region Initail
//        //readonly public Type EntityType = typeof(T);
//        readonly EntityEventNotifier<T, TId> entityEventNotifier;
//        public EntityNotifierControl(EntityEventNotifier<T, TId> entityEventNotifier)
//        {
//            this.entityEventNotifier = entityEventNotifier;
//            pre_refresh();
//        }
//        private void pre_refresh()
//        {
//            pre_refresh_NotifyList();
//            pre_refresh_Operations();
//        }


//        #endregion

//        #region NotifyList

//        List<EntityNotifierArgsCore<T, TId>> entityNotifierArgs_list;
//        void pre_refresh_NotifyList()
//        {
//            entityNotifierArgs_list = new List<EntityNotifierArgsCore<T, TId>>();
//        }
//        internal void AddNotify_ToTemp(EntityNotifierArgsCore<T, TId> entityNotifierArgs)
//        {
//            entityNotifierArgs_list.Add(entityNotifierArgs);
//        }
//        internal void AddNotify_ToTemp(EntityNotifierArgsCore<T, TId>[] entityNotifierArgs)
//        {
//            entityNotifierArgs_list.AddRange(entityNotifierArgs);
//        }

//        #endregion

//        #region Operation
//        List<EntityNotifierArgsCore<T, TId>> tempList_entityNotifierArgs;
//        void pre_refresh_Operations()
//        {
//            tempList_entityNotifierArgs = new List<EntityNotifierArgsCore<T, TId>>();
//        }

//        public void moveList_ToTemp()
//        {
//            tempList_entityNotifierArgs.AddRange(entityNotifierArgs_list.ToArray());
//            entityNotifierArgs_list.Clear();
//        }
//        public void refreshOperation()
//        {
//            foreach (var arg in tempList_entityNotifierArgs)
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
//            tempList_entityNotifierArgs.Clear();
//        }


//        void _DoNotifiyEntity(EntityNotifierArgsCore<T, TId> entityNotifierArgs)
//        {
//            switch (entityNotifierArgs.notifierType)
//            {
//                case DBModelNotifierType.Insert:
//                    {
//                        entityEventNotifier.Model_Add(entityNotifierArgs.entity);
//                        break;
//                    }
//                case DBModelNotifierType.Update:
//                    {
//                        entityEventNotifier.Model_Update(entityNotifierArgs.entity);
//                        break;
//                    }
//                case DBModelNotifierType.Delete:
//                    {
//                        entityEventNotifier.OnModel_Remove(entityNotifierArgs.entity);
//                        break;
//                    }
//                default:
//                    {
//                        throw new NotSupportedException();
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

//        ~EntityNotifierControl()
//        {
//            Dispose(false);
//        }

//        #endregion

//    }
//}
