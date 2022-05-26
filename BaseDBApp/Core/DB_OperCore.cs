using Mawa.Lock;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.Storage;
//using Xamarin.Forms.PlatformConfiguration;

namespace Mawa.DBCore
{
    public abstract class DB_OperCore
    {
        #region Singleton

        internal DbContext _db;
        public DbContext db => _db;

        #endregion

        #region Lock Driver Properties

        private readonly ObjectLock objectLock = new ObjectLock();
        internal ObjectLock dbLocker => objectLock;
        public object DBOpeningLock => objectLock.opening_Lock;

        public void open_Lock()
        {
            objectLock.open_lock();
            _db = getNewDbContext();
        }

        public void close_Lock()
        {
            _db.Dispose();
            _db = null;
            objectLock.close_lock();
        }

        #endregion

        #region Abstract Methods

        abstract protected DbContext getNewDbContext();

        internal DbContext GetNew_dbContext() => getNewDbContext();

        #endregion

    }
}
