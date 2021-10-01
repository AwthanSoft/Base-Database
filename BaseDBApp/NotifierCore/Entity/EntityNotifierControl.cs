using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;
using Mawa.DBCore.EntityCore;

namespace Mawa.DBCore.NotifierCore.Entity
{
    internal interface IEntityNotifierControl
    {
        void moveList_ToTemp();
        void refreshOperation();
    }
    class EntityNotifierControl<T> : IEntityNotifierControl, IDisposable
        where T : ModelEntityCore
    {
        #region Initail
        //readonly public Type EntityType = typeof(T);
        readonly EntityEventNotifier<T> entityEventNotifier;
        public EntityNotifierControl(EntityEventNotifier<T> entityEventNotifier)
        {
            this.entityEventNotifier = entityEventNotifier;
            pre_refresh();
        }
        private void pre_refresh()
        {
            pre_refresh_NotifyList();
            pre_refresh_Operations();
        }


        #endregion

        #region NotifyList

        List<EntityNotifierArgsCore<T>> entityNotifierArgs_list;
        void pre_refresh_NotifyList()
        {
            entityNotifierArgs_list = new List<EntityNotifierArgsCore<T>>();
        }
        internal void AddNotify_ToTemp(EntityNotifierArgsCore<T> entityNotifierArgs)
        {
            entityNotifierArgs_list.Add(entityNotifierArgs);
        }
        internal void AddNotify_ToTemp(EntityNotifierArgsCore<T>[] entityNotifierArgs)
        {
            entityNotifierArgs_list.AddRange(entityNotifierArgs);
        }

        #endregion

        #region Operation
        List<EntityNotifierArgsCore<T>> tempList_entityNotifierArgs;
        void pre_refresh_Operations()
        {
            tempList_entityNotifierArgs = new List<EntityNotifierArgsCore<T>>();
        }

        public void moveList_ToTemp()
        {
            tempList_entityNotifierArgs.AddRange(entityNotifierArgs_list.ToArray());
            entityNotifierArgs_list.Clear();
        }
        public void refreshOperation()
        {
            foreach (var arg in tempList_entityNotifierArgs)
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
            tempList_entityNotifierArgs.Clear();
        }


        void _DoNotifiyEntity(EntityNotifierArgsCore<T> entityNotifierArgs)
        {
            switch (entityNotifierArgs.notifierType)
            {
                case DBModelNotifierType.Insert:
                    {
                        entityEventNotifier.Model_Add(entityNotifierArgs.entity);
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

        ~EntityNotifierControl()
        {
            Dispose(false);
        }

        #endregion

    }
}
