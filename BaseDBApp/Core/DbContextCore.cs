﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DBAppCore
{
    using Microsoft.EntityFrameworkCore;
    //using System.Data.Entity;
    using System;
    using System.Collections.Generic;

    public class DbContextCore : IDisposable
    {
        readonly DbContext _db;
        public DbContext db => _db;
        readonly Dictionary<Type, object> _temp_types_dic;

        public DbContextCore(object appDbContext , Dictionary<Type, object> types_dic)
        {
            this._db = appDbContext as DbContext;
            this._temp_types_dic = types_dic;
        }

        internal object  db_Model(Type T)
        {
            return _temp_types_dic[T];
        }

        #region Dispose

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.

            }

            // Free any unmanaged objects here.
            _db.Dispose();

            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~DbContextCore()
        {
            Dispose(false);
        }

        #endregion
    }


}
