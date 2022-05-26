using System;
using System.Collections.Generic;
using System.Text;

namespace Mawa.RepositoryBase.DBs.Results
{
    //
    // Summary:
    //     The state in which an entity is being tracked by a context.
    public enum EntityState
    {
        //
        // Summary:
        //     The entity is not being tracked by the context.
        Detached,
        //
        // Summary:
        //     The entity is being tracked by the context and exists in the database. Its property
        //     values have not changed from the values in the database.
        Unchanged,
        //
        // Summary:
        //     The entity is being tracked by the context and exists in the database. It has
        //     been marked for deletion from the database.
        Deleted,
        //
        // Summary:
        //     The entity is being tracked by the context and exists in the database. Some or
        //     all of its property values have been modified.
        Modified,
        //
        // Summary:
        //     The entity is being tracked by the context but does not yet exist in the database.
        Added
    }
}
