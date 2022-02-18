using System;
using Mawa.BaseDBCore;

namespace Mawa.DBCore.NotifierCore
{
    abstract class ModelNotifierArgsCore<T, TId> : EventArgs
        where T : class,IDBModelCore
        where TId : struct
    {
        readonly public T model;
        readonly public TId modelId;
        public readonly DBModelNotifierType notifierType;
        public ModelNotifierArgsCore(DBModelNotifierType notifierType, TId? modelId = null, T model = null)
        {
            this.notifierType = notifierType;
            this.model = model;
            if(modelId != null)
            {
                this.modelId = modelId.Value;
            }
        }
    }
    class ModelNotifierArgs<T, TId> : ModelNotifierArgsCore<T, TId>
        where T : class, IDBModelCore
        where TId : struct
    {
        public ModelNotifierArgs(DBModelNotifierType notifierType, TId? modelId = null, T model = null) : base(notifierType, modelId, model)
        {

        }
    }


    //class InsertModelNotifierArgs<T> : ModelNotifierArgsCore<T>
    //    where T : class, IDBModelCore
    //{
    //    //readonly public EntityEntry<T> entityEntry;
    //    public InsertModelNotifierArgs(T entity) : base(entity, DBModelNotifierType.Insert)
    //    {

    //    }
    //}
    //class UpdateModelNotifierArgs<T> : ModelNotifierArgsCore<T>
    //    where T : class, IDBModelCore
    //{
    //    //readonly public EntityEntry<T> entityEntry;
    //    public UpdateModelNotifierArgs(T entity) : base(entity, DBModelNotifierType.Update)
    //    {

    //    }
    //}

    //class DeleteModelNotifierArgs<T> : ModelNotifierArgsCore<T>
    //    where T : class, IDBModelCore
    //{
    //    //readonly public EntityEntry<T> entityEntry;
    //    public readonly object ModelId;

    //    public DeleteModelNotifierArgs(object ModelId, T entity = null) : base(entity, DBModelNotifierType.Delete)
    //    {
    //        this.ModelId = ModelId;
    //    }
    //}


    //class RefreshModelNotifierArgs<T> : ModelNotifierArgsCore<T>
    //    where T : class, IDBModelCore
    //{
    //    //readonly public EntityEntry<T> entityEntry;
    //    public RefreshModelNotifierArgs() : base(null, DBModelNotifierType.Refresh)
    //    {

    //    }
    //}
}
