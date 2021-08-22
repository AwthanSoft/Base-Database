﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DBAppCore.NotifierCore;

//using System.Data.Entity.Migrations;

namespace DBAppCore
{
    public class ModelDBController<T> 
        where T: DBModelCore
    {
        #region For Internal Extension
        internal DbSet<T> db_Model_Ex => db_Model;
        internal void open_lock_Ex()
        {
            open_lock();
        }
        internal void close_lock_Ex()
        {
            close_lock();
        }

        #endregion

        #region Singleton


        protected readonly DBManagersControlCore dBManagerCore;
        protected DbContextCore dbContextCore => dBManagerCore.dbContextCore;
        protected DbContext db { get { return dBManagerCore.db; } }
        protected virtual DbSet<T> db_Model { get { return dbContextCore.db_Model(typeof(T)) as DbSet<T>; } }
        
        public ModelDBController(DBManagersControlCore dBManager)
        {
            this.dBManagerCore = dBManager;
            modelEventNotifier = new ModelEventNotifier<T>();
            pre_refresh();
        }
        private void pre_refresh()
        {
            dBManagerCore.modelNotifierControlsManager.AddNotifier<T>(modelEventNotifier);
        }

        #endregion

        #region For Openning
        //public CommonAppCore.Locks.ObjectLock DBLock => dBManagerCore.DBLock;
        public object DBOpeningLock => dBManagerCore.DBOpeningLock;
        protected void open_lock()
        {
            dBManagerCore.open_Lock();
        }
        protected void close_lock()
        {
            dBManagerCore.close_Lock();
        }

       

        #endregion



        #region Get

        //All
        public T[] get_Models_All()
        {
            open_lock();
            var tempList = _get_Models_All();
            close_lock();
            return tempList;
        }
        protected T[] _get_Models_All()
        {
            return db_Model.ToArray();
            //var obj_list = from c in db_Model
            //               select c;

            //var tempList = obj_list.ToList();
            //return obj_list.ToArray();
        }
        public T[] get_Models_All_trans()
        {
            return _get_Models_All();
        }
        
        //Properties
        public virtual T Get_Model_By_Properties(params object[] keyValues)
        {
            open_lock();
            T temp_model = _Get_Model_By_Properties(keyValues);
            close_lock();
            return temp_model;
        }
        public T Get_Model_By_Properties_trans(params object[] keyValues)
        {
            return _Get_Model_By_Properties(keyValues);
        }
        protected T _Get_Model_By_Properties(params object[] keyValues)
        {
            T temp_model = db_Model.Find(keyValues);
            return temp_model;
        }



        #endregion

        #region Get Where
        //Where
        public virtual T Get_Model_Where(Func<T, bool> predicate)
        {
            open_lock();
            T temp_model = _Get_Model_Where(predicate);
            close_lock();
            return temp_model;
        }
        public T Get_Model_Where_trans(Func<T, bool> predicate)
        {
            return _Get_Model_Where(predicate);
        }
        protected T _Get_Model_Where(Func<T, bool> predicate)
        {
            var temp_model = db_Model.Where<T>(predicate).FirstOrDefault();
            return temp_model;
        }


        public virtual T[] Get_Models_Where(Func<T, bool> predicate)
        {
            open_lock();
            T[] temp_model = _Get_Models_Where(predicate);
            close_lock();
            return temp_model;
        }
        public T[] Get_Models_Where_trans(Func<T, bool> predicate)
        {
            return _Get_Models_Where(predicate);
        }
        private T[] _Get_Models_Where(Func<T, bool> predicate)
        {
            var temp_model = db_Model.Where<T>(predicate).ToArray();
            return temp_model;
        }

        #endregion


        #region Select
        ////Where
        //public TResult Select_Model_Attributes<TResult>(Func<T, TResult> selector)
        //{
        //    open_lock();
        //    TResult temp_model = _Select_Model_Attributes(selector);
        //    close_lock();
        //    return temp_model;
        //}
        //public TResult Select_Model_Attributes_trans<TResult>(Func<T, TResult> selector)
        //{
        //    return _Select_Model_Attributes(selector);
        //}
        //protected TResult _Select_Model_Attributes<TResult>(Func<T, TResult> selector)
        //{
        //    var temp_model = db_Model.Select<T>(predicate).FirstOrDefault();
        //    return temp_model;
        //}


        //public virtual T[] Get_Models_Where(Func<T, bool> predicate)
        //{
        //    open_lock();
        //    T[] temp_model = _Get_Models_Where(predicate);
        //    close_lock();
        //    return temp_model;
        //}
        //public T[] Get_Models_Where_trans(Func<T, bool> predicate)
        //{
        //    return _Get_Models_Where(predicate);
        //}
        //private T[] _Get_Models_Where(Func<T, bool> predicate)
        //{
        //    var temp_model = db_Model.Where<T>(predicate).ToArray();
        //    return temp_model;
        //}

        #endregion




        #region Methods Add

        public EntityEntry<T> Add_Model<TEntity>(TEntity new_model)
            where TEntity : EntityCore.ModelEntityCore
        {
            open_lock();
            EntityEntry<T> temp_resultt = _Add_Model(new_model);
            close_lock();
            return temp_resultt;
        }
        public EntityEntry<T> Add_Model_trans<TEntity>(TEntity model)
            where TEntity : EntityCore.ModelEntityCore
        {
            return _Add_Model(model);
        }
        protected EntityEntry<T> _Add_Model<TEntity>(TEntity new_model)
            where TEntity : EntityCore.ModelEntityCore
        {
            var temp_resultt = db_Model.Add(new_model as T);

            if (temp_resultt.State == EntityState.Added)
            {
                db.SaveChanges();
                dBManagerCore.modelNotifierControlsManager.InsertNotiFy<T>(temp_resultt.Entity);
            }
            else
            {
                throw new Exception();
            }
            return temp_resultt;
        }

        //AddOrGet
        public EntityEntry<T> AddOrGet(T new_model)
        {
            open_lock();
            EntityEntry<T> temp_resultt = _AddOrGet(new_model);
            close_lock();
            return temp_resultt;
        }
        protected EntityEntry<T> _AddOrGet(T new_model)
        {
            var temp_resultt = db_Model.AddOrGet(new_model);

            if (temp_resultt.State == EntityState.Added)
            {
                db.SaveChanges();
                dBManagerCore.modelNotifierControlsManager.InsertNotiFy<T>(temp_resultt.Entity);
            }
            return temp_resultt;
        }
        public EntityEntry<T> AddOrGet_trans(T new_model)
        {
            return _AddOrGet(new_model);
        }

        //Add Range
        public T[] AddRange_Model(T[] new_models)
        {
            open_lock();
            T[] temp_resultt = _AddRange_Model(new_models);
            close_lock();
            return temp_resultt;
        }
        public T[] AddRange_Model_trans(T[] new_models)
        {
            return _AddRange_Model(new_models);
        }
        protected T[] _AddRange_Model(T[] new_models)
        {

            db_Model.AddRange(new_models);
            int resultt_num = db.SaveChanges();
            if (resultt_num > 0)
            {
                dBManagerCore.modelNotifierControlsManager.InsertNotiFy<T>(new_models);
                return new_models;
            }
            else
            {
                throw new Exception();
            }
        }

        //AddOrUpdate
        public EntityEntry<T> AddOrUpdate(T new_model)
        {
            open_lock();
            EntityEntry<T> temp_resultt = _AddOrUpdate(new_model);
            close_lock();
            return temp_resultt;
        }
        protected EntityEntry<T> _AddOrUpdate(T model)
           // where TEntity : EntityCore.ModelEntityCore
        {
            //db.Entry(model).State = EntityState.Detached;
            //db.ChangeTracker.AcceptAllChanges();

            var temp_resultt = db_Model.AddOrUpdate(model);
            
            switch (temp_resultt.State)
            {
                case EntityState.Added:
                    {
                        db.SaveChanges();
                        dBManagerCore.modelNotifierControlsManager.InsertNotiFy<T>(temp_resultt.Entity);
                        break;
                    }
                case EntityState.Modified:
                    {
                        try
                        {
                            db.SaveChanges();
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }

                        dBManagerCore.modelNotifierControlsManager.UpdateNotiFy<T>(temp_resultt.Entity);
                        break;
                    }
                default:
                    {
                        throw new Exception();
                    }
            }
            
            return temp_resultt;
        }
        public EntityEntry<T> AddOrUpdate_trans(T new_model)
        {
            return _AddOrGet(new_model);
        }


        #endregion

        #region Update Methods
        
        //Update
        public EntityEntry<T> Update_Model<TEntity>(TEntity model)
            where TEntity : EntityCore.ModelEntityCore
        {
            open_lock();
            EntityEntry<T> temp_resultt = _Update_Model(model);
            close_lock();
            return temp_resultt;
        }
        public EntityEntry<T> Update_Model_trans<TEntity>(TEntity model)
            where TEntity : EntityCore.ModelEntityCore
        {
            return _Update_Model(model);
        }
        protected EntityEntry<T> _Update_Model<TEntity>(TEntity model)
            where TEntity : EntityCore.ModelEntityCore
        {
            var temp_resultt = db_Model.Update(model as T);

            if (temp_resultt.State == EntityState.Modified)
            {
                db.SaveChanges();
                dBManagerCore.modelNotifierControlsManager.UpdateNotiFy<T>(temp_resultt.Entity);
            }
            else
            {
                throw new Exception("The entity not exist in db");
            }
            return temp_resultt;
        }


        //UpdateRange
        public EntityEntry<T>[] UpdateRange<TEntity>(TEntity[] models)
            where TEntity : EntityCore.ModelEntityCore
        {
            open_lock();
            var temp_resultt = _UpdateRange(models);
            close_lock();
            return temp_resultt;
        }
        public EntityEntry<T>[] UpdateRange_trans<TEntity>(TEntity[] models)
            where TEntity : EntityCore.ModelEntityCore
        {
            return _UpdateRange(models);
        }
        protected EntityEntry<T>[] _UpdateRange<TEntity>(TEntity[] models)
            where TEntity : EntityCore.ModelEntityCore
        {
            var temp_List = new List<EntityEntry<T>>();
            foreach(var model in models)
            {
                var temp_resultt = db_Model.Update(model as T);
                if (temp_resultt.State != EntityState.Modified)
                    throw new Exception();
                temp_List.Add(temp_resultt);
            }
            int save_resultt = db.SaveChanges();
            if(save_resultt > 0)
            {
                foreach (var entry in temp_List)
                {
                    dBManagerCore.modelNotifierControlsManager.UpdateNotiFy<T>(entry.Entity);
                }
                return temp_List.ToArray();
            }
            else
            {
                //throw new Exception();
            }
            return temp_List.ToArray();
        }

        #endregion

        #region Update Methods

        //Remove
        public EntityEntry<T> Remove_Model<TEntity>(TEntity model)
            where TEntity : EntityCore.ModelEntityCore
        {
            open_lock();
            EntityEntry<T> temp_resultt = _Remove_Model(model);
            close_lock();
            return temp_resultt;
        }
        public EntityEntry<T> Remove_Model_trans<TEntity>(TEntity model)
            where TEntity : EntityCore.ModelEntityCore
        {
            return _Remove_Model(model);
        }
        protected EntityEntry<T> _Remove_Model<TEntity>(TEntity model)
            where TEntity : EntityCore.ModelEntityCore
        {
            var temp_resultt = db_Model.Remove(model as T);

            if (temp_resultt.State == EntityState.Deleted)
            {
                db.SaveChanges();
                dBManagerCore.modelNotifierControlsManager.DeleteNotiFy<T>(temp_resultt.Entity);
            }
            else
            {
                throw new Exception("The entity not exist in db");
            }
            return temp_resultt;
        }


        //RemoveRange
        public EntityEntry<T>[] RemoveRange<TEntity>(TEntity[] models)
            where TEntity : EntityCore.ModelEntityCore
        {
            open_lock();
            var temp_resultt = _RemoveRange(models);
            close_lock();
            return temp_resultt;
        }
        public EntityEntry<T>[] RemoveRange_trans<TEntity>(TEntity[] models)
            where TEntity : EntityCore.ModelEntityCore
        {
            return _RemoveRange(models);
        }
        protected EntityEntry<T>[] _RemoveRange<TEntity>(TEntity[] models)
            where TEntity : EntityCore.ModelEntityCore
        {
            var temp_List = new List<EntityEntry<T>>();
            foreach (var model in models)
            {
                var temp_resultt = db_Model.Remove(model as T);
                if (temp_resultt.State != EntityState.Deleted)
                    throw new Exception();
                temp_List.Add(temp_resultt);
            }
            int save_resultt = db.SaveChanges();
            if (save_resultt > 0)
            {
                dBManagerCore.modelNotifierControlsManager.DeleteNotiFy<T>(
                    temp_List.Select<EntityEntry<T>,T>(b=>b.Entity).ToArray());
                return temp_List.ToArray();
            }
            else
            {
                if(save_resultt == 0)
                {
                    // may there already no model in db
                    return temp_List.ToArray();
                }
                else
                {
                    // as temp
                    throw new Exception();
                }
            }
        }

        #endregion









        #region For Load Model
        public void LoadModelProperty(T model ,params string[] propertyNames)
        {
            open_lock();
            _LoadModelProperty(model, propertyNames);
            close_lock();
        }
        public void _LoadModelProperty_trans(T model, params string[] propertyNames)
        {
            _LoadModelProperty(model, propertyNames);
        }
        protected void _LoadModelProperty(T model, params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                //db.Entry(model).Reference(b => b.PDFEntity).Load();
                db.Entry(model).Reference(name).Load();
            }
        }



        #endregion

        #region For Searching Methods 


        #endregion

        #region For Events

        public readonly ModelEventNotifier<T> modelEventNotifier;

        #endregion
    }




}