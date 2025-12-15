using Simplisity;
using System.Collections.Generic;

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
        public string AccessCode { get { return Record.GetXmlProperty("genxml/settings/accesscode"); } set { Record.SetXmlProperty("genxml/settings/accesscode", value); } }
        public string AccessPassword { get { return Record.GetXmlProperty("genxml/settings/accesspassword"); } set { Record.SetXmlProperty("genxml/settings/accesspassword", value); } }
        public string RecordItemBase64 { get { return GeneralUtils.Base64Encode(Record.ToXmlItem()); } }

        #endregion


    }



}
