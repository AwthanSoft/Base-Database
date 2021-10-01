using Mawa.DBCore.ViewEntityCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.DBCore.NotifierCore
{
    abstract class ModelNotifierArgsCore<T>: EventArgs
        where T : DBModelCore
    {
        //public readonly Type entityType = typeof(T);
        readonly public T model;
        public readonly DBModelNotifierType notifierType;
        public ModelNotifierArgsCore(T entity, DBModelNotifierType notifierType)
        {
            this.notifierType = notifierType;
            this.model = entity;
        }
    }


    class InsertModelNotifierArgs<T> : ModelNotifierArgsCore<T>
        where T : DBModelCore
    {
        //readonly public EntityEntry<T> entityEntry;
        public InsertModelNotifierArgs(T entity) : base(entity, DBModelNotifierType.Insert)
        {

        }
    }
    class UpdateModelNotifierArgs<T> : ModelNotifierArgsCore<T>
        where T : DBModelCore
    {
        //readonly public EntityEntry<T> entityEntry;
        public UpdateModelNotifierArgs(T entity) : base(entity, DBModelNotifierType.Update)
        {

        }
    }

    class DeleteModelNotifierArgs<T> : ModelNotifierArgsCore<T>
        where T : DBModelCore
    {
        //readonly public EntityEntry<T> entityEntry;
        public DeleteModelNotifierArgs(T entity) : base(entity, DBModelNotifierType.Delete)
        {

        }
    }


    class RefreshModelNotifierArgs<T> : ModelNotifierArgsCore<T>
        where T : DBModelCore
    {
        //readonly public EntityEntry<T> entityEntry;
        public RefreshModelNotifierArgs() : base(null, DBModelNotifierType.Refresh)
        {

        }
    }
}
