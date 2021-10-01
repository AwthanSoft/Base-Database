using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

//using System.Data.Entity.Migrations;
using Mawa.DBCore.ViewEntityCore;

namespace Mawa.DBCore.CtrlCore
{
    public sealed class ViewEntityDBController<T> : ModelDBController<T>
        where T: ModelViewEntityCore 
    {
        #region Initial

        public ViewEntityDBController(DBManagersControlCore dBManagerCore) : base(dBManagerCore)
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
            //var temp_model = db_ModelEntity.Find(ObjectId);
            //var temp_model = db_ModelEntity.Select(m => m.ObjectId.Equals(ObjectId)).Distinct();
            var temp_model = db_Model.Where(m => m.ObjectId.Equals(ObjectId)).FirstOrDefault();
            return temp_model;
        }

        #endregion

    }




}
