using System;
using System.Collections.Generic;
using System.Text;

namespace DBAppCore.NotifierCore.ViewEntity
{
    public delegate void OnViewEntity_Delegate<T>(T model) where T : ViewEntityCore.ModelViewEntityCore;

    public class ViewEntityEventNotifier<T>
        where T : ViewEntityCore.ModelViewEntityCore
    {
        public ViewEntityEventNotifier()
        {

        }

        #region For Events
        // add
        public event OnViewEntity_Delegate<T> OnView_Add;
        internal virtual void Model_Add(T model)
        {
            OnView_Add?.Invoke(model);
        }
        // update
        public event OnViewEntity_Delegate<T> OnView_Update;
        internal virtual void Model_Update(T model)
        {
            if (OnView_Update != null)
            {
                OnView_Update?.Invoke(model);
            }
        }
        // remove
        public event OnViewEntity_Delegate<T> OnView_Remove;
        internal virtual void Model_Remove(T model)
        {
            if (OnView_Remove != null)
            {
                OnView_Remove?.Invoke(model);
            }
        }
        // refresh
        public event OnViewEntity_Delegate<T> OnView_Refresh;
        internal virtual void Model_Refresh(T model)
        {
            if (OnView_Refresh != null)
            {
                OnView_Refresh?.Invoke(model);
            }
        }
        #endregion
    }

}
