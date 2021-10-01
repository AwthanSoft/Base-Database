using Mawa.DBCore.ViewEntityCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.DBCore.NotifierCore.ViewEntity
{
    abstract class ViewEntityNotifierArgsCore<T>: EventArgs
        where T : ViewEntityCore.ModelViewEntityCore
    {
        //public readonly Type entityType = typeof(T);
        readonly public T entity;
        public readonly DBModelNotifierType notifierType;
        public ViewEntityNotifierArgsCore(T entity, DBModelNotifierType notifierType)
        {
            this.notifierType = notifierType;
            this.entity = entity;
        }
    }


    class InsertViewEntityNotifierArgs<T> : ViewEntityNotifierArgsCore<T>
        where T : ViewEntityCore.ModelViewEntityCore
    {
        //readonly public EntityEntry<T> entityEntry;
        public InsertViewEntityNotifierArgs(T entity) : base(entity, DBModelNotifierType.Insert)
        {

        }
    }
    class RefreshViewEntityNotifierArgs<T> : ViewEntityNotifierArgsCore<T>
        where T : ViewEntityCore.ModelViewEntityCore
    {
        //readonly public EntityEntry<T> entityEntry;
        public RefreshViewEntityNotifierArgs() : base(null, DBModelNotifierType.Refresh)
        {

        }
    }
}
