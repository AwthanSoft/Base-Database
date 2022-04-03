using Mawa.BaseDBCore.ViewEntityCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Mawa.DBCore.ViewEntityCore
{
    public abstract class ViewEntityViewModel<TViewEntity> : INotifyPropertyChanged 
        where TViewEntity : ModelViewEntityCore 
    {

        private TViewEntity _viewEntity;
        public TViewEntity viewEntity
        {
            get => _viewEntity;
            set
            {
                _viewEntity = value;
                OnModelViewChanged(value);
            }
        }

        public string ObjectId => viewEntity.ModelObjectId;

        public ViewEntityViewModel(TViewEntity viewEntity)
        {
            _viewEntity = viewEntity;
        }



        protected abstract void OnModelViewChanged(TViewEntity modelView);

        #region fiers
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
