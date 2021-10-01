using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

//using System.Data.Entity.Migrations;
using Mawa.DBCore.EntityCore;

namespace Mawa.DBCore.CtrlCore
{
    public sealed class EntityDBController<T> : ModelDBController<T>
        where T: ModelEntityCore 
    {
        #region Singleton

        public EntityDBController(DBManagersControlCore dBManagerCore) :base(dBManagerCore)
        {
            pre_refresh();
        }
        private void pre_refresh()
        {

        }

        #endregion

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
            var temp_model = db_Model.Where(m => m.ObjectId.Equals(ObjectId)).FirstOrDefault();
            return temp_model;
        }

        #endregion
        
    }




}
