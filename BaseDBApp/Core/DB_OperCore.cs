using CommonAppCore.Locks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.Storage;
//using Xamarin.Forms.PlatformConfiguration;

namespace DBAppCore
{
    public abstract class DB_OperCore
    {
        #region Singleton

        private DbContextCore _dbContextCore;
        internal DbContextCore dbContextCore => _dbContextCore;
        public DbContext db => _dbContextCore.db;

        #endregion

        #region Lock Driver Properties

        private ObjectLock objectLock = new ObjectLock();
        //public ObjectLock DBLock => objectLock;
        public object DBOpeningLock => objectLock.opening_Lock;

        public void open_Lock()
        {
            objectLock.open_lock();
            _dbContextCore = getNewDbContextCore();
        }

        public void close_Lock()
        {
            _dbContextCore.Dispose();
            _dbContextCore = null;
            objectLock.close_lock();
        }

        #endregion

        #region Abstract Methods

        //abstract protected DbContextCore getDbContextCore();
        abstract protected DbContextCore getNewDbContextCore();

        #endregion

    }
}
