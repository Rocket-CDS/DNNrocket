using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;
using DNNrocketAPI.Components;

namespace DNNrocketAPI
{

    public abstract class SchedulerInterface
    {
        #region "Shared/Static Methods"

        // constructor
        static SchedulerInterface()
		{
		}

		// return the provider
        public static SchedulerInterface Instance(string assembly, string namespaceclass)
		{
            var handle = Activator.CreateInstance(assembly, namespaceclass);
            return (SchedulerInterface)handle.Unwrap();
		}

        #endregion

        public abstract void DoWork(SystemLimpet systemData, RocketInterface rocketInterface);

    }

}

