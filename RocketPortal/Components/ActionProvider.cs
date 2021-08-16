using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPortal.Components
{
    public abstract class ActionProvider
    {
        #region "Shared/Static Methods"

        // constructor
        static ActionProvider()
        {
        }

        // return the provider
        public static ActionProvider Instance(string assembly, string namespaceclass)
        {           
            string objectToInstantiate = namespaceclass + ", " + assembly;
            var objectType = Type.GetType(objectToInstantiate);
            var instantiatedObject = Activator.CreateInstance(objectType);
            return (ActionProvider)instantiatedObject;
        }

        #endregion
        public abstract string DoAction(PortalLimpet portalShop, string actionData);

    }


}
