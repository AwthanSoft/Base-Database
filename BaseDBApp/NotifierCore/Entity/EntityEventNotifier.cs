﻿using DBAppCore.EntityCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBAppCore.NotifierCore.Entity
{
    public delegate void OnModel_Delegate<T>(T model) where T : ModelEntityCore;

    public class EntityEventNotifier<T>
        where T : ModelEntityCore
    {
        public EntityEventNotifier()
        {

        }

        #region For Events
        // add
        public event OnModel_Delegate<T> OnModel_Add;
        internal virtual void Model_Add(T model)
        {
            OnModel_Add?.Invoke(model);
        }
        // update
        public event OnModel_Delegate<T> OnModel_Update;
        internal virtual void Model_Update(T model)
        {
            if (OnModel_Update != null)
            {
                OnModel_Update?.Invoke(model);
            }
        }
        // remove
        public event OnModel_Delegate<T> OnModel_Remove;
        internal virtual void Model_Remove(T model)
        {
            if (OnModel_Remove != null)
            {
                OnModel_Remove?.Invoke(model);
            }
        }

        public event OnModel_Delegate<T> OnModel_Refresh;
        internal virtual void Model_Refresh(T model)
        {
            if (OnModel_Refresh != null)
            {
                OnModel_Refresh?.Invoke(model);
            }
        }

        #endregion
    }

}