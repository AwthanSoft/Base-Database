using Mawa.DBCore.EntityCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.DBCore.NotifierCore.Entity
{
    abstract class EntityNotifierArgsCore<T>: EventArgs
        where T : ModelEntityCore
    {
        //public readonly Type entityType = typeof(T);
        readonly public T entity;
        public readonly DBModelNotifierType notifierType;
        /*public EntityNotifierArgsCore(EntityEntry<T> entityEntry , EntityNotifierType notifierType)
        {
            this.notifierType = notifierType;
        }*/
        public EntityNotifierArgsCore(T entity, DBModelNotifierType notifierType)
        {
            this.notifierType = notifierType;
            this.entity = entity;
        }
    }


    class InsertEntityNotifierArgs<T> : EntityNotifierArgsCore<T>
        where T : ModelEntityCore
    {
        //readonly public EntityEntry<T> entityEntry;
        public InsertEntityNotifierArgs(T entity) : base(entity, DBModelNotifierType.Insert)
        {

        }
    }
}
