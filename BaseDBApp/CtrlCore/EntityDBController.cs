using System.Linq;

//using System.Data.Entity.Migrations;
using Mawa.BaseDBCore.EntityCore;

namespace Mawa.DBCore.CtrlCore
{
    public sealed class EntityDBController<T, TId> : ModelDBController<T, TId>
        where T: ModelEntityCore
        where TId : struct
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
