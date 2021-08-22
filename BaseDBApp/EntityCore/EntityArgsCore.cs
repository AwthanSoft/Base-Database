/*
using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Syncfusion.DataSource.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection;
*/
namespace DBAppCore.EntityCore
{
    public abstract class EntityArgsCore<T> where T : ModelEntityCore
    {
        internal DBManagersControlCore dBManager;
        public T model_entity;

        public EntityArgsCore(T model_entity, DBManagersControlCore dBManager)
        {
            this.dBManager = dBManager;
            this.model_entity = model_entity;
        }

        public void Refresh_ObjModel<TEntityArgs>() where TEntityArgs : EntityArgsCore<T>
        {
            model_entity = dBManager.GetEntityDBManager<T, TEntityArgs>().Get_Model_By_ObjectId(model_entity.ObjectId);
        }

        //public void Refresh_ObjModel();
        
        public string ObjectId => model_entity.ObjectId;

        public void Load(string propertyName)
        {
            dBManager.db.Entry(model_entity).Reference(propertyName).Load();
        }

    }

}
