using Simplisity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNrocketAPI.Components
{
    public class Dependency
    {
        public Dependency()
        {
        }
        public Dependency(SimplisityRecord dependencyRec)
        {
            ctrltype = dependencyRec.GetXmlProperty("genxml/ctrltype");
            id = dependencyRec.GetXmlProperty("genxml/id");
            url = dependencyRec.GetXmlProperty("genxml/url");
            ignoreonskin = dependencyRec.GetXmlProperty("genxml/ignoreonskin");
            ecofriendly = dependencyRec.GetXmlPropertyBool("genxml/ecofriendly");
        }
        public bool IgnoreOnSkin(string skinSrc)
        {
            if (String.IsNullOrEmpty(ignoreonskin)) return false;
            var l = ignoreonskin.Split(',');
            var rtn = false;
            foreach (var i in l)
            {
                if (i != "")
                {
                    if (skinSrc.ToLower().Contains(i.ToLower())) return true;
                }
            }
            return rtn;
        }

        public string ctrltype { get; set; }
        public string id { get; set; }
        public string url { get; set; }
        public string ignoreonskin { get; set; }
        public bool ecofriendly { get; set; }

    }
}
