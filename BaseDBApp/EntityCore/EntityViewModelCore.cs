using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mawa.DBCore.EntityCore
{
    public abstract class EntityViewModelCore<TEntity ,TEntityArgs> : INotifyPropertyChanged where TEntity : ModelEntityCore where TEntityArgs : EntityArgsCore<TEntity>
    {
    
        readonly TEntityArgs _modelArgs;
        public TEntityArgs modelArgs => _modelArgs;

        public EntityViewModelCore(TEntityArgs modelArgs)
        {
            _modelArgs = modelArgs;
        }

        #region fiers
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
