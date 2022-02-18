//using Mawa.BaseDBCore.EntityCore;
//using System;

//namespace Mawa.DBCore.NotifierCore.Entity
//{
//    abstract class EntityNotifierArgsCore<T, TId> : EventArgs
//        where T : class, IModelEntityCore
//    {
//        public readonly T entity;
//        public readonly TId ModelId;
//        public readonly DBModelNotifierType notifierType;
//        public EntityNotifierArgsCore(DBModelNotifierType notifierType, TId ModelId, T entity = null)
//        {
//            this.notifierType = notifierType;
//            this.ModelId = ModelId;
//            this.entity = entity;
//        }
//    }

//    class GeneralEntityNotifierArgs<T, TId> : EntityNotifierArgsCore<T, TId>
//        where T : class, IModelEntityCore
//    {
//        public GeneralEntityNotifierArgs(TId ModelId, T entity = null) : base(DBModelNotifierType.Insert, ModelId, entity)
//        {

//        }
//    }

//    //class UpdateEntityNotifierArgs<T> : EntityNotifierArgsCore<T>
//    //    where T : class, IModelEntityCore
//    //{
//    //    public UpdateEntityNotifierArgs(T entity) : base(entity, DBModelNotifierType.Update)
//    //    {

//    //    }
//    //}


//    //class InsertEntityNotifierArgs<T, TId> : EntityNotifierArgsCore<T, TId>
//    //    where T : class, IModelEntityCore
//    //{
//    //    //readonly public EntityEntry<T> entityEntry;
//    //    public InsertEntityNotifierArgs(T entity) : base(entity, DBModelNotifierType.Insert)
//    //    {

//    //    }
//    //}
//    //class UpdateEntityNotifierArgs<T> : EntityNotifierArgsCore<T>
//    //    where T : class, IModelEntityCore
//    //{
//    //    public UpdateEntityNotifierArgs(T entity) : base(entity, DBModelNotifierType.Update)
//    //    {

//    //    }
//    //}
//    //class DeleteEntityNotifierArgs<T,TId> : EntityNotifierArgsCore<T>
//    //    where T : class, IModelEntityCore
//    //{

//    //    public DeleteEntityNotifierArgs(TId ModelId, T entity = null) : base(entity, DBModelNotifierType.Delete)
//    //    {
//    //        this.ModelId = ModelId;
//    //    }
//    //}
//}
