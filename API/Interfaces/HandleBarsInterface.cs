using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;
using DNNrocketAPI.Components;
using HandlebarsDotNet;

namespace DNNrocketAPI
{

    public abstract class HandleBarsInterface
    {
        static HandleBarsInterface()
		{
		}
        public static HandleBarsInterface Instance(string assembly, string namespaceclass)
        {
            var handle = Activator.CreateInstance(assembly, namespaceclass);
            return (HandleBarsInterface)handle.Unwrap();
        }
        public abstract void RegisterHelpers(ref IHandlebars hbs);
    }

}

