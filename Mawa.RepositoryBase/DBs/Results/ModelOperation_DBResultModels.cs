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
        //readonly bool _isRange;
        //public bool isRange=> _isRange;

        readonly DBModelOperationType _OperationType;
        public DBModelOperationType OperationType => _OperationType;
        public EntityState State { set; get; }
        public string Message { set; get; }

        public ModelOperationDBResult(DBModelOperationType OperationType) : base(ResultType.ModelOperation)
        {
            this._OperationType = OperationType;
            //this._isRange = isRange;
        }
    }

    public class ModelOperationDBResult<TModel> : ModelOperationDBResult
        where TModel : Mawa.BaseDBCore.IDBModelCore
    {
        readonly TModel _Entity;
        public TModel Entity => _Entity;
        public TModel ResultEntity { set; get; }
        public ModelOperationDBResult(DBModelOperationType OperationType, TModel Entity) : base(OperationType)
        {
            this._Entity = Entity;
        }

        ////Range
        //readonly TModel[] _Entities;
        //public TModel[] Entities => _Entities;
        //public TModel[] ResultEntities { set; get; }
        //public AddModelOperationDBResult(TModel[] Entities) : base(DBModelOperationType.Add, true)
        //{
        //    this._Entities = Entities;
        //}
    }


    //public class GeneralOperationDBResult<TModel> : ModelOperationDBResult
    //    where TModel : Mawa.BaseDBCore.IDBModelCore
    //{
    //    private readonly List<ModelOperationDBResult> _OperationsResults;
    //    public List<ModelOperationDBResult> OperationsResults
    //    {
    //        get => _OperationsResults;
    //    }
    //    public GeneralOperationDBResult(DBModelOperationType OperationType, bool isRange) : base(OperationType, isRange)
    //    {
    //        _OperationsResults = new List<ModelOperationDBResult>();
    //    }
    //}



    public class AddModelOperationDBResult<TModel> : ModelOperationDBResult<TModel>
        where TModel : Mawa.BaseDBCore.IDBModelCore
    {
        public AddModelOperationDBResult(TModel Entity) : base(DBModelOperationType.Add, Entity)
        {

        }
    }

    public class DeleteModelOperationDBResult<TModel> : ModelOperationDBResult<TModel>
       where TModel : Mawa.BaseDBCore.IDBModelCore
    {
        public DeleteModelOperationDBResult(TModel Entity) : base(DBModelOperationType.Delete, Entity)
        {

        }
    }

    public class UpdateModelOperationDBResult<TModel> : ModelOperationDBResult<TModel>
     where TModel : Mawa.BaseDBCore.IDBModelCore
    {
        public UpdateModelOperationDBResult(TModel Entity) : base(DBModelOperationType.Update, Entity)
        {

        }
    }

}
