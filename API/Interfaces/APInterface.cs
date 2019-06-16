using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;

namespace DNNrocketAPI
{

    public abstract class APInterface
	{

        public static string ControlRelPath;

        #region "Shared/Static Methods"

        // constructor
        static APInterface()
		{
		}

		// return the provider
        public static APInterface Instance(string assembly, string namespaceclass, string controlRelPath = "")
		{
            ControlRelPath = controlRelPath;
            var handle = Activator.CreateInstance(assembly, namespaceclass);
            return (APInterface)handle.Unwrap();
		}

        #endregion

        public abstract Dictionary<string,string> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string userHostAddress, string langRequired = "");

    }

}

