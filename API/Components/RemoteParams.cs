using DNNrocketAPI;
using DotNetNuke.Entities.Modules;
using RazorEngine;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace DNNrocketAPI.Components
{
    /// <summary>
    /// Used to call remote engine and returns data
    /// </summary>
    public class RemoteParams
    {
        public RemoteParams(string systemKey)
        {
            Record = new SimplisityRecord();
            SystemKey = systemKey;
        }
        #region "properties"
        public SimplisityRecord Record { get; private set; }
        public bool Exists
        {
            get
            {
                if (Record.ItemID <= 0)
                {
                    return false;
                }
                return true;
            }
        }
        public string SystemKey { get { return Record.GetXmlProperty("genxml/settings/systemkey"); } set { Record.SetXmlProperty("genxml/settings/systemkey", value); } }
        public string RemoteAPI { get { return EngineURL.TrimEnd('/') + "/Desktopmodules/DNNrocket/api/rocket/actionremote"; } }
        public string EngineURL { get { return Record.GetXmlProperty("genxml/settings/engineurl"); } set { Record.SetXmlProperty("genxml/settings/engineurl", value); } }
        public string RemoteCmd { get { return Record.GetXmlProperty("genxml/settings/remotecmd"); } set { Record.SetXmlProperty("genxml/settings/remotecmd", value); } }
        public string AdminUrl { get { return EngineURL.TrimEnd('/') + "/" + SystemKey; } }
        public string RecordItemBase64 { get { return GeneralUtils.Base64Encode(Record.ToXmlItem()); } }

        #endregion


    }



}
