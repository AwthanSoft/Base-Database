//using Mawa.BaseDBCore.EntityCore;

//namespace Mawa.DBCore.NotifierCore.Entity
//{
//    //public delegate void OnModel_Insert_Delegate<T>(T model) where T : class, IModelEntityCore;
//    //public delegate void OnModel_Insert_Delegate<T>(T model) where T : class, IModelEntityCore;
//    //public delegate void OnModel_Update_Delegate<T>(T model) where T : class, IModelEntityCore;
//    //public delegate void OnModel_Delete_Delegate<T, TId>(TId ModelId, T model = null) where T : class, IModelEntityCore;
//    //public delegate void OnModel_Refresh_Delegate<T>() where T : class, IModelEntityCore;

//    public class EntityEventNotifier<T, TId>
//        where T : class, IModelEntityCore
//    {
//        public EntityEventNotifier()
//        {

//        }

//        #region For Events
//        // add
//        public event OnModel_Insert_Delegate<T> OnModel_Add;
//        internal virtual void Model_Add(TId ModelId, T model)
//        {
//            OnModel_Add?.Invoke(model);
//        }
//        // update
//        public event OnModel_Update_Delegate<T> OnModel_Update;
//        internal virtual void Model_Update(T model)
//        {
//            if (OnModel_Update != null)
//            {
//                OnModel_Update?.Invoke(model);
//            }
//        }
//        // remove
//        public event OnModel_Delete_Delegate<T, TId> OnModel_Remove;
//        internal virtual void Model_Remove(TId ModelId,T model = null)
//        {
//            if (OnModel_Remove != null)
//            {
//                OnModel_Remove?.Invoke(ModelId, model);
//            }
//        }

//        public event OnModel_Refresh_Delegate<T> OnModel_Refresh;
//        internal virtual void Model_Refresh()
//        {
//            if (OnModel_Refresh != null)
//            {
//                OnModel_Refresh?.Invoke();
//            }
//        }

//        #endregion
//    }

//}
