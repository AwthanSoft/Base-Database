using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.BaseDBCore.EntityCore
{
    public interface IModelEntityCore : IDBModelCore
    {
        string ObjectId { get; set; }
        System.DateTime CreateDate { get; set; }
        System.DateTime ModifiedDate { get; set; }
    }
}
