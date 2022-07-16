using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;
using DNNrocketAPI.Components;
using HandlebarsDotNet;

namespace DNNrocketAPI
{

    public abstract class RazorInterface
    {
        static RazorInterface()
		{
		}
        public static RazorInterface Instance(string assembly, string namespaceclass)
        {
            var handle = Activator.CreateInstance(assembly, namespaceclass);
            return (RazorInterface)handle.Unwrap();
        }
        public abstract string RenderToken(string interfaceKey, string cmd, SimplisityRazor model);
    }

}

