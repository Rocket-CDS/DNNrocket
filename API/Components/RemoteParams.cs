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
    public class RemoteParams
    {
        public RemoteParams()
        {
            Record = new SimplisityRecord();
        }
        public void AddSystem(string systemKey)
        {
            var sRec = new SimplisityRecord();
            sRec.SetXmlProperty("genxml/systemkey", systemKey);
            Record.AddRecordListItem("systems", sRec);
        }
        public void RemoveSystems()
        {
            Record.RemoveRecordList("systems");
        }
        public List<SimplisityRecord> GetSystems()
        {
            return Record.GetRecordList("systems");
        }

        #region "properties"
        public SimplisityRecord Record { get; private set; }
        public string SecurityKey { get { return Record.GetXmlProperty("genxml/settings/securitykey"); } set { Record.SetXmlProperty("genxml/settings/securitykey", value); } }
        public string SecurityKeyEdit { get { return Record.GetXmlProperty("genxml/settings/securitykeyedit"); } set { Record.SetXmlProperty("genxml/settings/securitykeyedit", value); } }
        public string EngineURL { get { return Record.GetXmlProperty("genxml/settings/engineurl"); } set { Record.SetXmlProperty("genxml/settings/engineurl", value); } }
        public string RecordItemBase64 { get { return GeneralUtils.Base64Encode(Record.ToXmlItem()); } }

        #endregion


    }



}
