using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mawa.DBCore.NotifierCore;
using Mawa.BaseDBCore;
using Mawa.Lock;
using Mawa.BaseDBCore.EntityCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

//using System.Data.Entity.Migrations;

namespace Mawa.DBCore
{
    public interface IModelDBController<T>
        where T : class, IDBModelCore
    {
        void open_lock_Ex();
        void close_lock_Ex();
        DbSet<T> db_Model_Ex { get; }
    }

    public class ModelDBController<T, TId> : IModelDBController<T>
        where T: class ,IDBModelCore

    {
        #region For Internal Extension
       

        #endregion

        #region Singleton

        protected ObjectLock dbLocker => dBManagerCore.dbLocker;

        protected readonly DBManagersControlCore dBManagerCore;
        protected DbContext db { get { return dBManagerCore.db; } }
        protected virtual DbSet<T> db_Model => db.Set<T>();
        public DbSet<T> db_Model_Ex => db_Model;

        protected readonly TId defaultNull;

        public ModelDBController(DBManagersControlCore dBManager, TId defualtNull)
        {
            this.defaultNull = defualtNull;
            this.dBManagerCore = dBManager;
            modelEventNotifier = new ModelEventNotifier<T, TId>();
            pre_refresh();
        }
        private void pre_refresh()
        {
            dBManagerCore.modelNotifierControlsManager.AddNotifier<T, TId>(modelEventNotifier);
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
            lock (dBManagerCore.DBOpeningLock)
            {
                return _get_Models_All();
            }
        }
        protected T[] _get_Models_All()
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().ToArray();
            }
        }
        public T[] get_Models_All_trans()
        {
            return db_Model.ToArray();
        }

        //Properties
        //public virtual T Get_Model_By_Properties(params object[] keyValues)
        //{
        //    open_lock();
        //    T temp_model = _Get_Model_By_Properties(keyValues);
        //    close_lock();
        //    return temp_model;
        //}
        //public T Get_Model_By_Properties_trans(params object[] keyValues)
        //{
        //    return _Get_Model_By_Properties(keyValues);
        //}
        //protected T _Get_Model_By_Properties(params object[] keyValues)
        //{
        //    T temp_model = db_Model.Find(keyValues);
        //    return temp_model;
        //}


        #endregion

        #region Get Where

        //Where
        public virtual T Get_Model_Where(Func<T, bool> predicate)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Get_Model_Where(predicate);
            }
        }
        public T Get_Model_Where_trans(Func<T, bool> predicate)
        {
            return db_Model.Where(predicate).FirstOrDefault();
        }
        protected T _Get_Model_Where(Func<T, bool> predicate)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Where(predicate).FirstOrDefault();
            }
        }

        //
        public virtual T[] Get_Models_Where(Func<T, bool> predicate)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Get_Models_Where(predicate);
            }
        }
        public T[] Get_Models_Where_trans(Func<T, bool> predicate)
        {
            return db_Model.Where(predicate).ToArray();
        }
        private T[] _Get_Models_Where(Func<T, bool> predicate)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Where(predicate).ToArray();
            }
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


        #region yield

        public IEnumerable<T> Get_IEnumerable()
        {
            open_lock();
            var temp_model = _Get_IEnumerable();
            close_lock();
            return temp_model;
        }
        private IEnumerable<T> _Get_IEnumerable()
        {
            foreach (var item in db_Model)
            {
                yield return item;
            }
        }

        #endregion

        #region Custom Action

        public TResult Get_DbSet_predicate<TResult>(Func<DbSet<T>, TResult> predicate)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Get_DbSet_predicate(predicate);
            }
        }
        private TResult _Get_DbSet_predicate<TResult>(Func<DbSet<T>, TResult> predicate)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return predicate(db.Set<T>());
            }
        }

        #endregion

        #region Count

        public int Count()
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Count();
            }
        }
        protected int _Count()
        {
            using(var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Count();
            }
        }
        protected int _Count_trans()
        {
            return db_Model.Count();
        }

        #endregion



        #region Struct Q

        //Select-Min
        public TResult Q_Select_Min<TResult>(Func<T, TResult> selector)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Q_Select_Min(selector);
            }
        }
        public TResult Q_Select_Min_trans<TResult>(Func<T, TResult> selector)
        {
            return db_Model.Select(selector).Min();
        }
        private TResult _Q_Select_Min<TResult>(Func<T, TResult> selector)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Select(selector).Min();
            }
        }


        //Select-Max
        public TResult Q_Select_Max<TResult>(Func<T, TResult> selector)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Q_Select_Max(selector);
            }
        }
        public TResult Q_Select_Max_trans<TResult>(Func<T, TResult> selector)
        {
            return db_Model.Select(selector).Max();
        }
        private TResult _Q_Select_Max<TResult>(Func<T, TResult> selector)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Select(selector).Max();
            }
        }


        //QWhere-Select-Min
        public TResult Q_QWhere_Select_Min<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Q_QWhere_Select_Min(predicate, selector);
            }
        }
        public TResult Q_QWhere_Select_Min_trans<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            return db_Model.Where(predicate).Select(selector).Min();
        }
        private TResult _Q_QWhere_Select_Min<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Where(predicate).Select(selector).Min();
            }
        }

        //QWhere-Select-Max
        public TResult Q_QWhere_Select_Max<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Q_QWhere_Select_Max(predicate, selector);
            }
        }
        public TResult Q_QWhere_Select_Max_trans<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            return db_Model.Where(predicate).Select(selector).Max();
        }
        private TResult _Q_QWhere_Select_Max<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Where(predicate).Select(selector).Max();
            }
        }

        //QWhere-Count
        //public int Q_QWhere_Count(Expression<Func<T, bool>> predicate)
        //{
        //    lock (dBManagerCore.DBOpeningLock)
        //    {
        //        return _Q_QWhere_Count(predicate);
        //    }
        //}
        //public int Q_QWhere_Count_trans(Expression<Func<T, bool>> predicate)
        //{
        //    return db_Model.Where(predicate).Count();
        //}
        //private int _Q_QWhere_Count(Expression<Func<T, bool>> predicate)
        //{
        //    using (var db = dBManagerCore.GetNew_dbContext())
        //    {
        //        return db.Set<T>().Where(predicate).Count();
        //    }
        //}

        //QWhere-Select-ToArray
        public TResult[] Q_QWhere_Select_ToArray<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            open_lock();
            var temp_model = _Q_QWhere_Select_ToArray(predicate, selector);
            close_lock();
            return temp_model;
        }
        public TResult[] Q_QWhere_Select_ToArray_trans<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            return _Q_QWhere_Select_ToArray(predicate, selector);
        }
        private TResult[] _Q_QWhere_Select_ToArray<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            return db_Model
                .Where(predicate)
                .Select(selector)
                .ToArray();
        }

        //QWhere-Select-Distinct-ToArray
        public TResult[] Q_QWhere_Select_Distinct_ToArray<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Q_QWhere_Select_Distinct_ToArray(predicate, selector);
            }
        }
        public TResult[] Q_QWhere_Select_Distinct_ToArray_trans<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            return db_Model.Where(predicate).Select(selector).Distinct().ToArray();
        }
        private TResult[] _Q_QWhere_Select_Distinct_ToArray<TResult>(Expression<Func<T, bool>> predicate, Func<T, TResult> selector)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Where(predicate).Select(selector).Distinct().ToArray();
            }
        }

        //QWhere-Take-ToArray
        public T[] Q_Where_Take_ToArray(Func<T, bool> predicate, int takeCount)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Q_Where_Take_ToArray(predicate, takeCount);
            }
        }
        public T[] Q_Where_Take_ToArray_trans(Func<T, bool> predicate, int takeCount)
        {
            return db_Model.Where(predicate).Take(takeCount).ToArray();
        }
        private T[] _Q_Where_Take_ToArray(Func<T, bool> predicate, int takeCount)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                return db.Set<T>().Where(predicate).Take(takeCount).ToArray();
            }
        }

        #endregion

        #region Advance Struct Q

        //QWhere-First
        public T Q_QWhere_First(Expression<Func<T, bool>> predicate, bool OpeningLock = false)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                using (var dbContext = dBManagerCore.GetNew_dbContext())
                {
                    return dbContext.Set<T>().Where(predicate).First();
                }
            }
        }
        public async Task<T> Q_QWhere_FirstAsync(Expression<Func<T, bool>> predicate, bool OpeningLock = false)
        {
            if(OpeningLock)
            {
                open_lock();
                var resultt = await db_Model.Where(predicate).FirstAsync();
                close_lock();
                return resultt;
            }
            else
            {
                using (var dbContext = dBManagerCore.GetNew_dbContext())
                {
                    return await dbContext.Set<T>().Where(predicate).FirstAsync();
                }
            }
        }
        public T Q_QWhere_First_trans(Expression<Func<T, bool>> predicate)
        {
            return db_Model.Where(predicate).First();
        }
        public async Task<T> Q_QWhere_First_transAsync(Expression<Func<T, bool>> predicate)
        {
            return await db_Model.Where(predicate).FirstAsync();
        }

        //FirstOrDefault
        public T Q_FirstOrDefault(Expression<Func<T, bool>> predicate, bool OpeningLock = false)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                using (var dbContext = dBManagerCore.GetNew_dbContext())
                {
                    if(predicate != null)
                        return dbContext.Set<T>().FirstOrDefault(predicate);
                    else
                        return dbContext.Set<T>().FirstOrDefault();
                }
            }
        }
        public async Task<T> Q_FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool OpeningLock = false)
        {
            T resultt = null;
            if (OpeningLock)
            {
                open_lock();
                try
                {
                    if (predicate != null)
                        resultt = await db_Model.FirstOrDefaultAsync(predicate);
                    else
                        resultt = await db_Model.FirstOrDefaultAsync();
                }
                catch (Exception)
                {
                    close_lock();
                    throw;
                }
                close_lock();
                return resultt;
            }
            else
            {
                using (var dbContext = dBManagerCore.GetNew_dbContext())
                {
                    if (predicate != null)
                        resultt = await dbContext.Set<T>().FirstOrDefaultAsync(predicate);
                    else
                        resultt = await dbContext.Set<T>().FirstOrDefaultAsync();
                }
            }

            return resultt;
        }
        public T Q_FirstOrDefault_trans(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
                return db_Model.FirstOrDefault(predicate);
            else
                return db_Model.FirstOrDefault();
        }
        public Task<T> Q_FirstOrDefault_transAsync(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
                return db_Model.FirstOrDefaultAsync(predicate);
            else
                return db_Model.FirstOrDefaultAsync();
        }

        //QWhere-Count
        public int Q_QWhere_Count(Expression<Func<T, bool>> predicate, bool OpeningLock = false)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                using (var dbContext = dBManagerCore.GetNew_dbContext())
                {
                    return dbContext.Set<T>().Where(predicate).Count();
                }
            }
        }
        public async Task<int> Q_QWhere_CountAsync(Expression<Func<T, bool>> predicate, bool OpeningLock = false)
        {
            if (OpeningLock)
            {
                open_lock();
                var resultt = await db_Model.Where(predicate).CountAsync();
                close_lock();
                return resultt;
            }
            else
            {
                using (var dbContext = dBManagerCore.GetNew_dbContext())
                {
                    return await dbContext.Set<T>().Where(predicate).CountAsync();
                }
            }
        }
        public int Q_QWhere_Count_trans(Expression<Func<T, bool>> predicate)
        {
            return db_Model.Where(predicate).Count();
        }
        public async Task<int> Q_QWhere_Count_transAsync(Expression<Func<T, bool>> predicate)
        {
            return await db_Model.Where(predicate).CountAsync();
        }

        //Add
        public EntityEntry<T> Add<TEntity>(TEntity newModel)
            where TEntity : ModelEntityCore
        {
            EntityEntry<T> resultt = null;
            lock (dBManagerCore.DBOpeningLock)
            {
                try
                {
                    using (var dbContext = dBManagerCore.GetNew_dbContext())
                    {
                        resultt = dbContext.Set<T>().Add(newModel as T);
                        if (resultt.State == EntityState.Added)
                        {
                            dbContext.SaveChanges();
                            _ModelNotify(DBModelNotifierType.Insert, resultt.Entity);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return resultt;
        }
        public async Task<EntityEntry<T>> AddAsync<TEntity>(TEntity newModel, bool OpeningLock = false)
            where TEntity : ModelEntityCore
        {
            EntityEntry<T> resultt = null;
            if (OpeningLock)
            {
                open_lock();
                try
                {
                    resultt = await db_Model.AddAsync(newModel as T);
                    if (resultt.State == EntityState.Added)
                    {
                        await db.SaveChangesAsync();
                        _ModelNotify(DBModelNotifierType.Insert, resultt.Entity);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    close_lock();
                    throw ex;
                }
                close_lock();
            }
            else
            {
                try
                {
                    using (var dbContext = dBManagerCore.GetNew_dbContext())
                    {
                        resultt = await dbContext.Set<T>().AddAsync(newModel as T);
                        if (resultt.State == EntityState.Added)
                        {
                            await dbContext.SaveChangesAsync();
                            _ModelNotify(DBModelNotifierType.Insert, resultt.Entity);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                catch (Exception ex)
                {
                    close_lock();
                    throw ex;
                }
            }
            return resultt;
        }
        public EntityEntry<T> Add_trans<TEntity>(TEntity newModel)
            where TEntity : ModelEntityCore
        {
            EntityEntry<T> resultt = null;
            try
            {
                resultt = db_Model.Add(newModel as T);
                if (resultt.State == EntityState.Added)
                {
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultt;
        }
        public async Task<EntityEntry<T>> Add_transAsync<TEntity>(TEntity newModel)
            where TEntity : ModelEntityCore
        {
            EntityEntry<T> resultt = null;
            try
            {
                resultt = await db_Model.AddAsync(newModel as T);
                if (resultt.State == EntityState.Added)
                {
                    await db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resultt;
        }

        #endregion

        #region Methods Add

        public EntityEntry<T> Add_Model<TEntity>(TEntity new_model)
            where TEntity : ModelEntityCore
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Add_Model(new_model);
            }
        }
        public EntityEntry<T> Add_Model_trans<TEntity>(TEntity new_model)
            where TEntity : ModelEntityCore
        {
            var temp_resultt = db_Model.Add(new_model as T);

            if (temp_resultt.State == EntityState.Added)
            {
                db.SaveChanges();
                _ModelNotify(DBModelNotifierType.Insert, temp_resultt.Entity);
            }
            else
            {
                throw new Exception();
            }
            return temp_resultt;
        }
        protected EntityEntry<T> _Add_Model<TEntity>(TEntity new_model)
            where TEntity : ModelEntityCore
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                var temp_resultt = db.Set<T>().Add(new_model as T);

                if (temp_resultt.State == EntityState.Added)
                {
                    db.SaveChanges();
                    _ModelNotify(DBModelNotifierType.Insert, temp_resultt.Entity);
                }
                else
                {
                    throw new Exception();
                }
                return temp_resultt;
            }
        }

        //AddOrGet
        public EntityEntry<T> AddOrGet(T new_model)
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _AddOrGet(new_model);
            }
        }
        protected EntityEntry<T> _AddOrGet(T new_model)
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                var temp_resultt = db.Set<T>().AddOrGet(new_model);

                if (temp_resultt.State == EntityState.Added)
                {
                    db.SaveChanges();
                    _ModelNotify(DBModelNotifierType.Insert, temp_resultt.Entity);
                }
                return temp_resultt;
            }
        }
        public EntityEntry<T> AddOrGet_trans(T new_model)
        {
            var temp_resultt = db_Model.AddOrGet(new_model);

            if (temp_resultt.State == EntityState.Added)
            {
                db.SaveChanges();
                _ModelNotify(DBModelNotifierType.Insert, temp_resultt.Entity);
            }
            return temp_resultt;
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
             // where TEntity : EntityCore.ModelEntityCore
        {

            db_Model.AddRange(new_models);
            int resultt_num = db.SaveChanges();
            if (resultt_num > 0)
            {
                _ModelNotify(DBModelNotifierType.Insert, new_models);
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
                        _ModelNotify(DBModelNotifierType.Insert, temp_resultt.Entity);
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
                        _ModelNotify(DBModelNotifierType.Update, temp_resultt.Entity);
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
            where TEntity : ModelEntityCore
        {
            open_lock();
            EntityEntry<T> temp_resultt = _Update_Model(model);
            close_lock();
            return temp_resultt;
        }
        public EntityEntry<T> Update_Model_trans<TEntity>(TEntity model)
            where TEntity : ModelEntityCore
        {
            return _Update_Model(model);
        }
        protected EntityEntry<T> _Update_Model<TEntity>(TEntity model)
            where TEntity : ModelEntityCore
        {
            var temp_resultt = db_Model.Update(model as T);

            if (temp_resultt.State == EntityState.Modified)
            {
                db.SaveChanges();
                _ModelNotify(DBModelNotifierType.Update, temp_resultt.Entity);
            }
            else
            {
                throw new Exception("The entity not exist in db");
            }
            return temp_resultt;
        }


        //UpdateRange
        public EntityEntry<T>[] UpdateRange<TEntity>(TEntity[] models)
            where TEntity : ModelEntityCore
        {
            open_lock();
            var temp_resultt = _UpdateRange(models);
            close_lock();
            return temp_resultt;
        }
        public EntityEntry<T>[] UpdateRange_trans<TEntity>(TEntity[] models)
            where TEntity : ModelEntityCore
        {
            return _UpdateRange(models);
        }
        protected EntityEntry<T>[] _UpdateRange<TEntity>(TEntity[] models)
            where TEntity : ModelEntityCore
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
                    _ModelNotify(DBModelNotifierType.Update, entry.Entity);
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

        #region Remove Methods

        //Remove
        public EntityEntry<T> Remove_Model<TEntity>(TEntity model)
            where TEntity : ModelEntityCore
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _Remove_Model(model);
            }
        }
        public EntityEntry<T> Remove_Model_trans<TEntity>(TEntity model)
           where TEntity : ModelEntityCore
        {
            var temp_resultt = db_Model.Remove(model as T);

            if (temp_resultt.State == EntityState.Deleted)
            {
                db.SaveChanges();
                _ModelNotify(DBModelNotifierType.Delete, temp_resultt.Entity);
            }
            else
            {
                throw new Exception("The entity not exist in db");
            }
            return temp_resultt;
        }
        protected EntityEntry<T> _Remove_Model<TEntity>(TEntity model)
            where TEntity : ModelEntityCore
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                var temp_resultt = db.Set<T>().Remove(model as T);

                if (temp_resultt.State == EntityState.Deleted)
                {
                    db.SaveChanges();
                    _ModelNotify(DBModelNotifierType.Delete, temp_resultt.Entity);
                }
                else
                {
                    throw new Exception("The entity not exist in db");
                }
                return temp_resultt;
            }
        }


        //RemoveRange
        public EntityEntry<T>[] RemoveRange<TEntity>(TEntity[] models)
            where TEntity : ModelEntityCore
        {
            lock (dBManagerCore.DBOpeningLock)
            {
                return _RemoveRange(models);
            }
        }
        public EntityEntry<T>[] RemoveRange_trans<TEntity>(TEntity[] models)
            where TEntity : ModelEntityCore
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
                this._ModelNotify(DBModelNotifierType.Delete, temp_List.Select(b => b.Entity).ToArray());
                return temp_List.ToArray();
            }
            else
            {
                if (save_resultt == 0)
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
        protected EntityEntry<T>[] _RemoveRange<TEntity>(TEntity[] models)
            where TEntity : ModelEntityCore
        {
            using (var db = dBManagerCore.GetNew_dbContext())
            {
                var temp_List = new List<EntityEntry<T>>();
                foreach (var model in models)
                {
                    var temp_resultt = db.Set<T>().Remove(model as T);
                    if (temp_resultt.State != EntityState.Deleted)
                        throw new Exception();
                    temp_List.Add(temp_resultt);
                }
                int save_resultt = db.SaveChanges();
                if (save_resultt > 0)
                {
                    this._ModelNotify(DBModelNotifierType.Delete, temp_List.Select(b => b.Entity).ToArray());
                    return temp_List.ToArray();
                }
                else
                {
                    if (save_resultt == 0)
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

        #region For Notify Events

        public readonly ModelEventNotifier<T, TId> modelEventNotifier;

        public void ModelNotify(DBModelNotifierType notifierType, T model)
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, model);
            }
        }
        protected void _ModelNotify(DBModelNotifierType notifierType, T model)
        {
            dBManagerCore.modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, model, defaultNull);
        }

        public void ModelNotify(DBModelNotifierType notifierType, TId modelId)
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, modelId);
            }
        }
        protected void _ModelNotify(DBModelNotifierType notifierType, TId modelId)
        {
            dBManagerCore.modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, modelId);
        }

        public void ModelNotify(DBModelNotifierType notifierType, TId modelId, T model = null)
        {
            //lock(dbLocker.opening_Lock) // because it loacked by notifier manager
            {
                _ModelNotify(notifierType, modelId, model);
            }
        }
        protected void _ModelNotify(DBModelNotifierType notifierType, TId modelId, T model = null)
        {
            dBManagerCore.modelNotifierControlsManager.ModelNotify(notifierType, modelId, model);
        }

        public void ModelNotify(DBModelNotifierType notifierType, Dictionary<TId, T> dic)
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, dic);
            }
        }
        protected void _ModelNotify(DBModelNotifierType notifierType, Dictionary<TId, T> dic)
        {
            dBManagerCore.modelNotifierControlsManager.ModelNotify(notifierType, dic);
        }

        public void ModelNotify(DBModelNotifierType notifierType, TId[] modelIds)
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, modelIds);
            }
        }
        protected void _ModelNotify(DBModelNotifierType notifierType, TId[] modelIds)
        {
            dBManagerCore.modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, modelIds);
        }

        public void ModelNotify(DBModelNotifierType notifierType, T[] models)
        {
            //lock (dbLocker.opening_Lock)
            {
                _ModelNotify(notifierType, models);
            }
        }
        protected void _ModelNotify(DBModelNotifierType notifierType, T[] models)
        {
            dBManagerCore.modelNotifierControlsManager.ModelNotify<T, TId>(notifierType, models, defaultNull);
        }

        #endregion

        #region IModelDBController

        void IModelDBController<T>.open_lock_Ex()
        {
            this.open_lock();
        }

        void IModelDBController<T>.close_lock_Ex()
        {
            this.close_lock();
        }
        #endregion

    }
}
