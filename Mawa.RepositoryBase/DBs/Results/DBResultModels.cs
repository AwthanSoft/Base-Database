using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.RepositoryBase.DBs.Results
{
    public enum ResultType
    {
        Unknown = 0,
        ModelOperation = 1,
        Selection = 2
    }
    public abstract class DBResultCore
    {
        readonly ResultType _ResultType;
        public ResultType ResultType => _ResultType;


        public DBResultCore(ResultType ResultType)
        {
            this._ResultType = ResultType;
        }
    }
}
