using DNNrocketAPI;
using DNNrocketAPI.Componants;
using Simplisity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;

namespace RocketCatalog.Componants
{

    public class CategoryLimpetList
    {
        private string _langRequired;
        private List<CategoryLimpet> _articleList;
        private DNNrocketController _objCtrl;

        public CategoryLimpetList(int portalId, string tableName, string entityTypeCode, string langRequired, bool populate = true)
        {
            EntityTypeCode = entityTypeCode;
            TableName = tableName;
            PortalId = portalId;
            CultureCode = langRequired;
            if (CultureCode == "") CultureCode = DNNrocketUtils.GetCurrentCulture();
            _objCtrl = new DNNrocketController();

            if (populate) Populate();
        }
        public void Populate()
        {
            DataList = _objCtrl.GetList(PortalId, -1, EntityTypeCode, "", CultureCode, "", 0, 0, 0, 0, TableName);
        }
        public void DeleteAll()
        {
            foreach (var r in DataList)
            {
                _objCtrl.Delete(r.ItemID);
            }
        }

        public List<SimplisityInfo> DataList { get; private set; }
        public int PortalId { get; set; }
        public string TableName { get; set; }
        public string EntityTypeCode { get; set; }
        public string CultureCode { get; set; }
        public List<CategoryLimpet> GetCategoryList()
        {
            _articleList = new List<CategoryLimpet>();
            foreach (var o in DataList)
            {
                var articleData = new CategoryLimpet(PortalId, o.ItemID, TableName, EntityTypeCode, _langRequired);
                _articleList.Add(articleData);
            }
            return _articleList;
        }

        public void Validate()
        {
            foreach (var pInfo in DataList)
            {
                var categoryData = new CategoryLimpet(PortalId, pInfo.ItemID, TableName, EntityTypeCode, _langRequired);
                categoryData.ValidateAndUpdate();
            }
        }

    }

}
