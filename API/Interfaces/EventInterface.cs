using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;

namespace DNNrocketAPI
{

    public abstract class EventInterface
	{
        #region "Shared/Static Methods"

        // constructor
        static EventInterface()
		{
		}

		// return the provider
        public static EventInterface Instance(string assembly, string namespaceclass)
		{
            var handle = Activator.CreateInstance(assembly, namespaceclass);
            return (EventInterface)handle.Unwrap();
		}

        #endregion

        public abstract Dictionary<string, SimplisityInfo> BeforeEvent(string paramCmd, SystemInfoData systemInfoData, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "");

        public abstract Dictionary<string, string> AfterEvent(string paramCmd, SystemInfoData systemInfoData, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "");

    }

}

