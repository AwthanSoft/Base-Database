using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

//using System.Data.Entity.Migrations;
using Mawa.DBCore.NotifierCore;
using Mawa.DBCore.NotifierCore.Entity;

namespace Mawa.DBCore.EntityCore
{
    //public delegate void OnModelArgs_Delegate<T, TEntityArgs>(TEntityArgs modelArgs) where T : ModelEntity where TEntityArgs : ModelEntityArgs<T>;

    public abstract class EntityDBManagerCore<T , TArgs> : ModelDBController<T>
        where T: ModelEntityCore 
        where TArgs : EntityArgsCore<T>
        //where TViewModel : ModelViewModel<T,TArgs>
    {
        #region Singleton

        public EntityDBManagerCore(DBManagersControlCore dBManagerCore) : base(dBManagerCore)
        {
            pre_refresh();
        }
        private void pre_refresh()
        {
            
        }

        #endregion

        

        #region For Methods

        #region For Methods get

        public T Get_Model_By_ObjectId(string ObjectId)
        {
            open_lock();
            var resultt = _Get_Model_By_ObjectId(ObjectId);
            close_lock();
            return resultt;
        }
        private T _Get_Model_By_ObjectId(string ObjectId)
        {
            //var temp_model = db_ModelEntity.Find(ObjectId);
            //var temp_model = db_ModelEntity.Select(m => m.ObjectId.Equals(ObjectId)).Distinct();
            var temp_model = db_Model.Where(m => m.ObjectId.Equals(ObjectId)).FirstOrDefault();
            return temp_model;
        }

        public TArgs Get_ModelArgs_By_ObjectId(string ObjectId)
        {
            open_lock();
            var resultt = _Get_ModelArgs_By_ObjectId(ObjectId);
            close_lock();
            return resultt;
        }
        private TArgs _Get_ModelArgs_By_ObjectId(string ObjectId)
        {
            //var temp_model = db_ModelEntity.Find(ObjectId, "ObjectId");
            var temp_model = db_Model.Where(m => m.ObjectId.Equals(ObjectId)).FirstOrDefault();
            if (temp_model == null)
                return null;
            else
                return To_ModelArgs(temp_model);
        }
        
        public List<TArgs> Get_ModelArgs_All()
        {
            open_lock();
            var tempList = _Get_ModelArgs_All();
            close_lock();
            return tempList;
        }
        private List<TArgs> _Get_ModelArgs_All()
        {
            var tempList = db_Model.ToArray();
            var tt = new List<TArgs>();

            foreach (var p in tempList)
            {
                tt.Add(To_ModelArgs(p));
            }

            return tt;
        }


        //Args
        public virtual TArgs Get_ModelArgs_By_Properties(params object[] keyValues)
        {
            open_lock();
            TArgs temp_model = _Get_ModelArgs_By_Properties(keyValues);
            close_lock();
            return temp_model;
        }
        public TArgs Get_ModelArgs_By_Properties_trans(params object[] keyValues)
        {
            return _Get_ModelArgs_By_Properties(keyValues);
        }
        private TArgs _Get_ModelArgs_By_Properties(params object[] keyValues)
        {
            return To_ModelArgs(_Get_Model_By_Properties(keyValues));
        }


        #endregion

        #region Methods Add

      
        #endregion

        public abstract TArgs To_ModelArgs(T model);


        #endregion


        #region For Searching Methods 



        #endregion


    }




}
