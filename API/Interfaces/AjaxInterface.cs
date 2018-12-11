﻿
using System.Linq;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;


using System.Runtime.Remoting;
using System.Web;
using NBrightDNN;

namespace DNNrocketAPI
{

    public abstract class AjaxInterface
	{

		#region "Shared/Static Methods"

		// singleton reference to the instantiated object 

	    private static Dictionary<String, AjaxInterface> _providerList; 
        // constructor
        static AjaxInterface()
		{
			CreateProvider();
		}

		// dynamically create provider
		private static void CreateProvider()
		{

		    _providerList = new Dictionary<string, AjaxInterface>();

            var pluginData = new PluginData(0);
		    var l = pluginData.GetAjaxProviders(false);

		    foreach (var p in l)
		    {
                try
                {
                    var prov = p.Value;
                    ObjectHandle handle = null;
                    handle = Activator.CreateInstance(prov.GetXmlProperty("genxml/textbox/assembly"), prov.GetXmlProperty("genxml/textbox/namespaceclass"));
                    var objProvider = (AjaxInterface)handle.Unwrap();
                    var ctrlkey = prov.GetXmlProperty("genxml/textbox/ctrl");
                    var lp = 1;
                    while (_providerList.ContainsKey(ctrlkey))
                    {
                        ctrlkey = ctrlkey + lp.ToString("");
                        lp += 1;
                    }
                    objProvider.Ajaxkey = ctrlkey;
                    _providerList.Add(ctrlkey, objProvider);
                }
                catch (Exception ex)
                {
                    // ignore, we may possibly have plugin data without assembly.
                }
		    }
		}


		// return the provider
        public static AjaxInterface Instance(String ctrlkey)
		{
            if (_providerList.ContainsKey(ctrlkey)) return _providerList[ctrlkey];
            CreateProvider(); // plugin not found, so reload and search again.
            if (_providerList.ContainsKey(ctrlkey)) return _providerList[ctrlkey];
            return null;
		}

		#endregion

        public abstract String Ajaxkey { get; set; }

        public abstract String ProcessCommand(string paramCmd, HttpContext context, string editlang = "");

        public abstract void Validate();

    }

}

