using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;

namespace DNNrocketAPI
{

    public abstract class APInterface
	{

        #region "Shared/Static Methods"

        // constructor
        static APInterface()
		{
		}

		// return the provider
        public static APInterface Instance(string assembly, string namespaceclass)
		{
            var handle = Activator.CreateInstance(assembly, namespaceclass);
            return (APInterface)handle.Unwrap();
		}

		#endregion

        public abstract string ProcessCommand(string paramCmd, SimplisityInfo sInfo, string editlang = "");

    }

}

