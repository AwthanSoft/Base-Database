using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.RepositoryBase.DBs.Results
{
    public enum DBModelOperationType
    {
        Unknown = 0,
        Add = 1,
        Update = 2,
        Delete = 3
    }
  

    public abstract class ModelOperationDBResult : DBResultCore
    {
        readonly DBModelOperationType _OperationType;
        public DBModelOperationType OperationType => _OperationType;
        public EntityState State { set; get; }

        public ModelOperationDBResult(DBModelOperationType OperationType) : base(ResultType.ModelOperation)
        {
            this._OperationType = OperationType;
        }
    }


    public class AddModelOperationDBResult<TModel> : ModelOperationDBResult
        where TModel : Mawa.BaseDBCore.EntityCore.IModelEntityCore
    {
        readonly TModel _Entity;
        public TModel Entity => _Entity;
        public TModel ResultEntity { set; get; }
        public AddModelOperationDBResult(TModel Entity) : base(DBModelOperationType.Add)
        {
            this._Entity = Entity;
        }
    }


}
