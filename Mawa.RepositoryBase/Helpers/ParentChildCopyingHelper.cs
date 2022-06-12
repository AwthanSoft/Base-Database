using System;

namespace Mawa.RepositoryBase.Helpers
{
    //https://www.pluralsight.com/guides/property-copying-between-two-objects-using-reflection
    public static class ParentChildCopyingHelper
    {
        public static TChild CopyToChild<TParent, TChild>(TParent parent)
            where TParent : class
            where TChild : TParent
        {
            var child = System.Activator.CreateInstance<TChild>();
            var parentProperties = parent.GetType().GetProperties();
            var childProperties = child.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                    {
                        childProperty.SetValue(child, parentProperty.GetValue(parent));
                        break;
                    }
                }
            }

            return child;
        }

    }
}
